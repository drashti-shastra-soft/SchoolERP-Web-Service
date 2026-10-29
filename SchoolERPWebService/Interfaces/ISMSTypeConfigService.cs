using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ISMSTypeConfigService
    {
        Task<(bool Result, string Message)> SaveSMSTypeConfigBatchAsync(
    int? companyId, int smsTypeId, List<ConfigFieldItem> fields, int? userId);
        Task<List<FieldConfigResponse>> GetByCompanysmsTypeAsync(int smsTypeId, int companyID);
    }
}
