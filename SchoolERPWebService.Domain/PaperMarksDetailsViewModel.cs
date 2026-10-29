using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class PaperMarksDetailsViewModel
    {
        public int PaperMarksDetailID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int ExamID { get; set; }
        public int SubjectID { get; set; }
        public decimal? TheoryMaxMark { get; set; }
        public decimal? TheoryMinMark { get; set; }
        public decimal? PracticalMaxMark { get; set; }
        public decimal? PracticalMinMark { get; set; }
        public decimal? TotalMaxMark { get; set; }
        public decimal? TotalMinMark { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class PaperMarksDetailRowRequest
    {
        public int SubjectID { get; set; }
        public decimal TheoryMaxMark { get; set; }
        public decimal TheoryMinMark { get; set; }
        public decimal PracticalMaxMark { get; set; }
        public decimal PracticalMinMark { get; set; }
    }
    public class PaperMarksDetailsSaveRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int ExamID { get; set; }

        public List<PaperMarksDetailRowRequest> Rows { get; set; }
            = new List<PaperMarksDetailRowRequest>();

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class PaperMarksDetailsCopyRequest
    {
        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int SourceClassID { get; set; }

        public int SourceSectionID { get; set; }

        public int SourceExamID { get; set; }

        public int TargetClassID { get; set; }

        public int TargetSectionID { get; set; }

        public int TargetExamID { get; set; }

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }
}
