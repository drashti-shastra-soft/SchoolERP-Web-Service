using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ITopicService
    {
        TopicViewModel? GetByID(int id);
        Task<ApiResponse> Upsert(
            TopicModelRequest request,
            int userId);
        (bool success, string message) Delete(List<int> ids, int userId);
        (bool success, string message) ToggleStatus(StatusUpdateRequest request);
        Task<PagedResult<TopicViewModel>> GetAllTopicWithPage(TopicSearchRequest req);
        List<TopicViewModel> GetAll(int companyId, int sessionId, int userID, bool includeDeleted = false);
        List<TopicMap> GetAllMapTopic(int companyId, int sessionId, int topicId);
        Task<PagedResult<LessonSyllabusStatusResponse>> GetAllTopicSyllaBussStatusWithPage(TopicSearchRequest req);
        (bool success, string message) ToggleTopicCompleteStatus(StatusUpdateRequest request);
        Task<List<TopicDropDwonResponse>> BindTopicDropDwonList(int lessonMapId);
    }
}
