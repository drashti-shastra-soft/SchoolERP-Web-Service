using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IVehicleAssignService
    {
        List<VehicleAssignViewModel> GetAllAssignments(int companyId, int sessionId);
        VehicleAssignViewModel? GetAssignmentByID(int id);
        (bool Success, string Message) UpsertAssignments(VehicleAssignUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteAssignment(List<int> id, int userId);
        //(bool Success, string Message) ToggleAssignmentStatus(int id, bool isActive, int userId);
        (bool success, string message) ToggleAssignmentStatus(StatusUpdateRequest request);

        Task<StudentTransportDetailsModel?> GetTransportDetailsByStudents(
            int companyId,
            int sessionId, int studentId);
    }
}
