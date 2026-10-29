using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class MediaViewModel
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int MediaID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class MediaRequest 
    {
        public int MediaID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int CreatedBy { get; set; }
        public string? MediaJson { get; set; }
        // NEW: client sends back the attachments it wants to keep (edit mode)
        public string? ExistingAttachmentsJson { get; set; }
    }

    public class MediaAddViewModel 
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MediaViewModel Homeworks { get; set; } = new MediaViewModel();
    }
    public class AttachmentDto
    {
        public string Attachment { get; set; } = "";
        public string FileName { get; set; } = "";
        public string FileType { get; set; } = "";
    }

    public class MediaPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MediaViewModel> Media { get; set; } = new List<MediaViewModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }
}
