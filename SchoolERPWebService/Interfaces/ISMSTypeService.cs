using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ISMSTypeService
    {
        SpSMSType UpsertSMSTypeAsync(SMSTypeRequest request);
        Task<PagedResult<SMSTypeModel>> GetAllSMSTypeWithPage(SearchRequest req);
        Task<SMSTypeModel?> GetSMSByIdAsync(int smsTypeId);
        (bool success, string message) DeleteSMSType(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleSMSTypeStatus(StatusUpdateRequest request);
        Task<List<SMSTypeModel>> GetAllSMSTypeAsync(bool? isActive = null);
        (bool success, string message) UpsertSMSAttachmentFile(
SMSTypeAttachmentUpsertRequest req, int userId);
        List<DropdownModel> GetSMSTypeDropdownList();
    }
}
