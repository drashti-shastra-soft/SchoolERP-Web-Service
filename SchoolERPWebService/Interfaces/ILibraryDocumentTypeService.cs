using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibraryDocumentTypeService
    {
        Task<ApiResponse> UpsertLibraryDocumentTypeAsync(LibraryDocumentTypeRequest model);
        Task<LibraryDocumentTypeModel?> GetLibraryDocumentTypeByIdAsync(int documentId);
        Task<PagedResult<LibraryDocumentTypeModel>> GetAllLibraryDocumentTypeWithPage(SearchRequest req);
        (bool success, string message) DeleteLibraryDocumentType(List<int> ids, int userId);
        (bool success, string message) ToggleLibraryDocumentTypeStatus(StatusUpdateRequest request);
        List<LibraryDocumentTypeModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibraryDocumentType(CopyForCompanyRequest req);
    }
}
