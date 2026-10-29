using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IMarksFeedingService
    {
        Task<List<MarksFeedingResponse>> GetAllMarksFeedingBindGridAsync(MarksFeedingGetRequest Req);
        (bool success, string message) DeleteMarksFeeding(List<int> marksFeedingID, int userId, string ipAddress);
        (bool success, string message) UpsertMarksFeeding(MarksFeedingRequest request);
    }
}
