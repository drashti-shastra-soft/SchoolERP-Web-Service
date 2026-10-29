using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ISMSTemplateService
    {
        Task<ApiResponse> UpsertSMSTemplateAsync(SMSTemplateSaveRequest model);
        Task<PagedResult<SMSTemplateViewModel>> GetAllSMSTemplateWithPage(SearchRequest req);
        Task<SMSTemplateViewModel?> GetSMSTemplateByIdAsync(int smsTemplateId);
        (bool success, string message) DeleteSMSTemplate(List<int> ids, int userId, string ipAddress);
        (bool success, string message) ToggleSMSTemplateStatus(StatusUpdateRequest request);
        List<EmailTempletDropdwon> GetSMSTemplateToDropdownList(int companyId, int userId);
    }
}
