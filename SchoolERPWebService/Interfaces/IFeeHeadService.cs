using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IFeeHeadService
    {
        Task<ApiResponse> UpsertFeeHeadAsync(FeeHeadRequest request);
        Task<PagedResult<FeeHeadViewModel>> GetAllFeeHeadWithPage(SearchRequest req);
        Task<FeeHeadViewModel?> GetFeeHeadByIdAsync(int feeCategoryId);
        (bool success, string message) DeleteFeeHead(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleFeeHeadStatus(StatusUpdateRequest request);
        List<DropdownModel> GetFeeHeadDropdownList(int companyId, int sessionID, int userId);
    }
}
