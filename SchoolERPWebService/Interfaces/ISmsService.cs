using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ISmsService
    {
        Task<SmsSendResult> SendAsync(int companyId, string toPhone, string message);
    }
}
