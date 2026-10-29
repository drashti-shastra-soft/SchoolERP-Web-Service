using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IEmailSenderService
    {
        Task<EmailSendResult> SendAsync(EmailMessage m, CancellationToken ct = default);
    }
}
