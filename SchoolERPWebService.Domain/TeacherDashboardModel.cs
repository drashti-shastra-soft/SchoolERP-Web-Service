using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class TeacherDashboardModel
    {
    }
    public class TeacherTimetableDto
    {
        public int TimeTableID { get; set; }
        public string Day { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string RoomNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string SubjectName { get; set; }
    }

    public class TeacherStudentDto
    {
        public int StudentID { get; set; }
        public string FullName { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNo { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
    }

    public class TeacherHomeworkDto
    {
        public int HomeworkID { get; set; }
        public string Description { get; set; }
        public DateTime HomeworkDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string SubjectName { get; set; }
        public int TotalSubmissions { get; set; }
        public int SubmittedCount { get; set; }
    }

    public class TeacherHWSubmissionDto
    {
        public int SubmissionID { get; set; }
        public string FullName { get; set; }
        public string HomeworkTitle { get; set; }
        public string SubjectName { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public decimal? MarksObtained { get; set; }
        public string SubmissionStatus { get; set; }
    }

    public class TeacherAttendanceDto
    {
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int TotalStudents { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
    }
}
