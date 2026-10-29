using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibraryDocumentStatusService
    {
        Task<ApiResponse> UpsertDocumentStatusAsync(LibraryDocumentStatusRequest model);
        Task<LibraryDocumentStatusModel?> GetDocumentStatusByIdAsync(int documentStatusId);
        Task<PagedResult<LibraryDocumentStatusModel>> GetAllDocumentStatusWithPage(SearchRequest req);
        (bool success, string message) DeleteDocumentStatus(List<int> ids, int userId);
        (bool success, string message) ToggleDocumentStatus(StatusUpdateRequest request);
        List<LibraryDocumentStatusModel> GetAll(int companyId);
        Task<(bool Success, string Message)> CopyDocumentStatus(CopyForCompanyRequest req);
    }
}
