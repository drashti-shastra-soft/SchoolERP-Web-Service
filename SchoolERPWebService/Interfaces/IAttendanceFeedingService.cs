using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IAttendanceFeedingService
    {
        Task<List<AttendanceFeedingViewModel>> GetAttendanceFeedingAsync(AttendanceFeedingRequest request);
        (bool success, string message) DeleteAttendanceFeeding(List<int> attendanceFeedingID, int userId, string ipAddress);
        (bool success, string message) UpsertAttendanceFeeding(AttendanceFeedingAddRequest request);
    }
}
