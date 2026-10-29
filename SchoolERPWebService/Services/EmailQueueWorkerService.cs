using System.Data;
using System.Net;
using System.Net.Mail;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class EmailQueueWorkerService : BackgroundService
    {
        private readonly string _cs;
        private readonly EmailSettings _smtp;
        private readonly ILogger<EmailQueueWorkerService> _log;
        private readonly IConfiguration _config;
        private readonly IEmailQueueSignal _signal;
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(60) };
        private DateTime _lastDueCheck = DateTime.MinValue;

        public EmailQueueWorkerService(IConfiguration config, 
            IOptions<EmailSettings> smtp, ILogger<EmailQueueWorkerService> log, IEmailQueueSignal signal)
        {
            _config = config;
            _cs = config.GetConnectionString("DefaultConnection")!;
            _smtp = smtp.Value;
            _log = log;
            _signal = signal;
        }

        public async Task<MstEmailConfigViewModel?> GetConfigAsync(int companyId)
        {
            using var conn = new SqlConnection(_cs);
            return await conn.QueryFirstOrDefaultAsync<MstEmailConfigViewModel>(
                "dbo.sp_EmailConfig_Get",
                new { CompanyId = companyId },
                commandType: CommandType.StoredProcedure);
        }

        // ==================================================================
        //  MAIN LOOP  (runs until the host shuts the process down)
        // ==================================================================
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _log.LogInformation("Email worker STARTED (attachments-v3). BatchSize={Batch} MaxRetries={Max} host={Host} user={User} passwordSet={HasPwd}",
                _smtp.BatchSize, _smtp.MaxRetries, _smtp.Host, _smtp.UserName, !string.IsNullOrEmpty(_smtp.Password));

            // crash recovery: rows left in "Processing" by a previous run go back to Pending
            try
            {
                using var con = new SqlConnection(_cs);
                await con.ExecuteAsync("dbo.usp_EmailQueue_ResetProcessing", commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex) { _log.LogError(ex, "Email worker startup recovery failed"); }

            while (!ct.IsCancellationRequested)
            {
                int processed;
                try
                {
                    processed = await ProcessBatchAsync(ct);
                }
                catch (OperationCanceledException) { break; }               // host is stopping
                catch (Exception ex)
                {
                    _log.LogError(ex, "Email worker cycle failed");
                    processed = -1;
                }

                // keep going immediately while there is work; otherwise wait a little
                if (processed <= 0)
                {
                    try
                    {
                        try { await _signal.WaitAsync(TimeSpan.FromSeconds(10), ct); }
                        catch (OperationCanceledException) { break; }
                    }

                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }

            _log.LogWarning("Email worker STOPPING (host shutdown / app pool recycle).");
        }


        public async Task<Dictionary<long, string?>> GetAttachmentPathsAsync(IEnumerable<long> queueIds)
        {
            var ids = queueIds.Distinct().ToList();
            if (ids.Count == 0) return new Dictionary<long, string?>();

            using var con = new SqlConnection(_cs);
            var rows = await con.QueryAsync<PathRow>(
                "dbo.SP_EmailQueue_GetAttachmentPaths",
                new { QueueIds = string.Join(",", ids) },
                commandType: CommandType.StoredProcedure);

            return rows.ToDictionary(r => r.QueueID, r => r.AttachmentPath);
        }

        // ==================================================================
        //  ONE CYCLE
        // ==================================================================
        private async Task<int> ProcessBatchAsync(CancellationToken ct)
        {
            // scheduled messages whose time has come -> EmailQueue (throttled to every 30 s)
            await QueueDueCommunicationsAsync(ct);

            using var con = new SqlConnection(_cs);

            // 1) claim rows atomically (safe even if two app instances run)
            var items = (await con.QueryAsync<QueueItem>(
                "dbo.usp_EmailQueue_ClaimBatch",
                new { BatchSize = _smtp.BatchSize },
                commandType: CommandType.StoredProcedure)).ToList();

            if (items.Count == 0) return 0;

            // 2) ALWAYS read the attachment path from the table itself.
            //    (the claim SP does not return that column, so item.AttachmentPath was always empty)
            var queueIds = new List<string>();

            foreach (var item in items)
            {
                queueIds.Add(item.QueueID.ToString());
            }

            var pathRows = await con.QueryAsync<PathRow>(
                "dbo.SP_EmailQueue_GetAttachmentPaths",
                new { QueueIds = string.Join(",", queueIds) },
                commandType: CommandType.StoredProcedure);

            var paths = new Dictionary<long, string?>();

            foreach (var item in pathRows)
            {
                paths[item.QueueID] = item.AttachmentPath;
            }

            _log.LogInformation("Claimed {Count} email(s)", items.Count);
            var cfgCache = new Dictionary<int, MstEmailConfigViewModel?>();
            foreach (var item in items)
            {
                if (ct.IsCancellationRequested)
                {
                    await ReleaseAsync(con, item.QueueID);      // not sent: put back as pending
                    continue;
                }

                paths.TryGetValue(item.QueueID, out var attachPath);
                _log.LogInformation("Sending {Id} to {To}, attachment='{Path}'", item.QueueID, item.ToEmail, attachPath);

                try
                {
                    if (!cfgCache.TryGetValue(item.CompanyID, out var cfg))
                    {
                        cfg = await GetConfigAsync(item.CompanyID);
                        cfgCache[item.CompanyID] = cfg;
                    }
                    if (cfg == null)
                        throw new InvalidOperationException($"No active email config found for CompanyID {item.CompanyID}.");

                    var fromEmail = (cfg.SMTPUsername?.Contains('@') == true) ? cfg.SMTPUsername : _smtp.FromEmail;

                    using var msg = new MailMessage
                    {
                        From = new MailAddress(fromEmail!, _smtp.FromName),
                        Subject = item.Subject,
                        Body = item.Body,
                        IsBodyHtml = true
                    };
                    msg.To.Add(item.ToEmail);

                    if (!string.IsNullOrWhiteSpace(attachPath))
                    {
                        var att = await LoadAttachmentAsync(attachPath, ct)
                                  ?? throw new FileNotFoundException($"Attachment not found: {attachPath}");
                        msg.Attachments.Add(att);
                    }

                    if (_config.GetValue<bool>("Email:DryRun"))
                    {
                        _log.LogWarning("DRY RUN: email {Id} to {To} NOT sent", item.QueueID, item.ToEmail);
                    }
                    else
                    {
                        using var client = CreateClient(cfg);
                        await client.SendMailAsync(msg);          // real send
                    }

                    // reached ONLY if the send did not throw
                    await con.ExecuteAsync("dbo.usp_EmailQueue_MarkSent",
                        new { QueueID = item.QueueID },
                        commandType: CommandType.StoredProcedure);

                    _log.LogInformation("Email {Id} sent to {To}", item.QueueID, item.ToEmail);
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    // host stopped in the middle of this mail: not a failure, do not count a retry
                    await ReleaseAsync(con, item.QueueID);
                }
                catch (Exception ex)
                {
                    var retry = item.RetryCount + 1;
                    var status = retry >= _smtp.MaxRetries ? 3 : 0;      // 3 = Failed, 0 = try again later
                    var error = ex.InnerException?.Message ?? ex.Message;

                    _log.LogWarning("Email {QueueID} to {To} failed (attempt {Retry}): {Error}",
                        item.QueueID, item.ToEmail, retry, error);

                    await con.ExecuteAsync("dbo.usp_EmailQueue_MarkFailed",
                        new { QueueID = item.QueueID, Status = status, RetryCount = retry, ErrorMessage = error },
                        commandType: CommandType.StoredProcedure);
                }

                try { await Task.Delay(_smtp.DelayBetweenMs, ct); } catch (OperationCanceledException) { }
            }

            return items.Count;
        }

        private static Task ReleaseAsync(SqlConnection con, long queueId) =>
            con.ExecuteAsync("dbo.usp_EmailQueue_Release",
                new { QueueID = queueId },
                commandType: CommandType.StoredProcedure);

        // ==================================================================
        //  SCHEDULED MESSAGES -> EmailQueue
        // ==================================================================
        private async Task QueueDueCommunicationsAsync(CancellationToken ct)
        {
            if (DateTime.UtcNow - _lastDueCheck < TimeSpan.FromMinutes(1)) return;
            _lastDueCheck = DateTime.UtcNow;

            try
            {
                using var con = new SqlConnection(_cs);
                await con.ExecuteAsync(new CommandDefinition(
                    "dbo.usp_Communication_ProcessDue",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _log.LogError(ex, "Queueing due communications failed");
            }
        }

        // ==================================================================
        //  SMTP
        // ==================================================================
        private static SmtpClient CreateClient(MstEmailConfigViewModel c)
        {
            var client = new SmtpClient(c.SMTPServer, c.SMTPPort)
            {
                EnableSsl = string.Equals(c.SslTls, "tls", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(c.SslTls, "ssl", StringComparison.OrdinalIgnoreCase),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 30000
            };

            client.UseDefaultCredentials = false;
            if (string.Equals(c.SMTPAuth, "yes", StringComparison.OrdinalIgnoreCase))
                client.Credentials = new NetworkCredential(c.SMTPUsername, c.SMTPPassword);

            return client;
        }

        // ==================================================================
        //  ATTACHMENT LOADING  (disk path, full url, or web-relative path)
        // ==================================================================
        // Root URLs to try, in order: web console first (it saves the uploads), then the API
        private IEnumerable<string> RootUrls()
        {
            foreach (var key in new[] { "ApiSettings:WebUrl", "ApiSettings:BaseUrl" })
            {
                var v = _config[key];
                if (!string.IsNullOrWhiteSpace(v)) yield return v.TrimEnd('/');
            }
        }

        private async Task<Attachment?> LoadAttachmentAsync(string path, CancellationToken ct)
        {
            path = path.Trim();
            var rel = path.TrimStart('~', '/');

            // optional: files on this server's disk (set Attachments:RootPath to the folder that contains "EmailTemplate")
            var root = _config["Attachments:RootPath"];
            if (!string.IsNullOrWhiteSpace(root))
            {
                var local = Path.Combine(root, rel.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(local)) return new Attachment(local);
            }

            // already a real disk path
            if (Path.IsPathRooted(path) && !path.StartsWith("/") && File.Exists(path))
                return new Attachment(path);

            // full http(s) url
            if (Uri.TryCreate(path, UriKind.Absolute, out var abs) &&
                (abs.Scheme == Uri.UriSchemeHttp || abs.Scheme == Uri.UriSchemeHttps))
                return await DownloadAsync(abs, ct);

            // web-relative path -> try each root url from appsettings
            foreach (var r in RootUrls())
            {
                try
                {
                    var att = await DownloadAsync(new Uri($"{r}/{rel}"), ct);
                    if (att != null) return att;
                }
                catch (HttpRequestException ex)
                {
                    _log.LogWarning("Attachment download failed from {Root}: {Error}", r, ex.Message);
                }
            }
            return null;
        }

        private static async Task<Attachment?> DownloadAsync(Uri uri, CancellationToken ct)
        {
            var bytes = await _http.GetByteArrayAsync(uri, ct);
            var name = Path.GetFileName(Uri.UnescapeDataString(uri.LocalPath));
            return new Attachment(new MemoryStream(bytes), string.IsNullOrWhiteSpace(name) ? "attachment" : name);
        }

        // ==================================================================
        //  MODELS
        // ==================================================================
        private class QueueItem
        {
            public long QueueID { get; set; }
            public string ToEmail { get; set; } = "";
            public string Subject { get; set; } = "";
            public string Body { get; set; } = "";
            public string? AttachmentPath { get; set; }      // not filled by the claim SP - read via PathRow instead
            public int RetryCount { get; set; }
            public int CompanyID { get; set; }
        }

        private class PathRow
        {
            public long QueueID { get; set; }
            public string? AttachmentPath { get; set; }
        }
    }
}