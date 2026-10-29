using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibrarySeriesService
    {
        Task<ApiResponse> UpsertLibrarySeriesAsync(LibrarySeriesRequest model);
        Task<LibrarySeriesModel?> GetLibrarySeriesByIdAsync(int seriesId);
        Task<PagedResult<LibrarySeriesModel>> GetAllLibrarySeriesWithPage(SearchRequest req);
        (bool success, string message) DeleteLibrarySeries(List<int> ids, int userId);
        (bool success, string message) ToggleLibrarySeriesStatus(StatusUpdateRequest request);
        List<LibrarySeriesModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibrarySeries(CopyForCompanyRequest req);
    }
}
