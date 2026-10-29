using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ICategoryGroupService
    {
        Task<ApiResponse> UpsertCategoryGroupAsync(CategoryGroupRequest request);
        Task<PagedResult<CategoryGroupViewModel>> GetAllCategoryGroupWithPage(SearchRequest req);
        Task<CategoryGroupViewModel?> GetCategoryGroupByIdAsync(int categoryGroupId);
        (bool success, string message) DeleteCategoryGroup(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleCategoryGroupStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyCategoryGroupToSession(CopyRequest req);
        Task<List<CategoryGroupViewModel>> GetAllCategoryGroupAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetCategoryGroupDropdownList(int companyId, int sessionID, int userId);
    }
}
