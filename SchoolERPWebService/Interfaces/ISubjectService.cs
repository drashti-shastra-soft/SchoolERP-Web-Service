using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing academic subjects (e.g., Mathematics, Science).
    /// </summary>
    public interface ISubjectService
    {
        /// <summary>
        /// Retrieves a list of all subjects for a specific school and academic session.
        /// </summary>
        Task<PagedResult<MstSubjectViewModel>> GetAllSubjects(SubjectSearchRequest req);

        /// <summary>
        /// Finds and returns the details of a specific subject using its unique ID.
        /// </summary>
        MstSubjectViewModel? GetSubjectByID(int subjectId);

        /// <summary>
        /// Adds a new subject or updates an existing one in the database.
        /// </summary>
        (bool success, string message) UpsertSubject(MstSubjectUpsertRequest request, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a subject from the system.
        /// </summary>
        (bool success, string message) DeleteSubject(List<int> subjectId, int userId);

        /// <summary>
        /// Turns a subject's active status on or off.
        /// </summary>
        (bool success, string message) ToggleSubjectStatus(StatusUpdateRequest request);

        Task<(bool Success, string Message)> CopySubjectToSession(CopyRequest req);

        Task<List<Dropdowbinding>> SubjectsDropdowBinding(DropdowRequest request);

        List<DropdownModel> GetSubjectGropBySubjectDropdownList(int subjectGroupId);
    }
}
