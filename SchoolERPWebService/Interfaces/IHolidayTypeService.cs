using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IHolidayTypeService
    {
        Task<ApiResponse> UpsertHostelTypeAsync(HolidayTypeRequest model);
        Task<HolidayType?> GetHolidayTypeByIdAsync(int HolidayTypeId);
        Task<List<HolidayType>> GetAllHolidayTypeAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<HolidayType>> GetAllHolidayTypeWithPage(HostelTypeSearchRequest req);
        (bool success, string message) DeleteHolidayType(List<int> ids, int userId);
        (bool success, string message) ToggleHolidayTypeStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyHolidayTypeToSession(CopyRequest req);
    }
}
