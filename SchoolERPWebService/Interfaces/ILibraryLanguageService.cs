using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibraryLanguageService
    {
        Task<ApiResponse> UpsertLibraryLanguageAsync(LibraryLanguageRequest model);
        Task<LibraryLanguageModel?> GetLibraryLanguageByIdAsync(int languageId);
        Task<PagedResult<LibraryLanguageModel>> GetAllLibraryLanguageWithPage(SearchRequest req);
        (bool success, string message) DeleteLibraryLanguage(List<int> ids, int userId);
        (bool success, string message) ToggleLibraryLanguageStatus(StatusUpdateRequest request);
        List<LibraryLanguageModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibraryLanguage(CopyForCompanyRequest req);
    }
}
