using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class PromotedClassViewModel
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public int PromotionID { get; set; }
        public int StudentID { get; set; }
        public string RollNo { get; set; }
        public string? FatherName { get; set; }
        public string? StudentName { get; set; }
        public int? PromotedToClassID { get; set; }
        public int? PromotedToSectionID { get; set; }
        public int? TargetSessionID { get; set; }
        public string? ResultStatus { get; set; }
    }
    public class PromotedToClassRowRequest
    {
        public int StudentID { get; set; }

        public int? PromotedToClassID { get; set; }

        public int? PromotedToSectionID { get; set; }

        public string? Result { get; set; }
    }
    public class PromotedToClassRequest
    {
        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int ClassID { get; set; }

        public int SectionID { get; set; }

        public int? TargetSessionID { get; set; }

        public List<PromotedToClassRowRequest> Rows { get; set; }
            = new List<PromotedToClassRowRequest>();

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class PromotedToClassGetAllRequest
    {
        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int ClassID { get; set; }

        public int SectionID { get; set; }
    }
}
