using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing school vehicles (e.g., buses, vans).
    /// </summary>
    public interface IVehicleService
    {
        /// <summary>
        /// Retrieves a list of all vehicles for a specific school and academic session.
        /// </summary>
        List<VehicleViewModel> GetAllVehicles(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific vehicle using its unique ID.
        /// </summary>
        VehicleViewModel? GetVehicleByID(int id);

        /// <summary>
        /// Adds a new vehicle or updates an existing one, including details like driver information and capacity.
        /// </summary>
        (bool Success, string Message) UpsertVehicle(VehicleUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a vehicle from the system.
        /// </summary>
        (bool Success, string Message) DeleteVehicle(List<int> id, int userId);

        /// <summary>
        /// Turns a vehicle's active status on or off.
        /// </summary>
        //(bool Success, string Message) ToggleVehicleStatus(int id, bool isActive, int userId);
        (bool success, string message) ToggleVehicleStatus(StatusUpdateRequest request);
    }
}
