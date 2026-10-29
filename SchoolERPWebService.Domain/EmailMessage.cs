using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class EmailMessage
    {
        public int CompanyId { get; set; }
        public string To { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Body { get; set; } = "";
        public bool IsHtml { get; set; } = true;
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public List<string>? AttachmentPaths { get; set; }
    }

    public class EmailSendResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public static EmailSendResult Ok() => new() { Success = true };
        public static EmailSendResult Fail(string e) => new() { Success = false, Error = e };
    }
}
