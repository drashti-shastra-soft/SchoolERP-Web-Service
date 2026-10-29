using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IPaymentTypeFieldService
    {
        SpResult UpsertPaymentTypeFieldAsync(PaymentFieldRequest request);
        Task<PagedResult<PaymentTypeFieldModel>> GetAllPaymentTypeFieldWithPage(SearchRequest req);
        Task<PaymentTypeFieldModel?> GetPaymentTypeFieldByIdAsync(int fieldID);
        (bool success, string message) DeletePaymentTypeField(List<int> ids, int userId, string ipAddress);
        (bool success, string message) TogglePaymentTypeFieldStatus(StatusUpdateRequest request);
        Task<List<PaymentTypeFieldModel>> GetAllPaymentTypeByIdAsync(int paymentTypeId, bool? isActive = null);
    }
}
