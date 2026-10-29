using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    // Posted by the browser (sendBulkMail in BulkMailList.js)
    public class BulkMailSendRequest
    {
        public bool SelectAll { get; set; }
        public List<int> StudentIds { get; set; } = new();
        public List<int> ExcludedIds { get; set; } = new();
        public string? Search { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public string MessageTo { get; set; } = "";     // 1 = Student, 2 = Parent, 3 = Both
        public int EmailTemplateId { get; set; }
        public int CompanyId { get; set; }
    }

    // Returned by usp_GetBulkMailRecipients
    public class BulkMailRecipientRow
    {
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }
        public string? ParentEmail { get; set; }
        public string? StudentUsername { get; set; }
        public string? ParentUsername { get; set; }
        public string? StudentPassword { get; set; }
        public string? ParentPassword { get; set; }
        public string? AdmissionNo { get; set; }
        public string? FatherName { get; set; }
    }

    // Returned by usp_GetEmailTemplateForSend
    public class BulkMailTemplateInfo
    {
        public int EmailTemplateId { get; set; }
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Attachment { get; set; }
        public string? AttachmentName { get; set; }
    }

    // Returned to the browser after queuing
    public class BulkMailResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Guid? BatchId { get; set; }
        public int Queued { get; set; }
        public int Skipped { get; set; }

        public static BulkMailResult Fail(string message) => new() { Success = false, Message = message };
    }

    // Returned by usp_EmailQueue_BatchStatus
    public class BulkMailBatchStatus
    {
        public int Total { get; set; }
        public int Sent { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }
    }
}
