using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ExamConfigViewModel
    {
        public int ExamConfigID { get; set; }

        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int ClassID { get; set; }

        public int ExamGroupID { get; set; }

        public int ExamID { get; set; }

        public string? MargeExam { get; set; }

        public decimal Percentage { get; set; }

        public int Sequence { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ExamName { get; set; }
        public string ExamGroupName { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ExamConfigRequest
    {
        public int ExamConfigID { get; set; }

        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int ClassID { get; set; }

        public int ExamGroupID { get; set; }

        public int ExamID { get; set; }

        public string? MargeExam { get; set; }

        public decimal Percentage { get; set; }

        public int Sequence { get; set; }

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class ExamConfigCopyRequest
    {
        public int CompanyID { get; set; }

        public int FromSessionID { get; set; }

        public int FromClassID { get; set; }

        public int ToSessionID { get; set; }

        // Comma-separated Class IDs
        // Example: "5,7,2,1"
        public string ToClassIDs { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class ExamConfigListRequest
    {
        public int CompanyID { get; set; }

        public int? SessionID { get; set; }

        public int? ClassID { get; set; }

        public string? SearchText { get; set; }

        public bool? IsActive { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string SortColumn { get; set; } = "SEQUENCE";

        public string SortDirection { get; set; } = "ASC";

        public int TotalRecords { get; set; }
    }

    public class SubjectWithPaperMarksResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? SubjectType { get; set; }

        public string? SubjectCode { get; set; }

        public int SubjectGroupID { get; set; }

        public string? SubjectGroupName { get; set; }

        public int PaperMarksDetailID { get; set; }

        public decimal TheoryMaxMark { get; set; }

        public decimal TheoryMinMark { get; set; }

        public decimal PracticalMaxMark { get; set; }

        public decimal PracticalMinMark { get; set; }

        public decimal TotalMaxMark { get; set; }

        public decimal TotalMinMark { get; set; }
    }
}
