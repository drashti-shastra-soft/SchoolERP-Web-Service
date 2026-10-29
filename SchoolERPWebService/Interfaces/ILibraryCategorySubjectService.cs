using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibraryCategorySubjectService
    {
        Task<ApiResponse> UpsertLibraryCategorySubjectAsync(LibraryCategorySubjectRequest model);
        Task<LibraryCategorySubjectModel?> GetLibraryCategorySubjectByIdAsync(int id);
        Task<PagedResult<LibraryCategorySubjectModel>> GetAllLibraryCategorySubjectWithPage(LibraryCategorySubjectSearchRequest req);
        (bool success, string message) DeleteLibraryCategorySubject(List<int> ids, int userId);
        (bool success, string message) ToggleLibraryCategorySubjectStatus(StatusUpdateRequest request);
        Task<List<LibraryCategoryModel>> GetAllLibraryCategory(int companyId, bool? isActive = null);
        Task<List<LibrarySubjectModel>> GetAllLibrarySubject(int companyId, bool? isActive = null);
    }
}
