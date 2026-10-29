using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class AttendanceFeedingViewModel
    {
        public int Result { get; set; }

        public string? Message { get; set; }

        public int AttendanceFeedingID { get; set; }

        public int StudentID { get; set; }

        public string? RollNo { get; set; }

        public string? FatherName { get; set; }

        public string? StudentName { get; set; }

        public int? Attendance { get; set; }

        public decimal? SchoolDays { get; set; }
    }

    public class AttendanceFeedingRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int ExamID { get; set; }
    }

    public class AttendanceFeedingRowRequest
    {
        public int StudentID { get; set; }

        public decimal? Attendance { get; set; }

        public decimal? SchoolDays { get; set; }
    }

    public class AttendanceFeedingAddRequest
    {
        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int ClassID { get; set; }

        public int SectionID { get; set; }

        public int ExamID { get; set; }

        public List<AttendanceFeedingRowRequest> Rows { get; set; }
            = new List<AttendanceFeedingRowRequest>();

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }
}
