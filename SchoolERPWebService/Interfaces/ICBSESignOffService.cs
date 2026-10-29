using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ICBSESignOffService
    {
        Task<ApiResponse> UpsertCBSESignOffAsync(SignOfRequest request);
        Task<PagedResult<CBSESignOffViewModel>> GetAllCBSESignOffWithPage(SearchRequest req);
        Task<CBSESignOffViewModel?> GetCBSESignOffByIdAsync(int signOffID);
        (bool success, string message) DeleteCBSESignOff(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleCBSESignOffStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyCBSESignOffToSession(CopyRequest req);
        Task<List<CBSESignOffViewModel>> GetAllCBSESignOffAsync(int companyId, int sessionId, bool includeDeleted = false);
    }
}
