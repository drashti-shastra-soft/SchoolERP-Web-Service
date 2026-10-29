using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibraryCategoryService
    {
        Task<ApiResponse> UpsertLibraryCategoryAsync(LibraryCategoryRequest model);
        Task<LibraryCategoryModel?> GetLibraryCategoryByIdAsync(int categoryId);
        Task<PagedResult<LibraryCategoryModel>> GetAllLibraryCategoryWithPage(SearchRequest req);
        (bool success, string message) DeleteLibraryCategory(List<int> ids, int userId);
        (bool success, string message) ToggleLibraryCategoryStatus(StatusUpdateRequest request);
        List<LibraryCategoryModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibraryCategory(CopyForCompanyRequest req);
    }
}
