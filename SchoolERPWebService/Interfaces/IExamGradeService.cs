using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IExamGradeService
    {
        Task<List<ExamGradeViewModel>> GetByFilterAsync(int companyId, int sessionId,
            int subjectTypeID, int classID);
        Task<ExamGradeResponse?> UpsertExamGradeAsync(
   ExamGradeRequest request);
        Task<ExamGradeCopyResponse?> CopyExamGradeAsync(
    ExamGradeCopyRequest request);
        Task<List<DropdownModel>> GetExamGradeByClassListAsync(int companyId, int sessionId, int classId);
    }
}
