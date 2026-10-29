using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IWardensService
    {
        Task<WardenSaveResponse> UpsertWardensAsync(WardensRequestModel model);
        Task<WardensModel?> GetWardensByIdAsync(int countryId);
        Task<List<WardensModel>> GetAllWardensAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<WardensModel>> GetAllWardensWithPage(WardensSearchRequest req);
        (bool success, string message) DeleteWardens(List<int> ids, int userId);
        (bool success, string message) ToggleWardensStatus(StatusUpdateRequest request);
        (bool Success, string Message) UpdateWardenProfile(WardenProfileRequest req);
    }
}
