using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IHomeworkService
    {
        List<HomeworkViewModel> GetAll(int companyId, int sessionId, bool includeDeleted = false);
        HomeworkViewModel? GetByID(int id,int? studentId= null);
        Task<ApiResponse> Upsert(
            HomeworkUpsertRequest request,
            int companyId,
            int sessionId,
            int userId);
        (bool success, string message) Delete(List<int> ids, int userId);
        (bool success, string message) ToggleStatus(StatusUpdateRequest request);
        Task<PagedResult<HomeworkViewModel>> GetAllHomeWorkWithPage(SearchRequest req);
        Task<ApiResponse> UpsertAttachment(
            HomeworkAttachmentUpsertRequest request, int userId);
        List<HomeworkAttachmentViewModel> GetAllHomeWorkAttechmentById(int homeWorkId, int userId);

        Task<ApiResponse> UpsertSubmission(
            HomeworkSubmissionUpsertRequest request, int userId);
        Task<ApiResponse> UpsertSubmissionAttachment(
            HomeworkSubmissionAttachmentUpsertRequest request, int userId);
        Task<ApiResponse> UpsertEvaluateHomework(
            HomeworkSubmissionEvaluateUpsertRequest request, int userId);
        Task<PagedResult<HomeworkSubmissionListDto>> GetAllHomeWorkSubmissionWithPage(SearchRequest req);
        List<HomeworkAttachmentViewModel> GetAllHomeWorkSubmissionAttechmentById(int submissionID, int userId);

        Task<List<HomeworkSubmissionDashboardDto>> GetHomeworkSubmissionDashboard(int companyId, int sessionId, int? studentId = null);
    }
}
