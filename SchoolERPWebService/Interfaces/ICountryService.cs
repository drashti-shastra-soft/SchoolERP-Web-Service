using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ICountryService
    {
        Task<ApiResponse> UpsertCountryAsync(CountryRequestModel model);
        Task<CountryModel?> GetCountryByIdAsync(int countryId);
        Task<List<CountryModel>> GetAllCountryAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<CountryModel>> GetAllCountryWithPage(HostelTypeSearchRequest req);
        (bool success, string message) DeleteCountry(List<int> ids, int userId);
        (bool success, string message) ToggleCountryStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyCountryToSession(CopyRequest req);
    }
}
