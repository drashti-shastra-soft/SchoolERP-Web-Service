using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IBudgetHeadService
    {
        Task<ApiResponse> UpsertBudgetHeadAsync(BudgetHeadModelRequest model);
        Task<PagedResult<BudgetHeadModel>> GetAllBudgetHeadWithPage(SearchRequest req);
        Task<BudgetHeadModel?> GetBudgetHeadByIdAsync(int budgetHeadId);
        (bool success, string message) DeleteBudgetHead(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleBudgetHeadStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyBudgetHeadToSession(CopyRequest req);
        Task<List<BudgetHeadModel>> GetAllBudgetHeadAsync(int companyId, int sessionId, bool includeDeleted = false);
    }
}
