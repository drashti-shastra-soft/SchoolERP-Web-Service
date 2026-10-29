using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ManageLessonPlanViewModel
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int LessonPlanId { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectGroupID { get; set; }
        public int SubjectID { get; set; }
        public int LessonMapId { get; set; }
        public int TopicMapId { get; set; }
        public string? SubTopic { get; set; }
        public DateTime PlanDate { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public string? LectureYoutubeUrl { get; set; }
        public string? LectureVideoPath { get; set; }
        public string? AttachmentPath { get; set; }
        public string? TeachingMethod { get; set; }
        public string? GeneralObjectives { get; set; }
        public string? PreviousKnowledge { get; set; }
        public string? ComprehensiveQuestions { get; set; }
        public string? Presentation { get; set; }
        public int TimeTableID { get; set; }
        public byte Status { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    public class ManageLessonPlanRequest 
    {
        public int LessonPlanId { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectGroupID { get; set; }
        public int SubjectID { get; set; }
        public int LessonMapId { get; set; }
        public int TopicMapId { get; set; }
        public int TimeTableID { get; set; }

        public string? SubTopic { get; set; }

        public DateTime PlanDate { get; set; }
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }

        public string? LectureYoutubeUrl { get; set; }
        public string? LectureVideoPath { get; set; }
        public string? AttachmentPath { get; set; }

        public bool RemoveExistingVideo { get; set; }        // explicit "user cleared the video" flag
        public string? RemovedAttachmentIds { get; set; }     // JSON array of ids to soft-delete

        public string? TeachingMethod { get; set; }
        public string? GeneralObjectives { get; set; }
        public string? PreviousKnowledge { get; set; }
        public string? ComprehensiveQuestions { get; set; }
        public string? Presentation { get; set; }

        public byte Status { get; set; }

        public bool IsActive { get; set; }
        public string? IPAddress { get; set; }
    }

    public class ManageLessonPlanSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }

        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectID { get; set; }

        public bool IncludeDeleted { get; set; } = false;

        public string? SearchKeyword { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int? UserID { get; set; }
        public string? IPAddress { get; set; }

    }

    public class ManageLessonPlanAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public ManageLessonPlanViewModel EditManageLessonPlan { get; set; } = new ManageLessonPlanViewModel();
        public TimeTableViewModel TimeTable { get; set; } = new TimeTableViewModel();
        public List<LessonDropDwonResponse> BindLesson { get; set; } = new List<LessonDropDwonResponse>();
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public List<LessonPlanMap> LessonPlanMap { get; set; } = new List<LessonPlanMap>();
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectGroupID { get; set; }
        public int SubjectID { get; set; }
        public int? TimeTableID { get; set; }
    }

    public class ManageLessonPlanAttachmentUpsertRequest
    {
        public int LessonPlanId { get; set; }

        public int CompanyID { get; set; }

        public string? LectureVideoPath { get; set; }

        public string? AttachmentPath { get; set; }

        public int UserID { get; set; }

        public string? IPAddress { get; set; }
    }

    public class LessonPlanViewModel
    {
        public int LessonPlanId { get; set; }

        public string? ClassName { get; set; }

        public string? SectionName { get; set; }

        public string? SubjectName { get; set; }

        public string? SubjectCode { get; set; }

        public DateTime PlanDate { get; set; }

        public TimeSpan? TimeFrom { get; set; }

        public TimeSpan? TimeTo { get; set; }

        public string? LessonName { get; set; }

        public string? TopicName { get; set; }

        public string? SubTopic { get; set; }

        public string? GeneralObjectives { get; set; }

        public string? TeachingMethod { get; set; }

        public string? PreviousKnowledge { get; set; }

        public string? ComprehensiveQuestions { get; set; }

        public string? Presentation { get; set; }

        public string? LectureYoutubeUrl { get; set; }

        public string? LectureVideoPath { get; set; }

        public string? AttachmentPath { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsActive { get; set; }
    }

    public class ManageLessonDetailsPlanViewModel
    {
        public int? HomeworkID { get; set; }
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public LessonPlanViewModel LessonPlanDetails { get; set; } = new LessonPlanViewModel();
        public List<LessonPlanCommentResponse> Comments { get; set; } = new List<LessonPlanCommentResponse>();
    }

    public class LessonPlanCommentRequest
    {
        public int LessonPlanId { get; set; }

        public string CommentText { get; set; } = string.Empty;

        /// <summary>
        /// UserId or StudentId depending on CommenterType.
        /// </summary>
        public int? CommentedBy { get; set; }

        /// <summary>
        /// STAFF or STUDENT
        /// </summary>
        public string CommenterType { get; set; } = string.Empty;

        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        /// <summary>
        /// Logged-in user ID (used for audit).
        /// </summary>
        public int UserID { get; set; }

        public string? IPAddress { get; set; }
    }

    public class LessonPlanCommentResponse
    {
        public int CommentId { get; set; }

        public int LessonPlanId { get; set; }

        public string? CommentText { get; set; }

        public int CommentedBy { get; set; }

        public string? CommenterType { get; set; }

        public DateTime CommentedOn { get; set; }

        public string? CommenterName { get; set; }

        public string? CommenterImage { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
