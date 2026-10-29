using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Utilities;
using SchoolERPWebService.Domain;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net;
using Dapper;

namespace SchoolERPWebService.Services
{
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailSenderService _emailSender;
        private readonly ILogger<ForgotPasswordService> _log;

        public ForgotPasswordService(IConfiguration configuration, IEmailSenderService emailSender, ILogger<ForgotPasswordService> log)
        {
            _configuration = configuration;
            _emailSender = emailSender;
            _log = log;
        }

        private SqlConnection GetConn() =>  new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public async Task<(bool Success, string Message)> GenerateTokenAsync(string email, int companyId, int sessionId, string baseUrl)
        {
            try
            {
                using var conn = GetConn();
                var result = await conn.QueryFirstOrDefaultAsync<ForgotPasswordResult>(
                    "SP_FORGOT_PASSWORD_GENERATE",
                    new { EMAIL = email, COMPANYID = companyId, SESSIONID = sessionId },
                    commandType: CommandType.StoredProcedure);

                if (result == null || result.Result == 0)
                    return (false, result?.Message ?? "Failed to generate reset link.");

                // Build reset link
                var webBase = _configuration["ApiSettings:BaseUrl"];
                if (string.IsNullOrWhiteSpace(webBase))
                    return (false, "Web base URL is not configured.");

                var resetLink = $"{webBase.TrimEnd('/')}/Auth/ResetPassword?guid={result.Guid}";

                // Send through the shared email service (company SMTP config is loaded inside)
                var sendResult = await _emailSender.SendAsync(new EmailMessage
                {
                    CompanyId = companyId,
                    To = email,
                    Subject = "Password Reset Request",
                    Body = BuildResetEmailHtml(resetLink),
                    IsHtml = true
                });

                if (!sendResult.Success)
                {
                    _log.LogWarning("Password reset mail failed for company {Company}: {Error}",
                        companyId, sendResult.Error);
                    return (false, "Failed to send email. Please check email configuration.");
                }

                return (true, "Password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "GenerateTokenAsync failed");
                return (false, "Something went wrong. Please try again.");
            }
        }

        private static string BuildResetEmailHtml(string resetLink)
        {
            var link = WebUtility.HtmlEncode(resetLink);

            return $@"
        <!DOCTYPE html>
        <html>
        <head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1""></head>
        <body style=""margin:0;padding:0;background:#f4f6f8;font-family:Arial,Helvetica,sans-serif;"">
          <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f4f6f8;padding:24px 0;"">
            <tr><td align=""center"">
              <table role=""presentation"" width=""560"" cellpadding=""0"" cellspacing=""0""
                     style=""max-width:560px;width:100%;background:#ffffff;border-radius:8px;overflow:hidden;border:1px solid #e3e7eb;"">

                <tr><td style=""background:#2563eb;padding:20px 28px;color:#ffffff;font-size:20px;font-weight:bold;"">
                  School ERP
                </td></tr>

                <tr><td style=""padding:28px;color:#333333;font-size:15px;line-height:1.6;"">
                  <h2 style=""margin:0 0 16px;color:#2563eb;font-size:20px;"">Password Reset Request</h2>
                  <p style=""margin:0 0 12px;"">Hello,</p>
                  <p style=""margin:0 0 20px;"">We received a request to reset your password. Click the button below to choose a new one.</p>

                  <p style=""margin:24px 0;text-align:center;"">
                    <a href=""{link}""
                       style=""background:#2563eb;color:#ffffff;text-decoration:none;padding:12px 28px;border-radius:6px;font-weight:bold;display:inline-block;"">
                      Reset Password
                    </a>
                  </p>

                  <p style=""margin:0 0 12px;"">This link will expire in <b>24 hours</b>.</p>
                  <p style=""margin:0 0 16px;font-size:13px;color:#777777;"">
                    Button not working? Copy and paste this link into your browser:<br/>
                    <a href=""{link}"" style=""color:#2563eb;word-break:break-all;"">{link}</a>
                  </p>
                  <p style=""margin:0;font-size:13px;color:#777777;"">
                    If you did not request this, you can safely ignore this email. Your password will not change.
                  </p>
                </td></tr>

                <tr><td style=""background:#f8f9fb;padding:16px 28px;font-size:12px;color:#888888;text-align:center;border-top:1px solid #e3e7eb;"">
                  This is an automated email, please do not reply.<br/>School ERP System
                </td></tr>

              </table>
            </td></tr>
          </table>
        </body>
        </html>";
        }

        public ValidateTokenResult ValidateToken(string guid)
        {
            using var conn = GetConn();
            return conn.QueryFirstOrDefault<ValidateTokenResult>(
                "SP_FORGOT_PASSWORD_VALIDATE",     
                new { GUID = guid },
                commandType: CommandType.StoredProcedure);
        }

        public (bool Success, string Message) ResetPassword(string guid, string newPassword)
        {
            try
            {
                var (hash, salt) = SecurityHelper.HashPassword(newPassword);

                using var conn = GetConn();
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_FORGOT_PASSWORD_RESET",
                    new { GUID = guid, PASSWORDHASH = hash, PASSWORDSALT = salt },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message ?? "Failed to reset password.");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "ResetPassword failed");
                return (false, "Unable to reset password. Please try again.");
            }
        }
    }
}