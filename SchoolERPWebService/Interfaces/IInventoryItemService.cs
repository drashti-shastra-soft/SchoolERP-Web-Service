using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IInventoryItemService
    {
        Task<ApiResponse> UpsertInventoryItemAsync(InventoryItemModelRequest model);
        Task<PagedResult<InventoryItemModel>> GetAllInventoryItemWithPage(SearchInventoryItemRequest req);
        Task<InventoryItemModel?> GetInventoryItemByIdAsync(int itemId);
        (bool success, string message) DeleteInventoryItem(List<int> ids, int userId);
        (bool success, string message) ToggleInventoryItemStatus(StatusUpdateRequest request);
    }
}
