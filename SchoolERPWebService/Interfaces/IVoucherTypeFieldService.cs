using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IVoucherTypeFieldService
    {
        SpResult UpsertVoucherFieldAsync(VoucherFieldRequest request);
        Task<PagedResult<VoucherTypeFieldModel>> GetAllVoucherTypeFielddWithPage(SearchRequest req);
        Task<VoucherTypeFieldModel?> GetVoucherTypeFieldByIdAsync(int fieldID);
        (bool success, string message) DeleteVoucherTypeField(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleVoucherTypeFieldStatus(StatusUpdateRequest request);
        List<DropdownModel> GetVoucherTypeFieldDropdownList(int companyID);
        Task<List<VoucherDynamicFieldResponse>>GetVoucherDynamicFieldsAsync(int companyId,int voucherId, int sessionId);
    }
}
