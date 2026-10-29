using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IHolidayService
    {
        Task<ApiResponse> UpsertHolidayAsync(HolidayRequestModel model);
        Task<HolidayModel?> GetHolidayByIdAsync(int HolidayId);
        Task<List<HolidayModel>> GetAllHolidayAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<HolidayModel>> GetAllHolidayWithPage(HostelTypeSearchRequest req);
        (bool success, string message) DeleteHoliday(List<int> ids, int userId);
        (bool success, string message) ToggleHolidayStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyHolidayToSession(CopyRequest req);
    }
}
