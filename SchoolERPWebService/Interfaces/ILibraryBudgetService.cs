using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Defines operations for managing school library budgets.
    /// </summary>
    public interface ILibraryBudgetService
    {
        /// <summary>
        /// Creates or updates a library budget.
        /// </summary>
        /// <param name="model">The library budget request details.</param>
        /// <returns>An API response indicating operation outcome.</returns>
        Task<ApiResponse> UpsertLibraryBudgetAsync(LibraryBudgetRequest model);

        /// <summary>
        /// Retrieves a library budget record by its identifier.
        /// </summary>
        /// <param name="budgetId">The library budget ID.</param>
        /// <returns>The library budget model details if found; otherwise null.</returns>
        Task<LibraryBudgetModel?> GetLibraryBudgetByIdAsync(int budgetId);

        /// <summary>
        /// Retrieves a paginated list of library budgets based on the search criteria.
        /// </summary>
        /// <param name="req">The search request criteria.</param>
        /// <returns>A paginated list of library budgets.</returns>
        Task<PagedResult<LibraryBudgetModel>> GetAllLibraryBudgetWithPage(SearchRequest req);

        /// <summary>
        /// Deletes one or more library budgets by ID.
        /// </summary>
        /// <param name="ids">List of library budget identifiers to delete.</param>
        /// <param name="userId">The identifier of the user performing the deletion.</param>
        /// <returns>A tuple indicating success status and status message.</returns>
        (bool success, string message) DeleteLibraryBudgety(List<int> ids, int userId);

        /// <summary>
        /// Toggles the active/inactive status of a library budget.
        /// </summary>
        /// <param name="request">Status update request details.</param>
        /// <returns>A tuple indicating success status and status message.</returns>
        (bool success, string message) ToggleLibraryBudgetyStatus(StatusUpdateRequest request);
        
        /// <summary>
        /// Retrieves all budget for the company with optional includdeleted filtering.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="includdeleted">Optional includdeleted to filter budget.</param>
        /// <returns>A list of budget view models matching the criteria.</returns>
        List<LibraryBudgetModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibraryBudget(CopyForCompanyRequest req);
    }
}
