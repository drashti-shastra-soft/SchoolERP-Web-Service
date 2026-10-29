using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing account heads (e.g., 'Tuition Fees', 'Electricity Expense').
    /// </summary>
    public interface IAccountHeadService
    {
        /// <summary>
        /// Retrieves a list of all account heads for a specific category (e.g., 'Income' or 'Expense').
        /// </summary>
        List<AccountHeadViewModel> GetAllAccountHeads(int companyId, int sessionId, string headType, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific account head using its unique ID.
        /// </summary>
        AccountHeadModel? GetAccountHeadByID(int id);

        /// <summary>
        /// Adds a new account head or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertAccountHead(AccountHeadUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes an account head from the system.
        /// </summary>
        (bool Success, string Message) DeleteAccountHead(List<int> id, int userId);

        /// <summary>
        /// Turns an account head's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleAccountHeadStatus(StatusUpdateRequest request);
        Task<Domain.Common.PagedResult<AccountHeadViewModel>> GetAllAccountHeadWithPageIndex(AccountHeadSearchRequest req);
        Task<(bool Success, string Message)> CopyAccountHead(CopyRequest req);
        SpAccount UpsertAccountHead(AccountRequest request);
        List<DropdownModel> GetAccountParentDropdownList(int? excludeAccountID = null, int? companyId = null, int? sessionId = null);
        Task<PagedResult<AccountHeadModel>> GetAllListAccountHeadWithPageIndex(SearchAccountHeadRequest req);
        (bool success, string message) UpsertAccountHeadAttachmentFile(
  AccountEntryAttachmentUpsertRequest req, int userId);
    }
}
