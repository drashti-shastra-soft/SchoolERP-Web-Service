using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    internal class ChangeExamViewModel
    {
    }
    public class ExamMappingRequest
    {
        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int MarkTypeID { get; set; }

        public int ClassID { get; set; }

        public int SectionID { get; set; }

        public int OldExamID { get; set; }

        public int NewExamID { get; set; }
        public string? StudentIDsJson { get; set; }
        public int? UserID { get; set; }
    }
    public class StudentResponse
    {
        public string? RollNo { get; set; }

        public int StudentID { get; set; }

        public string? StudentName { get; set; }

        public string? FatherName { get; set; }

        public int RecordCount { get; set; }
    }

    public class ChangeExamRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int MarkTypeID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int ExamID { get; set; }
    }
}
