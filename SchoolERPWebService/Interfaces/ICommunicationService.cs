using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ICommunicationService
    {
        Task<CommunicationSaveResponse> SaveCommunicationAsync(
       CommunicationSaveRequest model);

        Task<List<BirthdayTodayResponse>>
       GetTodayBirthdaysAsync(BirthdayTodayRequest request);

        Task<PagedResult<CommunicationLogResponse>>
   GetCommunicationLogWithPageAsync(CommunicationLogRequest req);
    }
}
