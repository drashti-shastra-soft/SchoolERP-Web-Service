using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing school classes (e.g., Grade 1, Grade 2).
    /// </summary>
    public interface IClassService
    {
        /// <summary>
        /// Retrieves a list of all classes for a specific school and academic session.
        /// </summary>
        List<MstClassViewModel> GetAllClasses(int companyId, int sessionId, bool includeDeleted = false, int? staffID = null);

        /// <summary>
        /// Finds and returns the details of a specific class using its unique ID.
        /// </summary>
        MstClassViewModel? GetClassByID(int classId);

        /// <summary>
        /// Adds a new class or updates an existing one, and links it to the selected sections.
        /// </summary>
        (bool success, string message) UpsertClass(MstClassUpsertRequest request, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a class from the system.
        /// </summary>
        (bool success, string message) DeleteClass(List<int> ids, int userId);

        /// <summary>
        /// Turns a class's active status on or off.
        /// </summary>
        (bool success, string message) ToggleClassStatus(StatusUpdateRequest request);

        Task<PagedResult<MstClassViewModel>> GetAllClassWithPage(ClassSearchRequest req);

        Task<(bool Success, string Message)> CopyClassToSession(CopyRequest req);

        Task<(bool Success, string Message)> CopyStudentHouseToSession(CopyRequest req);
    }
}
