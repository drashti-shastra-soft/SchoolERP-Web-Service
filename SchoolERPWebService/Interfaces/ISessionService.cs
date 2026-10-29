using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing academic sessions (like '2023-2024') in the system.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Gets a list of all academic sessions registered in the system.
        /// </summary>
        List<MstSessionViewModel> GetAllSessions(bool includeDeleted = false, int? companyId = null);

        /// <summary>
        /// Finds the details of a specific session using its unique ID.
        /// </summary>
        MstSessionViewModel? GetSessionByID(int sessionId);

        /// <summary>
        /// Adds a new academic session or updates an existing one.
        /// </summary>
        (bool success, string message) UpsertSession(MstSessionUpsertRequest request, int userId, int companyId);

        /// <summary>
        /// Removes a session from the system.
        /// </summary>
        (bool success, string message) DeleteSession(List<int> sessionId, int userId);

        /// <summary>
        /// Turns a session's active status on or off.
        /// </summary>
        (bool success, string message) ToggleSessionStatus(int sessionId, bool isActive, int userId);

        /// <summary>
        /// Updates which session a specific user is currently working in (e.g., switching from last year's data to this year's).
        /// </summary>
        (bool success, string message) UpdateUserCurrentSession(int userId, int sessionId);

        /// <summary>
        /// Checks which academic session a user is currently using.
        /// </summary>
        int? GetUserCurrentSession(int userId, int? companyId = null);

        Task<PagedResult<MstSessionViewModel>> GetAllSessionWithPage(SearchRequest req);
        Task<(bool Success, string Message)> CopySession(CopyForCompanyRequest req);
    }
}
