using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class SyllabusStatusSubjectResponse
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }

        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int? SubjectID { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }

        public int TotalTopics { get; set; }
        public int CompletedTopics { get; set; }
        public int CompletionPercent { get; set; }
    }

    public class SyllabusStatusHierarchyResponse
    {
        public int? SubjectID { get; set; }
        public string SubjectName { get; set; }

        public int? ChapterNo { get; set; }
        public int? LessonMapId { get; set; }
        public string LessonName { get; set; }

        public int ChapterTotalTopics { get; set; }
        public int ChapterCompletedTopics { get; set; }
        public int ChapterCompletionPercent { get; set; }

        public int? TopicNo { get; set; }
        public int? TopicMapId { get; set; }
        public string TopicName { get; set; }

        public bool? IsCompleted { get; set; }
        public DateTime? TopicCompletionDate { get; set; }
    }

    public class SyllabusStatusRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
        public bool IncludeDeleted { get; set; } = false;
        public int? UserID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string IPAddress { get; set; }
        public string SEARCHKEYWORD { get; set; }
    }
}
