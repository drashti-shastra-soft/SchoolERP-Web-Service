using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IExamService
    {
        Task<PagedResult<ExamViewModel>> GetAllExamWithPage(ExamListRequest req);
        Task<List<ExamViewModel>> GetAllExamAsync(int companyId, int sessionId, bool isActive = false);
        ExamViewModel? GetExamByID(int examId);
        (bool success, string message) UpsertExam(ExamRequestModel request);
        (bool success, string message) DeleteExam(List<int> subjectId, int userId, string ipAddress);
        (bool success, string message) ToggleExamStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyExamToSession(CopyRequest req);
    }
}
