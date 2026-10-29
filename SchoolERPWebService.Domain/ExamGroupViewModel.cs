using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ExamGroupViewModel
    {
        public int ExamGroupId { get; set; }
        public string? ExamGroupName { get; set; }
        public string? ExamGroupShortCode { get; set; }
        public string? DisplayLabel { get; set; }
        public string? Description { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ExamGroupRequestModel
    {
        public int ExamGroupID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string ExamGroupName { get; set; } = string.Empty;
        public string ExamGroupShortCode { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

}
