using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Provides methods for managing student attendance, including attendance entry,
    /// bulk attendance saving, and attendance history reporting.
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// Retrieves the attendance list for students in a specific class and section
        /// on the selected attendance date.
        /// </summary>
        List<StudentAttendanceViewModel> GetStudentAttendanceList(int classId, int sectionId, DateTime date, int companyId);
        /// <summary>
        /// Saves attendance records for multiple students in a single operation.
        /// </summary>
        (bool Success, string Message) SaveBulkAttendance(AttendanceUpsertRequest req, int companyId, int userId);
        /// <summary>
        /// Retrieves attendance history and monthly attendance summaries
        /// for a specific student and academic year.
        /// </summary>
        StudentAttendanceHistoryViewModel GetStudentAttendanceHistory(int studentId, int year, int companyId);

        Task<PagedResult<StudentAttendanceViewModel>> GetAllStudentAttendanceWithPage(StudentAttendanceSearchRequest req);
        Task<PagedResult<StudentAttendanceViewModel>> GetAllStudentAttendanceReportWithPage(StudentAttendanceSearchRequest req);

        Task<StudentAttendanceSummaryModel?> GetStudentAttendanceSummary(int companyId, int sessionId, int studentId, int? year = null, int? status = null);

        Task<AttendanceCountViewModel> GetPresentStudentCount(int companyId, int sessionId, int attendanceStatus, DateTime date);

        Task<StaffAttendanceCountViewModel> GetPresentStaffCount(int companyId, int sessionId, DateTime? date = null);
    }
}
