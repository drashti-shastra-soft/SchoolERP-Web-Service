using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IVoucherFieldOptionService
    {
        SpResult UpsertVoucherFieldOptionAsync(VoucherFieldOptionRequest request);
        Task<PagedResult<VoucherFieldOptionModel>> GetAllVoucherFieldOptionWithPage(SearchRequest req);
        Task<VoucherFieldOptionModel?> GetVoucherFieldOptionIDByIdAsync(int optionID);
        (bool success, string message) DeleteVoucherFieldOption(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleVoucherFieldOptionStatus(StatusUpdateRequest request);
        Task<List<VoucherFieldOptionResponse>>
    GetLinkedOptionsAsync(
        int fieldId,
        string optionValue,
        int? companyId,
        int? sessionId);
    }
}
