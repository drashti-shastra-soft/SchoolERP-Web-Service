using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class RemarksFeedingViewModel
    {
        public int Result { get; set; }

        public string? Message { get; set; }

        public int RemarksFeedingID { get; set; }

        public int StudentID { get; set; }

        public string? RollNo { get; set; }

        public string? FatherName { get; set; }

        public string? StudentName { get; set; }

        public string? Remark { get; set; }
    }

    public class RemarksFeedingRowRequest
    {
        public int StudentID { get; set; }

        public string? Remark { get; set; }
    }

    public class RemarksFeedingRequest
    {
        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int ClassID { get; set; }

        public int SectionID { get; set; }

        public int ExamID { get; set; }

        public List<RemarksFeedingRowRequest> Rows { get; set; }
            = new List<RemarksFeedingRowRequest>();

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }
}
