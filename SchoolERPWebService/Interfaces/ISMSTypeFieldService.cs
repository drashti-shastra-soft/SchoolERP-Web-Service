using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ISMSTypeFieldService
    {
        SpResult UpsertSMSTypeFieldAsync(SMSFieldRequest request);
        Task<PagedResult<SMSTypeFieldModel>> GetAllSMSTypeFieldWithPage(SearchRequest req);
        Task<SMSTypeFieldModel?> GetSMSTypeFieldByIdAsync(int fieldID);
        (bool success, string message) DeleteSMSTypeField(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleSMSTypeFieldStatus(StatusUpdateRequest request);
        Task<List<SMSTypeFieldModel>> GetAllSMSTypeByIdAsync(int smsTypeId, bool? isActive = null);
    }
}
