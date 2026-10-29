using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IManageLessonPlanService
    {
        Task<ApiResponse> Upsert(
            ManageLessonPlanRequest request,
            int userId);
        ManageLessonPlanViewModel? GetByID(int id);
        Task<PagedResult<ManageLessonPlanViewModel>> GetAllLessonPlanWithPage(ManageLessonPlanSearchRequest request);
        (bool success, string message) Delete(List<int> ids, int userId);
        Task<ApiResponse> UpsertAttachment(
            ManageLessonPlanAttachmentUpsertRequest request, int userId);

        LessonPlanViewModel? GetLessonPlanDetailById(int lessonPlanId);
        Task<ApiResponse> UpsertLessonPlanCommit(
          LessonPlanCommentRequest request,
          int userId);
        List<LessonPlanCommentResponse> GetAllCommentList(int companyId, int sessionId, int lessonPlanId);
    }
}
