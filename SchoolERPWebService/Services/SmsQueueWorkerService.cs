using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class SmsQueueWorkerService: BackgroundService
    {
        private readonly string _cs;
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISmsQueueSignal _signal;
        private readonly ILogger<SmsQueueWorkerService> _log;

        private readonly int _batchSize;
        private readonly int _maxRetries;
        private readonly int _delayMs;

        public SmsQueueWorkerService(IConfiguration config, IServiceScopeFactory scopeFactory,
                                     ISmsQueueSignal signal, ILogger<SmsQueueWorkerService> log)
        {
            _config = config;
            _cs = config.GetConnectionString("DefaultConnection")!;
            _scopeFactory = scopeFactory;
            _signal = signal;
            _log = log;

            _batchSize = config.GetValue("Sms:BatchSize", 20);
            _maxRetries = config.GetValue("Sms:MaxRetries", 3);
            _delayMs = config.GetValue("Sms:DelayBetweenMs", 200);
        }

        // ------------------------------------------------------------------
        //  MAIN LOOP
        // ------------------------------------------------------------------
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _log.LogInformation("SMS worker STARTED pid={Pid} machine={M} batch={B} maxRetries={R} dryRun={D}",
                Environment.ProcessId, Environment.MachineName, _batchSize, _maxRetries, _config.GetValue<bool>("Sms:DryRun"));

            // crash recovery: rows left in "Processing" by a previous run go back to Pending
            try
            {
                using var con = new SqlConnection(_cs);
                await con.ExecuteAsync("dbo.usp_SmsQueue_ResetProcessing", commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex) { _log.LogError(ex, "SMS worker startup recovery failed"); }

            while (!ct.IsCancellationRequested)
            {
                int processed;
                try
                {
                    processed = await ProcessBatchAsync(ct);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    _log.LogError(ex, "SMS worker cycle failed");
                    processed = -1;
                }

                // keep going immediately while there is work; otherwise wait (or until Notify())
                if (processed <= 0)
                {
                    try { await _signal.WaitAsync(TimeSpan.FromSeconds(10), ct); }
                    catch (OperationCanceledException) { break; }
                }
            }

            _log.LogWarning("SMS worker STOPPING (host shutdown / app pool recycle).");
        }

        // ------------------------------------------------------------------
        //  ONE CYCLE
        //  (scheduled messages are queued by the email worker's usp_Communication_ProcessDue call,
        //   which also fills SmsQueue, so it is not called again here)
        // ------------------------------------------------------------------
        private async Task<int> ProcessBatchAsync(CancellationToken ct)
        {
            using var con = new SqlConnection(_cs);

            // 1) claim rows atomically
            var items = (await con.QueryAsync<SmsQueueItem>(
                "dbo.usp_SmsQueue_ClaimBatch",
                new { BatchSize = _batchSize },
                commandType: CommandType.StoredProcedure)).ToList();

            if (items.Count == 0) return 0;

            _log.LogInformation("pid={Pid} claimed {Count} sms", Environment.ProcessId, items.Count);

            // the sms service uses scoped dependencies (repositories), so resolve it from a scope
            using var scope = _scopeFactory.CreateScope();
            var sms = scope.ServiceProvider.GetRequiredService<ISmsService>();     // <- your interface name
            var dryRun = _config.GetValue<bool>("Sms:DryRun");

            foreach (var item in items)
            {
                if (ct.IsCancellationRequested)
                {
                    await ReleaseAsync(con, item.QueueID);
                    continue;
                }
                string mobileno = "+91" + item.ToMobile;
                try
                {
                    if (dryRun)
                    {
                        _log.LogWarning("DRY RUN: sms {Id} to {To} NOT sent", item.QueueID, item.ToMobile);
                        var result = await sms.SendAsync(item.CompanyID, mobileno, item.Message);
                    }
                    else
                    {
                        var result = await sms.SendAsync(item.CompanyID, mobileno, item.Message);
                        if (!result.Success)
                            throw new InvalidOperationException(result.Message);
                    }

                    // reached ONLY if the gateway accepted the message
                    await con.ExecuteAsync("dbo.usp_SmsQueue_MarkSent",
                        new { QueueID = item.QueueID, ProviderMessageId = (string?)null },
                        commandType: CommandType.StoredProcedure);

                    _log.LogInformation("pid={Pid} sms {Id} sent to {To}", Environment.ProcessId, item.QueueID, item.ToMobile);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    await ReleaseAsync(con, item.QueueID);
                }
                catch (Exception ex)
                {
                    var retry = item.RetryCount + 1;
                    var status = retry >= _maxRetries ? 3 : 0;               // 3 = Failed, 0 = try again later
                    var error = ex.InnerException?.Message ?? ex.Message;

                    _log.LogWarning("sms {Id} to {To} failed (attempt {Retry}): {Error}",
                        item.QueueID, item.ToMobile, retry, error);

                    await con.ExecuteAsync("dbo.usp_SmsQueue_MarkFailed",
                        new { QueueID = item.QueueID, Status = status, RetryCount = retry, ErrorMessage = error },
                        commandType: CommandType.StoredProcedure);
                }

                try { await Task.Delay(_delayMs, ct); } catch (OperationCanceledException) { }
            }

            return items.Count;
        }

        private static Task ReleaseAsync(SqlConnection con, long queueId) =>
            con.ExecuteAsync("dbo.usp_SmsQueue_Release",
                new { QueueID = queueId },
                commandType: CommandType.StoredProcedure);

        // ------------------------------------------------------------------
        //  MODEL  (matches the columns returned by usp_SmsQueue_ClaimBatch)
        // ------------------------------------------------------------------
        private class SmsQueueItem
        {
            public long QueueID { get; set; }
            public int? CommunicationId { get; set; }
            public int CompanyID { get; set; }
            public string ToMobile { get; set; } = "";
            public string Message { get; set; } = "";
            public string? DltTemplateId { get; set; }
            public int RetryCount { get; set; }
        }
    
}
}
