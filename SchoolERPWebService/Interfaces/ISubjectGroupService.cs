using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing subject groups, which link subjects to specific classes and sections.
    /// </summary>
    public interface ISubjectGroupService
    {
        /// <summary>
        /// Retrieves a list of all subject groups for a specific school and academic session.
        /// </summary>
        List<MstSubjectGroupViewModel> GetAll(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific subject group using its unique ID.
        /// </summary>
        MstSubjectGroupViewModel? GetByID(int id);

        /// <summary>
        /// Adds a new subject group or updates an existing one, linking it to classes, sections, and subjects.
        /// </summary>
        (bool success, string message) Upsert(MstSubjectGroupUpsertRequest request, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a subject group from the system.
        /// </summary>
        (bool success, string message) Delete(List<int> ids, int userId);

        /// <summary>
        /// Turns a subject group's active status on or off.
        /// </summary>
        (bool success, string message) ToggleStatus(StatusUpdateRequest request);
        List<DropdownModel> GetAllSubjectByClassandSectionId(int companyId, int sessionId, int classId, int sectionId);

        Task<PagedResult<MstSubjectGroupViewModel>> GetAllSubjectGroupWithPage(SubjectGroupSearchRequest req);
    }
}
