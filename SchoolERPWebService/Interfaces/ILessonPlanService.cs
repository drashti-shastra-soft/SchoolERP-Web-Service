using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILessonPlanService
    {
        LessonPlanModel? GetByID(int id);
        Task<ApiResponse> Upsert(
            LessonPlanModelRequest request,
            int userId);
        (bool success, string message) Delete(List<int> ids, int userId);
        (bool success, string message) ToggleStatus(StatusUpdateRequest request);
        Task<PagedResult<LessonPlanModel>> GetAllLessonPlanWithPage(LessonPlanSearchRequest req);
        List<LessonPlanModel> GetAll(int companyId, int sessionId, bool includeDeleted = false);
        List<LessonPlanMap> GetAllMapLesson(int companyId, int sessionId, int lessonId);
        Task<List<LessonDropDwonResponse>> BindLessonDropDwonList(LessonDropDwonReq req, int? userId = null);
    }
}
