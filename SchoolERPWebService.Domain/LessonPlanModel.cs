using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class LessonPlanModel
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int LessonId { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
        public string? LessonName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public string? ClassName { get; set; }
        public string? SubjectGroupName { get; set; } 
        public string? SubjectName { get; set; } 
        public string? SectionName { get; set; }
        public string? CreatedByName { get; set; }
        public string? ChaptersJson{ get; set; }
        public string? CLASSTEACHERNAME { get; set; }
       
    }

    public class LessonPlanModelRequest 
    {

        public int LessonId { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
        public string? LessonJson { get; set; } = "[]";
        public bool? IsActive { get; set; }
    }

    public class LessonDropDwonReq 
    {
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
    }

    public class LessonDropDwonResponse 
    {
        public int LessonId { get; set; }
        public int LessonMapId { get; set; }
        public string? LessonName { get; set; }
    }

    public class LessonPlanMap 
    {
        public int LessonId { get; set; }
        public int LessonMapId { get; set; }
        public string? LessonName { get; set; }
        public string? Description { get; set; }
    }

    public class LessonPlanSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }

        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
    }

    public class LessonPlanPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<LessonPlanModel> LessonPlan { get; set; } = new List<LessonPlanModel>();
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
    }

    public class LessonPlanAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public LessonPlanModel EditLessonPlan { get; set; } = new LessonPlanModel();
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public List<LessonPlanMap> LessonPlanMap { get; set; } = new List<LessonPlanMap>();

    }

    public class LessonChapterDto
    {
        public int LessonMapId { get; set; }
        public string? LessonName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
