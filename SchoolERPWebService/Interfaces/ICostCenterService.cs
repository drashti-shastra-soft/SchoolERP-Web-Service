using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ICostCenterService
    {
        Task<ApiResponse> UpsertCostCenterAsync(CostCenterModelRequest model);
        Task<PagedResult<CostCenterModel>> GetAllCostCenterWithPage(SearchRequest req);
        Task<CostCenterModel?> GetCostCenterByIdAsync(int itemStoreId);
        (bool success, string message) DeleteCostCenter(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleCostCenterStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyCostCenterToSession(CopyRequest req);
        Task<List<CostCenterModel>> GetAllCostCenterAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetCostCenterDropdownList(int companyID, int sessionId);
    }
}
