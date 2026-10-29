using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IPaymentTypeService
    {
        SpPaymentType UpsertPaymentTypeAsync(PaymentTypeRequest request);
        Task<PagedResult<PaymentTypeModel>> GetAllPaymentTypeWithPage(SearchRequest req);
        Task<PaymentTypeModel?> GetPaymentTypeByIdAsync(int paymentTypeId);
        (bool success, string message) DeletePaymentType(List<int> ids, int userId, string ipAddress);
        (bool success, string message) TogglePaymentTypeStatus(StatusUpdateRequest request);
        Task<List<PaymentTypeModel>> GetAllPaymentTypeAsync(bool? isActive = null);
        (bool success, string message) UpsertPaymentTypeAttachmentFile(
 PaymentTypeAttachmentUpsertRequest req, int userId);
        List<DropdownModel> GetPaymenTypeDropdownList();
    }
}
