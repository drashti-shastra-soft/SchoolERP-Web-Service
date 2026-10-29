using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibrarySubjectService
    {
        Task<ApiResponse> UpsertLibrarySubjectAsync(LibrarySubjectRequest model);
        Task<LibrarySubjectModel?> GetLibrarySubjectByIdAsync(int subjectId);
        Task<PagedResult<LibrarySubjectModel>> GetAllLibrarySubjectWithPage(SearchRequest req);
        (bool success, string message) DeleteLibrarySubject(List<int> ids, int userId);
        (bool success, string message) ToggleLibrarySubjectStatus(StatusUpdateRequest request);
        List<LibrarySubjectModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibrarySubject(CopyForCompanyRequest req);
    }
}
