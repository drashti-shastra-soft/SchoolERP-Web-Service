using SchoolERPWebService.Domain;
using Common = SchoolERPWebService.Domain.Common;
namespace SchoolERPWebService.Interfaces
{
    public interface IAuthServices
    {
        Task<Common.ApiResponse<UserSessionModel?>> LoginAsync(string username, string password);
        Task<DashboardModel?> GetDashboardByIdAsync(int dashboardId);
    }
}
