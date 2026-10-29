using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IExamConfigService
    {
        (bool success, string message) UpsertExamConfig(ExamConfigRequest request);
        (bool success, string message) DeleteExamConfig(List<int> examConfigID, int userId, string ipAddress);
        Task<(bool Success, string Message)> CopyExamConfigToSession(ExamConfigCopyRequest request);
        Task<List<ExamConfigViewModel>> GetAllExamConfigByClassIdAsync(int companyId, int sessionId, int classID);
        ExamConfigViewModel? GetExamConfigByID(int examConfigId);
        Task<PagedResult<ExamConfigViewModel>> GetAllExamConfigWithPage(ExamConfigListRequest request);
        (bool success, string message) ToggleExamStatus(StatusUpdateRequest request);
        Task<List<SubjectWithPaperMarksResponse>> GetAllExamSubjectPaperMarksListAsyc(int companyId,
            int sessionId, int classID, int sectionId, int examId);
        Task<List<SubjectWithPaperMarksResponse>> GetAllExamBySubjectIdListAsyc(int companyId,
            int sessionId, int classID, int subjectID);
    }
}
