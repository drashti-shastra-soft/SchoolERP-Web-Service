using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ExamGradeViewModel
    {
        public int ExamGradeID { get; set; }

        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int SubjectTypeID { get; set; }

        public int ClassID { get; set; }

        public int SortOrder { get; set; }

        public decimal MinRange { get; set; }

        public decimal MaxRange { get; set; }

        public string? ExamGradeName { get; set; }

        public decimal? GradePointer { get; set; }

        public string? Remark { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
    }

    public class ExamGradeRequest
    {
        public int ExamGradeID { get; set; }
        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int SubjectTypeID { get; set; }

        public int ClassID { get; set; }

        public List<ExamGradeRowRequest> GradeRows { get; set; } = new();

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class ExamGradeRowRequest
    {
        public decimal MinRange { get; set; }

        public decimal MaxRange { get; set; }

        public string ExamGrade { get; set; } = string.Empty;

        public decimal GradePointer { get; set; }

        public string? Remark { get; set; }
    }

    public class ExamGradeResponse
    {
        public int Result { get; set; }

        public string? Message { get; set; }

        public string? TechnicalMessage { get; set; }

        public int? ErrorLine { get; set; }
    }

    public class ExamGradeCopyRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int FromSubjectTypeID { get; set; }
        public int FromClassID { get; set; }
        public int ToSubjectTypeID { get; set; }
        public int ToClassID { get; set; }
        public int UserId { get; set; }
        public string? IPAddress { get; set; }

    }
    public class ExamGradeCopyResponse
    {
        public int Result { get; set; }

        public string? Message { get; set; }

        public int Copied { get; set; }

        public int Skipped { get; set; }
    }
}
