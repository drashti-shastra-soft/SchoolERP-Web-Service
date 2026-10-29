using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IAccountTypeService
    {
        Task<ApiResponse> UpsertAccountTypeAsync(AccountTypeModelRequest model);
        Task<ApiResponse> UpsertAccountAddSystemTypeAsync(AccountTypeModelRequest model);
        Task<PagedResult<AccountTypeModel>> GetAllAccountTypeWithPage(SearchRequest req);
        Task<AccountTypeModel?> GetAccountTypeByIdAsync(int accountTypeID);
        (bool success, string message) DeleteAccountType(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleAccountTypeStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyAccountTypeToSession(CopyRequest req);
        Task<List<AccountTypeModel>> GetAllAccountTypeAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetClosesToDropdownList(int companyId, int userId);
    }
}
