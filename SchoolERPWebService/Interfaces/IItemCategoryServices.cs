using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IItemCategoryServices
    {
        Task<ApiResponse> UpsertItemCategoryAsync(ItemCategoryModelRequest model);
        Task<PagedResult<ItemCategoryModel>> GetAllItemCategoryWithPage(SearchRequest req);
        Task<ItemCategoryModel?> GetItemCategoryByIdAsync(int itemcategoryId);
        (bool success, string message) DeleteItemCategory(List<int> ids, int userId);
        (bool success, string message) ToggleItemCategoryStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyItemCategoryToSession(CopyRequest req);
        Task<List<ItemCategoryModel>> GetAllItemCategoryAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetItemCategorDropdownList(int companyId, int sessionId, int userId);
        List<DropdownItemCategoryModel> GetItemByItemCategoryDropdownList(int companyId, int sessionId, int itemCategoryId, int userId);
    }
}
