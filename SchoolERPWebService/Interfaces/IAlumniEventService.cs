using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IAlumniEventService
    {
        List<AlumniEventViewModel> GetEvents(string? searchText, int companyId);
        SpEvent UpsertEvent(AlumniEventUpsertRequest req, int companyId, int userId);
        (bool Success, string Message) DeleteEvent(List<int> id, int companyId, int userId);
        (bool Success, string Message) ToggleEventStatus(StatusUpdateRequest request, int companyId, int sessionId);
        (byte[]? Bytes, string? FileName, string? ContentType) GetEventPhoto(int eventId, int companyId);
        Task<PagedResult<AlumniEventViewModel>> GetAllEventsWithPage(SearchRequest req);
        AlumniEventViewModel GetEventsById(int id);
        (bool success, string message) AlumniEventAttachmentFile(
   AlumniEventAttachmentUpsertRequest req, int userId);
    }
}
