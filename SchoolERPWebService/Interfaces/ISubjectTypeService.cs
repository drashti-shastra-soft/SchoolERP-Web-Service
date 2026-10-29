using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ISubjectTypeService
    {
        Task<PagedResult<SubjectTypeModel>> GetAllSubjectTypeWithPage(SubjectSearchRequest req);
        SubjectTypeModel? GetSubjectTypeByID(int subjectTypeID);
        (bool success, string message) UpsertSubject(SubjectTypeRequest request);
        (bool success, string message) DeleteSubjectType(List<int> subjectId, int userId, string ipAddress);
        (bool success, string message) ToggleSubjectTypeStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopySubjectTypeToSession(CopyRequest req);
        Task<List<SubjectTypeModel>> GetAllSubjectTypeAsync(int companyId, int sessionId, bool includeDeleted = false);
        List<DropdownModel> GetSubjectsTypeDropdownList(int companyID, int sessionID, int userId);
    }
}
