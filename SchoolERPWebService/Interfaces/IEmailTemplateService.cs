using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IEmailTemplateService
    {
        /// <summary>Upsert template, then (optionally) save file + update attachment columns.</summary>
        Task<EmailTemplateSaveResponse> UpsertEmailTemplateAsync(EmailTemplateSaveRequest model);

        /// <summary>Calls SP_EmailTemplate_UpdateAttachment directly (path / type / name).</summary>
        Task<ApiResponse> UpdateEmailTemplateAttachmentAsync(EmailTemplateAttachmentRequest model);

        Task<PagedResult<EmailTemplateViewModel>> GetAllEmailTemplateWithPage(SearchRequest req);

        Task<EmailTemplateDetailViewModel?> GetEmailTemplateByIdAsync(int emailTemplateId);

        Task<(bool success, string message)> DeleteEmailTemplateAsync(List<int> ids, int userId, string ipAddress);

        Task<(bool success, string message)> ToggleEmailTemplateStatusAsync(StatusUpdateRequest request);

        List<EmailTempletDropdwon> GetEmailTemplateToDropdownList(int companyId, int userId);
    }
}