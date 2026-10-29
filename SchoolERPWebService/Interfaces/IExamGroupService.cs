using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IExamGroupService
    {
        Task<PagedResult<ExamGroupViewModel>> GetAllExamGroupWithPage(ExamListRequest req);
        Task<List<ExamGroupViewModel>> GetAllExamGroupAsync(int companyId, int sessionId, bool isActive = false);
        ExamGroupViewModel? GetExamGroupByID(int examGroupID);
        (bool success, string message) UpsertExamGroup(ExamGroupRequestModel request);
        (bool success, string message) DeleteExamGroup(List<int> examGroupIDs, int userId, string ipAddress);
        (bool success, string message) ToggleExamGroupStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyExamGroupToSession(CopyRequest req);
    }
}
