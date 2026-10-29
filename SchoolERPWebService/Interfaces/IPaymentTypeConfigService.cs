using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IPaymentTypeConfigService
    {
        SpPaymentType UpsertPaymentTypeConfigAsync(PaymentTypeConfigRequest request);
        Task<(bool Result, string Message)> SavePaymentTypeConfigBatchAsync(
    int? companyId, int paymentTypeId, List<ConfigFieldItem> fields, int? userId, string? userName);
        Task<List<FieldConfigResponse>> GetByCompanyPaymentTypeAsync(int paymentTypeId, int companyID);
    }
}
