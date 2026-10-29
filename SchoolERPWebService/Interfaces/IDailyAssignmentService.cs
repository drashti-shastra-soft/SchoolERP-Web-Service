using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IDailyAssignmentService
    {
        DailyAssignmentModel? GetByID(int assignmentID);
        Task<ApiResponse> Upsert(
            AssignmentUpsertRequest request,
            int userId);
        Task<ApiResponse> UpsertAttachment(
            AssignmentAttachmentUpsertRequest request, int userId);

        Task<PagedResult<DailyAssignmentModel>> GetAllDailyAssignmentWithPage(DailyAssignmentSearchRequest req, int userId);
        (bool success, string message) Delete(List<int> ids, int userId);
    }
}
