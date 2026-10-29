using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.Net.Mail;
using System.Net;
using Dapper;

namespace SchoolERPWebService.Services
{
    public class BulkMailService: IBulkMailService
    {
        private readonly string _cs;
        private readonly IWebHostEnvironment _env;
        private readonly ICompanyService _companyService;
        public BulkMailService(IConfiguration cfg, IWebHostEnvironment env, ICompanyService companyService)
        {
            _cs = cfg.GetConnectionString("DefaultConnection")!;
            _env = env;
            _companyService = companyService;
        }

        // -----------------------------------------------------------------
        // Queue emails for the selected students
        // -----------------------------------------------------------------
        public async Task<BulkMailResult> QueueBulkMailAsyncold(BulkMailSendRequest req, int companyId)
        {
            if (req == null || req.EmailTemplateId <= 0 || string.IsNullOrWhiteSpace(req.MessageTo))
                return BulkMailResult.Fail("Message To and Notification Type are required.");
            if (companyId <= 0)
                return BulkMailResult.Fail("Company could not be determined.");

            using var con = new SqlConnection(_cs);

            // 1) student ids (all pages when SelectAll)
            List<int> ids;
            if (req.SelectAll)
            {
                ids = (await con.QueryAsync<int>(
                    "dbo.usp_GetFilteredStudentIds",
                    new { CompanyID = companyId, Search = req.Search, ClassID = req.ClassId, SectionID = req.SectionId },
                    commandType: CommandType.StoredProcedure)).ToList();

                ids = ids.Except(req.ExcludedIds).ToList();
            }
            else
            {
                ids = req.StudentIds.Distinct().ToList();
            }

            if (ids.Count == 0)
                return BulkMailResult.Fail("No students selected.");

            // 2) template
            var template = await con.QueryFirstOrDefaultAsync<BulkMailTemplateInfo>(
                "dbo.usp_GetEmailTemplateForSend",
                new { CompanyID = companyId, EmailTemplateId = req.EmailTemplateId },
                commandType: CommandType.StoredProcedure);

            if (template == null)
                return BulkMailResult.Fail("Email template not found or inactive.");

            var attachmentPath = ResolveAttachmentPath(template.Attachment);

            // 3) recipients + credentials
            var students = (await con.QueryAsync<BulkMailRecipientRow>(
                "dbo.usp_GetBulkMailRecipients",
                new { CompanyID = companyId, IdsCsv = string.Join(",", ids) },
                commandType: CommandType.StoredProcedure)).ToList();

            // 4) build queue rows (column order must match dbo.EmailQueueRowType)
            var batchId = Guid.NewGuid();
            var table = new DataTable();
            table.Columns.Add("BatchID", typeof(Guid));
            table.Columns.Add("CompanyID", typeof(int));
            table.Columns.Add("StudentID", typeof(int));
            table.Columns.Add("ToEmail", typeof(string));
            table.Columns.Add("Subject", typeof(string));
            table.Columns.Add("Body", typeof(string));
            table.Columns.Add("AttachmentPath", typeof(string));

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);   // email|username
            int skipped = 0;

            bool toStudent = req.MessageTo == "1" || req.MessageTo == "3";
            bool toParent = req.MessageTo == "2" || req.MessageTo == "3";

            void Add(BulkMailRecipientRow s, string? rawEmail, string? username, string? password)
            {
                var email = rawEmail?.Trim();
                if (string.IsNullOrEmpty(email) || !MailAddress.TryCreate(email, out _)) return;
                if (!seen.Add($"{email}|{username}")) return;

                table.Rows.Add(
                    batchId,
                    companyId,
                    s.StudentID,
                    email,
                    "Student Admission",
                    FillTemplate(template.Message, s, username ?? "", password ?? ""),
                    (object?)attachmentPath ?? DBNull.Value);
            }

            foreach (var s in students)
            {
                int before = table.Rows.Count;

                if (toStudent) Add(s, s.StudentEmail, s.StudentUsername, s.StudentPassword);   // student mail -> student login
                if (toParent) Add(s, s.ParentEmail, s.ParentUsername, s.ParentPassword);       // parent mail  -> parent login

                if (table.Rows.Count == before) skipped++;
            }

            if (table.Rows.Count == 0)
                return BulkMailResult.Fail("No valid email addresses found for the selected students.");

            // 5) insert into the queue (the background worker sends them)
            await con.ExecuteAsync(
                "dbo.usp_EmailQueue_InsertBatch",
                new { Rows = table.AsTableValuedParameter("dbo.EmailQueueRowType") },
                commandType: CommandType.StoredProcedure);

            return new BulkMailResult
            {
                Success = true,
                BatchId = batchId,
                Queued = table.Rows.Count,
                Skipped = skipped,
                Message = $"{table.Rows.Count} email(s) queued for sending. {skipped} student(s) skipped (no valid email)."
            };
        }


