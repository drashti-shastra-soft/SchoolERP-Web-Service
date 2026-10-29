using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IClassSubSubjectService
    {
        (bool success, string message) UpsertClassSubSubject(ClassSubSubjectRequest request);
        Task<PagedResult<ClassSubSubjectViewModel>> GetAllClassSubSubjectWithPage(SubjectSearchRequest req);
        ClassSubSubjectViewModel? GetSubSubjectByID(int classSubSubjectID);
        (bool success, string message) DeleteSubSubject(List<int> classSubSubjectID, int userId, string ipAddress);
        (bool success, string message) ToggleSubSubjectStatus(StatusUpdateRequest request);
    }
}
