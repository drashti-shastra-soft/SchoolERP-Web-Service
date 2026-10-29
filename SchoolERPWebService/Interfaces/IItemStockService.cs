using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IItemStockService
    {
        SpItemStockReceive UpsertItemStockAsync(ItemStockModelRequest model);
        Task<PagedResult<ItemStockModel>> GetAllItemStockWithPage(SearchItemStockRequest req);
        Task<ItemStockModel?> GetItemStockByIdAsync(int itemStockId);
        (bool success, string message) DeleteItemStock(List<int> ids, int userId);
        (bool success, string message) ToggleItemStockStatus(StatusUpdateRequest request);
        (bool success, string message) UpsertItemStockAttachmentFile(
   ItemStockAttachmentUpsertRequest req, int userId);
    }
}
