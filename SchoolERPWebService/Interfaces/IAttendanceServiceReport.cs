using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IAttendanceServiceReport
    {
        Task<AttendanceMonthlyPagedResult<AttendanceMonthlyResponse>> GetAttendanceMonthlyReport(AttendanceMonthlyReportRequest req);
        Task<PagedResult<AttendanceTypeReportResponse>> GetAttendanceTypeReport(StudentReportRequest req);
        Task<PagedResult<AttendanceDailyReportResponse>> GetAttendanceDailyReport(StudentReportRequest req);
        Task<PagedResult<AttendanceDayWiseReportResponse>> GetAttendanceDailyWiseReport(StudentReportRequest req);
        Task<AttendanceMonthlyPagedResult<AttendanceStaffMonthlyResponse>> GetAttendanceStaffMonthlyReport(AttendanceMonthlyReportRequest req);
        Task<PagedResult<StaffAttendanceDayWiseReportResponse>> GetStaffAttendanceDayWiseReport(StudentReportRequest req);
    }
}