        public async Task<BulkMailResult> QueueBulkMailAsync(BulkMailSendRequest req, int companyId)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.MessageTo))
                return BulkMailResult.Fail("Message To is required.");
            if (companyId <= 0)
                return BulkMailResult.Fail("Company could not be determined.");

            using var con = new SqlConnection(_cs);

            // 1) student ids (all pages when SelectAll)
            List<int> ids;
            if (req.SelectAll)
            {
                ids = (await con.QueryAsync<int>(
                    "dbo.usp_GetFilteredStudentIds",
                    new { CompanyID = companyId, Search = req.Search, ClassID = req.ClassId, SectionID = req.SectionId },
                    commandType: CommandType.StoredProcedure)).ToList();

                ids = ids.Except(req.ExcludedIds).ToList();
            }
            else
            {
                ids = req.StudentIds.Distinct().ToList();
            }

            if (ids.Count == 0)
                return BulkMailResult.Fail("No students selected.");

            // 2) recipients + credentials
            var students = (await con.QueryAsync<BulkMailRecipientRow>(
                "dbo.usp_GetBulkMailRecipients",
                new { CompanyID = companyId, IdsCsv = string.Join(",", ids) },
                commandType: CommandType.StoredProcedure)).ToList();

            // 3) build queue rows (column order must match dbo.EmailQueueRowType)
            var batchId = Guid.NewGuid();
            var table = new DataTable();
            table.Columns.Add("BatchID", typeof(Guid));
            table.Columns.Add("CompanyID", typeof(int));
            table.Columns.Add("StudentID", typeof(int));
            table.Columns.Add("ToEmail", typeof(string));
            table.Columns.Add("Subject", typeof(string));
            table.Columns.Add("Body", typeof(string));
            table.Columns.Add("AttachmentPath", typeof(string));

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int skipped = 0;

            bool toStudent = req.MessageTo == "1" || req.MessageTo == "3";
            bool toParent = req.MessageTo == "2" || req.MessageTo == "3";

            string subject = req.EmailTemplateId == 1 ? "Student Admission" : req.EmailTemplateId == 2 ? "Student Login Details" : "Student Credentials";

            const string loginUrl = "https://schoolerp-web-console.quantifyrellp.space/";
            
            void Add(BulkMailRecipientRow s, string? rawEmail, string? recipientName,
                     string? username, string? password)
            {
                var email = rawEmail?.Trim();
                if (string.IsNullOrEmpty(email) || !MailAddress.TryCreate(email, out _)) return;
                if (!seen.Add($"{email}|{username}")) return;

                var company = _companyService.GetCompanyByID(companyId);
                string companyName = string.Empty;
                if(company != null) 
                {
                    companyName = company.SchoolName;
                }
                var body = BuildCredentialsEmailHtml(
                       companyName, recipientName ?? "", loginUrl,
                       username ?? "", password ?? "", s.AdmissionNo ?? "");

                table.Rows.Add(
                    batchId,
                    companyId,
                    s.StudentID,
                    email,
                    subject,
                    body,
                    DBNull.Value);   // no attachment (it came from the template before)
            }

            foreach (var s in students)
            {
                int before = table.Rows.Count;

                if (toStudent) Add(s, s.StudentEmail, s.StudentName, s.StudentUsername, s.StudentPassword);
                if (toParent) Add(s, s.ParentEmail, s.FatherName, s.ParentUsername, s.ParentPassword);

                if (table.Rows.Count == before) skipped++;
            }

            if (table.Rows.Count == 0)
                return BulkMailResult.Fail("No valid email addresses found for the selected students.");

            // 4) insert into the queue (the background worker sends them)
            await con.ExecuteAsync(
                "dbo.usp_EmailQueue_InsertBatch",
                new { Rows = table.AsTableValuedParameter("dbo.EmailQueueRowType") },
                commandType: CommandType.StoredProcedure);

            return new BulkMailResult
            {
                Success = true,
                BatchId = batchId,
                Queued = table.Rows.Count,
                Skipped = skipped,
                Message = $"{table.Rows.Count} email(s) queued for sending. {skipped} student(s) skipped (no valid email)."
            };
        }

        private static string BuildCredentialsEmailHtml(
    string schoolName, string recipientName, string loginUrl,
    string username, string password, string admissionNo)
        {
            string E(string? v) => WebUtility.HtmlEncode(v ?? "");   // using System.Net;

            return $@"
<!DOCTYPE html>
<html>
<head><meta charset=""utf-8""><meta name=""viewport"" content=""width=device-width, initial-scale=1""></head>
<body style=""margin:0;padding:0;background:#f4f6f8;font-family:Arial,Helvetica,sans-serif;"">
  <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#f4f6f8;padding:24px 0;"">
    <tr><td align=""center"">
      <table role=""presentation"" width=""560"" cellpadding=""0"" cellspacing=""0""
             style=""max-width:560px;width:100%;background:#ffffff;border-radius:8px;overflow:hidden;border:1px solid #e3e7eb;"">

        <!-- Header -->
        <tr><td style=""background:#1f4e9c;padding:20px 28px;color:#ffffff;font-size:20px;font-weight:bold;"">
          {E(schoolName)}
        </td></tr>

        <!-- Body -->
        <tr><td style=""padding:28px;color:#333333;font-size:15px;line-height:1.6;"">
          <p style=""margin:0 0 12px;"">Hello <b>{E(recipientName)}</b>,</p>
          <p style=""margin:0 0 20px;"">Your login details are given below.</p>

          <table role=""presentation"" width=""100%"" cellpadding=""0"" cellspacing=""0""
                 style=""border:1px solid #e3e7eb;border-radius:6px;font-size:14px;"">
            <tr>
              <td style=""padding:10px 14px;background:#f8f9fb;width:140px;border-bottom:1px solid #e3e7eb;""><b>Admission No</b></td>
              <td style=""padding:10px 14px;border-bottom:1px solid #e3e7eb;"">{E(admissionNo)}</td>
            </tr>
            <tr>
              <td style=""padding:10px 14px;background:#f8f9fb;border-bottom:1px solid #e3e7eb;""><b>Username</b></td>
              <td style=""padding:10px 14px;border-bottom:1px solid #e3e7eb;"">{E(username)}</td>
            </tr>
            <tr>
              <td style=""padding:10px 14px;background:#f8f9fb;""><b>Password</b></td>
              <td style=""padding:10px 14px;font-family:Consolas,monospace;"">{E(password)}</td>
            </tr>
          </table>

          <p style=""margin:24px 0;text-align:center;"">
            <a href=""{E(loginUrl)}""
               style=""background:#1f4e9c;color:#ffffff;text-decoration:none;padding:12px 28px;border-radius:6px;font-weight:bold;display:inline-block;"">
              Login Now
            </a>
          </p>

          <p style=""margin:0;font-size:13px;color:#777777;"">
            Or open this link: <a href=""{E(loginUrl)}"" style=""color:#1f4e9c;"">{E(loginUrl)}</a>
          </p>
        </td></tr>

        <!-- Footer -->
        <tr><td style=""background:#f8f9fb;padding:16px 28px;font-size:12px;color:#888888;text-align:center;border-top:1px solid #e3e7eb;"">
          Please keep your login details safe and do not share them with anyone.<br/>
          This is an automated email, please do not reply.
        </td></tr>

      </table>
    </td></tr>
  </table>
</body>
</html>";
        }

        // -----------------------------------------------------------------
        // Progress of one batch
        // -----------------------------------------------------------------
        public async Task<BulkMailBatchStatus> GetBatchStatusAsync(Guid batchId)
        {
            using var con = new SqlConnection(_cs);
            return await con.QuerySingleAsync<BulkMailBatchStatus>(
                "dbo.usp_EmailQueue_BatchStatus",
                new { BatchID = batchId },
                commandType: CommandType.StoredProcedure);
        }

        // -----------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------
        private static string Enc(string? v) => WebUtility.HtmlEncode(v ?? "");

        private static string FillTemplate(string html, BulkMailRecipientRow s, string username, string password)
        {
            return (html ?? "")
                .Replace("{StudentName}", Enc(s.StudentName))
                .Replace("{Username}", Enc(username))
                .Replace("{Password}", Enc(password))
                .Replace("{StudentUsername}", Enc(s.StudentUsername))
                .Replace("{StudentPassword}", Enc(s.StudentPassword))
                .Replace("{ParentUsername}", Enc(s.ParentUsername))
                .Replace("{ParentPassword}", Enc(s.ParentPassword));
        }

        // Template.Attachment is stored as a URL / relative path. The worker needs the physical file.
        // Returns null when the file is missing or outside wwwroot (email is then sent without attachment).
        // If the worker runs in a different project/server, make sure it can reach this path.
        private string? ResolveAttachmentPath(string? attachment)
        {
            if (string.IsNullOrWhiteSpace(attachment) || string.IsNullOrEmpty(_env.WebRootPath)) return null;

            var rel = attachment.Trim();
            if (Uri.TryCreate(rel, UriKind.Absolute, out var uri)) rel = uri.AbsolutePath;
            rel = Uri.UnescapeDataString(rel).TrimStart('~', '/', '\\');

            var root = Path.GetFullPath(_env.WebRootPath);
            var full = Path.GetFullPath(Path.Combine(root, rel));

            return full.StartsWith(root, StringComparison.OrdinalIgnoreCase) && File.Exists(full) ? full : null;
        }
    }
}
