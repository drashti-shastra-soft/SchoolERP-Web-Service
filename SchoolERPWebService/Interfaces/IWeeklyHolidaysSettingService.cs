using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IWeeklyHolidaysSettingService
    {
        Task<ApiResponse> UpsertWeeklyHolidaysSettingAsync(WeeklyHolidayBatchRequest model);
        Task<WeeklyHolidaysSettingModel?> GetWeeklyHolidaysSettingByIdAsync(int id);
        Task<List<WeeklyHolidaysSettingModel>> GetAllWeeklyHolidaysSettingAsync(int companyId, bool includeDeleted = false);
        Task<PagedResult<WeeklyHolidaysSettingModel>> GetAllWeeklyHolidaysSettingWithPage(WeeklyHolidaysSettingSearchRequest req);
        (bool success, string message) DeleteWeeklyHolidaysSetting(List<int> ids, int userId);
        (bool success, string message) ToggleWeeklyHolidaysSettingStatus(StatusUpdateRequest request);
    }
}
