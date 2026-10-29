using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IItemStoreServices
    {
        Task<ApiResponse> UpsertItemStoreAsync(ItemStoreModelRequest model);
        Task<PagedResult<ItemStoreModel>> GetAllItemStoreWithPage(SearchRequest req);
        Task<ItemStoreModel?> GetItemStoreByIdAsync(int itemStoreId);
        (bool success, string message) DeleteItemStore(List<int> ids, int userId);
        (bool success, string message) ToggleItemStoreStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyItemStoreToSession(CopyRequest req);
        Task<List<ItemStoreModel>> GetAllItemStoreAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetItemByItemStoreDropdownList(int companyId, int sessionId, int userId);
    }
}
