using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing transport routes.
    /// </summary>
    public interface IRouteService
    {
        /// <summary>
        /// Retrieves a list of all transport routes for a specific school and academic session.
        /// </summary>
        List<RouteViewModel> GetAllRoutes(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific route using its unique ID.
        /// </summary>
        RouteViewModel? GetRouteByID(int id);

        /// <summary>
        /// Adds a new transport route or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertRoute(RouteUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a transport route from the system.
        /// </summary>
        (bool Success, string Message) DeleteRoute(List<int> id, int userId);

        /// <summary>
        /// Turns a route's active status on or off.
        /// </summary>
        //(bool Success, string Message) ToggleRouteStatus(int id, bool isActive, int userId);
        (bool success, string message) ToggleRoutesStatus(StatusUpdateRequest request);
    }
}
