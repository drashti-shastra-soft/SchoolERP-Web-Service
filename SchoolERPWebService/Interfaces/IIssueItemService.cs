using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IIssueItemService
    {
        Task<ApiResponse> UpsertItemIssueAsync(ItemIssueRequest model);
        Task<PagedResult<ItemIssueModel>> GetAllItemIssueWithPage(SearchInventoryItemRequest req);
        Task<ItemIssueModel?> GetItemIssueByIdAsync(int itemcategoryId);
        (bool success, string message) DeleteItemIssue(List<int> ids, int userId);
        (bool success, string message) ToggleItemIssueStatus(StatusUpdateRequest request);
        Task<ApiResponse> UpsertItemIssueReturnAsync(ItemIssueReturnModel model);
    }
}
