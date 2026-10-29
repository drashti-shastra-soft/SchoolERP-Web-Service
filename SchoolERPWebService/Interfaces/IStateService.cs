using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IStateService
    {
        Task<ApiResponse> UpsertStateAsync(StateRequestModel model);
        Task<StateModel> GetStateByIdAsync(int stateID);
        Task<List<StateModel>> GetAllStateAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<StateModel>> GetAllStateWithPage(HostelTypeSearchRequest req);
        (bool success, string message) DeleteState(List<int> ids, int userId);
        (bool success, string message) ToggleStateStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyStateToSession(CopyRequest req);
        Task<List<StateModel>> GetAllStateByCountyAsync(int companyId, int sessionId, int countryId);
        
    }
}
