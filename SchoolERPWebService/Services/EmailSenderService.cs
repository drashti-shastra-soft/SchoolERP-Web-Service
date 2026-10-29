using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Collections.Concurrent;
using System.Data;
using System.Net.Mail;
using System.Net;

namespace SchoolERPWebService.Services
{
    public class EmailSenderService: IEmailSenderService
    {
        private readonly string _cs;
        private readonly EmailSettings _smtp;                 // only FromName / FromEmail fallback
        private readonly ILogger<EmailSenderService> _log;
       // private readonly ConcurrentDictionary<int, (SmtpConfig? Cfg, DateTime LoadedAt)> _cache = new();

        public EmailSenderService(IConfiguration cfg, IOptions<EmailSettings> smtp, ILogger<EmailSenderService> log)
        {
            _cs = cfg.GetConnectionString("DefaultConnection")!;
            _smtp = smtp.Value;
            _log = log;
        }

        private class SmtpConfig
        {
            public string SMTPServer { get; set; } = "";
            public int SMTPPort { get; set; }
            public string? SMTPUsername { get; set; }
            public string? SMTPPassword { get; set; }
            public string? SslTls { get; set; }
            public string? SMTPAuth { get; set; }
        }

        public async Task<MstEmailConfigViewModel?> GetConfigAsync(int companyId)
        {
            using var conn = new SqlConnection(_cs);
            return await conn.QueryFirstOrDefaultAsync<MstEmailConfigViewModel>(
                "dbo.sp_EmailConfig_Get",
                new { CompanyId = companyId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<EmailSendResult> SendAsync(EmailMessage m, CancellationToken ct = default)
        {
            try
            {
                if (!MailAddress.TryCreate(m.To?.Trim(), out var toAddr))
                    return EmailSendResult.Fail("Invalid recipient email address.");                
                var cfg = await GetConfigAsync(m.CompanyId);
                if (cfg == null)
                    return EmailSendResult.Fail($"No active email config found for CompanyID {m.CompanyId}.");

                var fromEmail = (cfg.SMTPUsername?.Contains('@') == true) ? cfg.SMTPUsername : _smtp.FromEmail;

                using var msg = new MailMessage
                {
                    From = new MailAddress(fromEmail!, _smtp.FromName),
                    Subject = m.Subject,
                    Body = m.Body,
                    IsBodyHtml = m.IsHtml
                };
                msg.To.Add(toAddr!);

                foreach (var cc in m.Cc ?? new()) if (MailAddress.TryCreate(cc, out var a)) msg.CC.Add(a!);
                foreach (var bc in m.Bcc ?? new()) if (MailAddress.TryCreate(bc, out var a)) msg.Bcc.Add(a!);

                foreach (var path in m.AttachmentPaths ?? new())
                    if (File.Exists(path)) msg.Attachments.Add(new Attachment(path));

                using var client = new SmtpClient(cfg.SMTPServer, cfg.SMTPPort)
                {
                    EnableSsl = string.Equals(cfg.SslTls, "tls", StringComparison.OrdinalIgnoreCase)
                             || string.Equals(cfg.SslTls, "ssl", StringComparison.OrdinalIgnoreCase),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Timeout = 30000
                };
                if (string.Equals(cfg.SMTPAuth, "yes", StringComparison.OrdinalIgnoreCase))
                    client.Credentials = new NetworkCredential(cfg.SMTPUsername, cfg.SMTPPassword);

                await client.SendMailAsync(msg, ct);
                return EmailSendResult.Ok();
            }
            catch (Exception ex)
            {
                var error = ex.InnerException?.Message ?? ex.Message;
                _log.LogWarning("Email to {To} (company {Company}) failed: {Error}", m.To, m.CompanyId, error);
                return EmailSendResult.Fail(error);
            }
        }
    }
}
