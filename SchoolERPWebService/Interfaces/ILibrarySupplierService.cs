using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibrarySupplierService
    {
        Task<ApiResponse> UpsertLibrarySupplierAsync(LibrarySupplierRequest model);
        Task<LibrarySupplierModel?> GetLibrarySupplierByIdAsync(int supplierId);
        Task<PagedResult<LibrarySupplierModel>> GetAllLibrarySupplierWithPage(SearchRequest req);
        (bool success, string message) DeleteLibrarySupplier(List<int> ids, int userId);
        (bool success, string message) ToggleLibrarySupplierStatus(StatusUpdateRequest request);
        List<LibrarySupplierModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibrarySupplier(CopyForCompanyRequest req);
    }
}
