using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IBackDateSettingService
    {
        Task<ApiResponse> UpsertAttendanceBackDateSettingAsync(SaveBackDateSettingRequest model);
        Task<BackDateSettingViewModel?> GetAttendanceBackDateSettingByIdAsync(int settingID);
        Task<PagedResult<BackDateSettingViewModel>> GetAllAttendanceBackDateSettingWithPage(BackDateSettingSearchRequest req);
        Task<int> GetBackDateCount(int companyId, int staffID, int roleId);
        Task<BackDateSettingViewModel?> GetAttendanceBackDateSettingByStaffIdAsync(int staffId);
    }
}
