using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IFeeCategoryService
    {
        Task<ApiResponse> UpsertFeeCategoryAsync(FeeCategoryRequest request);
        Task<PagedResult<FeeCategoryViewModel>> GetAllFeeCategoryWithPage(SearchRequest req);
        Task<FeeCategoryViewModel?> GetFeeCategoryByIdAsync(int feeCategoryId);
        (bool success, string message) DeleteFeeCategory(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleFeeCategoryStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyFeeCategoryToSession(CopyRequest req);
        Task<List<FeeCategoryViewModel>> GetAllFeeCategoryAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetFeeCategoryDropdownList(int companyId, int sessionID, int userId, int? categoryGroupId);
    }
}
