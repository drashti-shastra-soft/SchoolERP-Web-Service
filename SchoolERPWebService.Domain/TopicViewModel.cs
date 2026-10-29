using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class TopicViewModel
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int TopicId { get; set; }
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
        public string? TopicNamesJson { get; set; }
        public string? CLASSTEACHERNAME { get; set; }
    }

    public class TopicModelRequest
    {

        public int TopicId { get; set; }
        public int LessonId { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
        public string? TopicJson { get; set; } = "[]";
        public bool? IsActive { get; set; }
    }

    public class TopicMap
    {
        public int TopicId { get; set; }
        public int TopicMapId { get; set; }
        public string? TopicName { get; set; }
        public string? Description { get; set; }
    }

    public class TopicSearchRequest
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
        public int? LessonID { get; set; }
    }

    public class TopicPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<TopicViewModel> Topic { get; set; } = new List<TopicViewModel>();
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

    public class TopicAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public TopicViewModel EditTopic { get; set; } = new TopicViewModel();
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public List<TopicMap> TopicMap { get; set; } = new List<TopicMap>();

    }

    public class TopicChapterDto
    {
        public int TopicMapId { get; set; }
        public string? TopicName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
    public class TopicResponseModel
    {
        public string? Ref { get; set; }
        public int TopicMapId { get; set; }
        public string? TopicName { get; set; }
        public DateTime? TopicCompletionDate { get; set; }
        public bool IsCompleted { get; set; }
    }
    public class LessonSyllabusStatusResponse
    {
        public int? SNo { get; set; }
        public int? LessonMapId { get; set; }
        public string? LessonName { get; set; }

        /// <summary>
        /// Raw JSON returned by SQL FOR JSON PATH
        /// </summary>
        public string? TopicsJson { get; set; }

        /// <summary>
        /// Optional: Deserialize TopicsJson into this property.
        /// </summary>
        public List<TopicResponseModel>? Topics { get; set; }

        public int Result { get; set; }
        public string? Message { get; set; }

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class LessonSyllabusStatusPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<LessonSyllabusStatusResponse> LessonSyllabusStatus { get; set; } = new List<LessonSyllabusStatusResponse>();
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

    public class TopicDropDwonResponse
    {
        public int TopicId { get; set; }
        public int TopicMapId { get; set; }
        public string? TopicName { get; set; }
    }
}
