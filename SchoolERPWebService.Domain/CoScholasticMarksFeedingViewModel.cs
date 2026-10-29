using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class CoScholasticMarksFeedingViewModel
    {
        public int Result { get; set; }

        public string? Message { get; set; }
        public int CoScholasticMarksFeedingID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int CoScholasticID { get; set; }
        public int ExamID { get; set; }
        public int StudentID { get; set; }
        public string? Grade { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
    public class CoScholasticMarksFeedingRowRequest
    {
        public int StudentID { get; set; }
        public int? ExamGradeID { get; set; }
    }

    public class CoScholasticMarksFeedingRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int CoScholasticID { get; set; }
        public int ExamID { get; set; }

        public List<CoScholasticMarksFeedingRowRequest> Rows { get; set; }
            = new List<CoScholasticMarksFeedingRowRequest>();
        public int UserId { get; set; }
        public string? IPAddress { get; set; }
    }

    public class CoScholasticMarksFeedingGetRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int CoScholasticID { get; set; }
        public int ExamID { get; set; }
    }

    public class CoScholasticMarksFeedingResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int CoScholasticMarksFeedingID { get; set; }
        public int StudentID { get; set; }
        public string RollNo { get; set; }
        public string? SRCode { get; set; }
        public string? FatherName { get; set; }
        public string? StudentName { get; set; }
        public string? ExamGradeID { get; set; }
    }

    public class CoScholasticMarksFeedingAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstClassViewModel> Class { get; set; } = new();
        public List<MstSectionViewModel> Section { get; set; } = new();
    }
}
