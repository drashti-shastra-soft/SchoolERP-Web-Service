using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ExamViewModel
    {
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public string? ExamShortCode { get; set; }
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

    public class ExamRequestModel
    {
        public int ExamId { get; set; }
        public string? ExamName { get; set; }
        public string? ExamShortCode { get; set; }
        public string? DisplayLabel { get; set; }
        public string? Description { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
        public string? IPAddress { get; set; }
    }

    public class ExamListRequest
    {
        public int CompanyID { get; set; }
        public int? SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string SortColumn { get; set; } = "ExamName";

        public string SortDirection { get; set; } = "ASC";
    }
}
