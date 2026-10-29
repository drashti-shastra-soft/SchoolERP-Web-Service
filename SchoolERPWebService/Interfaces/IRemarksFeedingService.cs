using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IRemarksFeedingService
    {
        Task<List<RemarksFeedingViewModel>> GetRemarksFeedingAsync(AttendanceFeedingRequest request);
        (bool success, string message) UpsertRemarksFeedingAsync(RemarksFeedingRequest request);
        (bool success, string message) DeleteRemarksFeedingAsync(List<int> remarksFeedingID, int userId, string ipAddress);
    }
}
