using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IFeeTypeService
    {
        Task<ApiResponse> UpsertFeeTypeAsync(FeeTypeRequest request);
        Task<PagedResult<FeeTypeViewModel>> GetAllFeeTypeWithPage(SearchRequest req);
        Task<FeeTypeViewModel?> GetFeeTypeByIdAsync(int feeHeadId);
        (bool success, string message) DeleteFeeType(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleFeeTypeStatus(StatusUpdateRequest request);
        List<DropdownModel> GetFeeTypeDropdownList(int companyId, int sessionID, int userId);
    }
}
