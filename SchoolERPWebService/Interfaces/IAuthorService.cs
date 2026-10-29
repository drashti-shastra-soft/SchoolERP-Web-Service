using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IAuthorService
    {
        Task<ApiResponse> UpsertAuthorAsync(AuthorRequest model);
        Task<AuthorModel?> GetAuthorByIdAsync(int authorId);
        Task<PagedResult<AuthorModel>> GetAllAuthorWithPage(SearchRequest req);
        (bool success, string message) DeleteAuthor(List<int> ids, int userId);
        (bool success, string message) ToggleAuthorStatus(StatusUpdateRequest request);
        List<AuthorModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibraryAuthor(CopyForCompanyRequest req);
    }
}
