using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IDashboardApiService
    {
        Task<List<DashboardModel>> GetAllAsync();
        Task<DashboardModel?> GetByIdAsync(int dashboardId);
        Task<(bool Success, string Message, int DashboardID)> InsertUpdateAsync(DashboardRequestModel request, int userId, string ipAddress);
        Task<(bool Success, string Message)> ToggleStatusAsync(StatusUpdateRequest request);
        Task<(bool Success, string Message)> DeleteMultipleAsync(List<int> dashboardIds, int userId, string ipAddress);
        Task<DashboardModel?> GetByRoleIdAsync(int roleId);
        Task<AttendanceCountModel> GetStudentAttendanceCount(ReqAttendanceCountModel req);
        Task<List<UserRoleWiseCountModel>> GetUserRoleWiseCount(int companyId, int sessionId, int userId);
    }
}
