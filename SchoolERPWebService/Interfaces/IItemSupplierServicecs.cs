using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IItemSupplierServicecs
    {
        Task<ApiResponse> UpsertItemSupplierAsync(ItemSupplierRequest model);
        Task<PagedResult<ItemSupplierModel>> GetAllItemSupplierWithPage(SearchRequest req);
        Task<ItemSupplierModel?> GettemSupplierByIdAsync(int itemSupplierId);
        (bool success, string message) DeleteItemSupplier(List<int> ids, int userId);
        (bool success, string message) ToggleItemSupplierStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyItemSupplierToSession(CopyRequest req);
        Task<List<ItemSupplierModel>> GetAllItemSupplierAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetItemSupplierDropdownList(int companyId, int sessionId, int userId);
    }
}
