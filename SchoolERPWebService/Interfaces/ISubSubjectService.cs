using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ISubSubjectService
    {
        Task<PagedResult<SubSubjectViewModel>> GetAllSubSubjectWithPage(SubjectSearchRequest req);
        Task<List<DropdownModel>> GetSubSubjectBindDropdownAsync(int companyId, int sessionId);
        public SubSubjectViewModel? GetSubSubjectByID(int subSubjectID);
        public (bool success, string message) UpsertSubSubject(SubSubjectRequest request);
        public (bool success, string message) DeleteSubSubject(List<int> subSubjectID, int userId, string ipAddress);
        public (bool success, string message) ToggleSubSubjectStatus(StatusUpdateRequest request);
    }
}
