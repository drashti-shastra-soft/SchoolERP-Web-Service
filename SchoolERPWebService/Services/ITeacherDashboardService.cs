using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Services
{
    public interface ITeacherDashboardService
    {
        List<TeacherTimetableDto> GetTimetable(int companyId, int sessionId, int staffId);
        List<TeacherStudentDto> GetStudents(int companyId, int sessionId, int staffId);
        List<TeacherHomeworkDto> GetHomework(int companyId, int sessionId, int staffId);
        List<TeacherHWSubmissionDto> GetHWSubmissions(int companyId, int sessionId, int staffId);
        List<TeacherAttendanceDto> GetAttendance(int companyId, int sessionId, int staffId);
    }
}
