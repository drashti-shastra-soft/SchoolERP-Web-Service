using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    // ============================================================
    // REQUEST : SAVE (CREATE / UPDATE)  ->  SP_NoticeBoard_Upsert
    // NoticeBoardId = 0 -> insert, > 0 -> update
    // ============================================================
    public class NoticeBoardRequestModel
    {
        public int NoticeBoardId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? NoticeDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string NoticeMessage { get; set; } = string.Empty;   // editor html
        public string? Attachment { get; set; }                     // saved file path / url
        public string? AttachmentName { get; set; }
        public string? AttachmentType { get; set; }
        public List<int> RoleIds { get; set; } = new();             // "Message To" checkboxes
        public bool IsActive { get; set; } = true;
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    // ============================================================
    // REQUEST : PAGED LIST  ->  SP_NoticeBoard_GetAllPageIndex
    // IsSuperAdmin = true  -> all notices
    // IsSuperAdmin = false -> only notices sent to RoleId
    // ============================================================
    public class NoticeBoardSearchRequest
    {
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public int? RoleId { get; set; }                // logged-in user's role
        public bool IsSuperAdmin { get; set; }
        public DateTime? FromDate { get; set; }         // notice date from
        public DateTime? ToDate { get; set; }           // notice date to
        public bool? IsActive { get; set; }             // super admin only
        public int? FilterRoleId { get; set; }          // super admin only
        public bool IncludeDeleted { get; set; }        // super admin only
        public string? SearchKeyword { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int UserID { get; set; }
    }

    // ============================================================
    // RESPONSE : SAVE
    // ============================================================
    public class NoticeBoardSaveResponse : ApiResponse
    {
        public int NoticeBoardId { get; set; }
    }

    // ============================================================
    // RESPONSE : ONE ROW (GetById + each row of the paged list)
    // Dapper maps column names case-insensitively.
    //   Message       = SP status text (SUCCESS / error)
    //   NoticeMessage = the actual notice body
    // ============================================================
    public class NoticeBoardModel
    {
        // standard SP result columns
        public int Result { get; set; }
        public string? Message { get; set; }
        public string? TechnicalMessage { get; set; }
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        // notice columns
        public int NoticeBoardId { get; set; }
        public string? Title { get; set; }
        public DateTime? NoticeDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? NoticeMessage { get; set; }
        public string? Attachment { get; set; }
        public string? AttachmentName { get; set; }
        public string? AttachmentType { get; set; }
        public string? RoleIds { get; set; }            // "1,2,8"
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CompanyID { get; set; }
        public int SessionId { get; set; }
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        // convenience for the UI (tick the "Message To" checkboxes)
        public List<int> RoleIdList =>
            string.IsNullOrWhiteSpace(RoleIds)
                ? new List<int>()
                : RoleIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                         .Select(x => int.TryParse(x, out var id) ? id : 0)
                         .Where(x => x > 0)
                         .ToList();

        public string RoleNames { get; set; }
    }

    public class NoticeBoardAttachmentRequest
    {
        public int NoticeBoardID { get; set; }
        public string? Attachment { get; set; }            // stored path
        public string? AttachmentType { get; set; }        // content type / extension
        public string? AttachmentName { get; set; }        // original file name
        public int UserId { get; set; }
        public string? IPAddress { get; set; }
    }

}
