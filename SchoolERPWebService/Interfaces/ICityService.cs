using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ICityService
    {
        Task<CityApiResponse> UpsertCityAsync(CityUpsertRequest model);
        Task<CityModel?> GetCityByIdAsync(int cityID);
        Task<CityListResponse> GetAllCityAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<CityModel>> GetAllCityWithPage(CitySearchRequest req);
        (bool success, string message) DeleteCity(List<int> ids, int userId);
        (bool success, string message) ToggleCityStatus(StatusUpdateRequest request);
        Task<List<CityModel>> GetAllCityByStateIdAsync(int stateId);
    }
}
