using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IVoucherHeaderService
    {
        SpResult UpsertVoucherHeaderAsync(VoucherHeaderRequest request);
        Task<PagedResult<VoucherHeaderModel>> GetAllVoucherHeaderWithPage(VoucherHeaderSearchRequest req);
        Task<VoucherHeaderFullResult?> GetVoucherHeaderIDByIdAsync(int voucherHeaderID);
        (bool success, string message) DeleteVoucherHeader(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleVoucherHeaderStatus(StatusUpdateRequest request);

        #region Voucher Header Value
        SpResult UpsertVoucherHeaderValueAsync(VoucherHeaderValueRequest request);
        List<VoucherHeaderValue> GetVoucherHeaderValueList(int companyID, int sessionID, int fieldID, bool? isActive);
        Task<VoucherHeaderValue?> GetVoucherHeaderValueByIdAsync(int valueID);
        #endregion

        #region Voucher Details
        SpResult UpsertVoucherDetailAsync(VoucherDetailRequest request);
        List<VoucherDetail> GetVoucherDetailList(int companyID, int sessionID, int voucherHeaderID, bool? isActive);
        Task<VoucherDetail?> GetVoucherDetailByIdAsync(int detailID);
        #endregion

        #region   Voucher Details Value
        SpResult UpsertVoucherDetailValueAsync(VoucherDetailValueRequest request);
        List<VoucherDetailValue> GetVoucherDetailValueList(VoucherValueSearchRequest request);
        Task<VoucherDetailValue?> GetVoucherDetailValueByIdAsync(int valueID);
        #endregion

        Task<VoucherSaveFullResult> SaveVoucherFullAsync(
    VoucherSaveFullRequest request);

        List<DropdownModel> GetStaffDropdownList(int companyID, int sessionId);
        List<DropdownModel> GetStudentsDropdownList(int companyID, int sessionId);

        (bool success, string message) UpsertVoucherHeaderAttachmentFile(
    VoucherHeaderAttachmentUpsertRequest req, int userId);

        List<VoucherDetailClearingResponse> GetForClearing(VoucherBankClearingRequest req);
        Task<SpResult> SaveVoucherClearingAsync(
    VoucherBankClearingSaveRequest request);
    }
}
