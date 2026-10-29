using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IVoucherTypeService
    {
        Task<ApiResponse> UpsertVoucherTypeAsync(VoucherTypeModelRequest model);
        Task<PagedResult<VoucherTypeModel>> GetAllVoucherTypeWithPage(SearchRequest req);
        Task<VoucherTypeModel?> GetVoucherTypeByIdAsync(int voucherId);
        (bool success, string message) DeleteVoucherType(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleVoucherTypeStatus(StatusUpdateRequest request);
        List<DropdownModel> GetVoucherTypeDropdownList();
    }
}
