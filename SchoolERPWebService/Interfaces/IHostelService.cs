using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing hostel facilities, including room types, hostels, and individual rooms.
    /// </summary>
    public interface IHostelService
    {
        // --- Room Type ---

        /// <summary>
        /// Retrieves a list of all room categories (e.g., 'Single', 'Shared') from the database.
        /// </summary>
        List<RoomTypeViewModel> GetAllRoomTypes(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific room type using its unique ID.
        /// </summary>
        RoomTypeViewModel? GetRoomTypeByID(int id);

        /// <summary>
        /// Adds a new room type or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertRoomType(RoomTypeUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a room type from the system.
        /// </summary>
        (bool Success, string Message) DeleteRoomType(List<int> id, int userId);

        /// <summary>
        /// Turns a room type's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleRoomTypeStatus(StatusUpdateRequest request);

        // --- Hostel ---

        /// <summary>
        /// Retrieves a list of all hostel buildings from the database.
        /// </summary>
        List<HostelViewModel> GetAllHostels(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific hostel building using its unique ID.
        /// </summary>
        HostelViewModel? GetHostelByID(int id);

        /// <summary>
        /// Adds a new hostel building or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertHostel(HostelUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a hostel building from the system.
        /// </summary>
        (bool Success, string Message) DeleteHostel(List<int> id, int userId);

        /// <summary>
        /// Turns a hostel building's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleHostelStatus(StatusUpdateRequest request);

        // --- Hostel Room ---

        /// <summary>
        /// Retrieves a list of all individual rooms across all hostels from the database.
        /// </summary>
        List<HostelRoomViewModel> GetAllHostelRooms(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific hostel room using its unique ID.
        /// </summary>
        HostelRoomViewModel? GetHostelRoomByID(int id);

        /// <summary>
        /// Adds a new hostel room or updates an existing one, including details like bed count and cost.
        /// </summary>
        (bool Success, string Message) UpsertHostelRoom(HostelRoomUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a hostel room from the system.
        /// </summary>
        (bool Success, string Message) DeleteHostelRoom(List<int> id, int userId);

        /// <summary>
        /// Turns a hostel room's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleHostelRoomStatus(StatusUpdateRequest request);

        /// <summary>
        /// Retrieves a list of all WITH PAGEINDEX room categories (e.g., 'Single', 'Shared') from the database.
        /// </summary>
        Task<PagedResult<RoomTypeViewModel>> GetAllRoomTypeWithPage(ClassSearchRequest req);

        Task<(bool Success, string Message)> CopyRoomTypeToSession(CopyRequest req);

        Task<PagedResult<HostelViewModel>> GetAllHostelWithPage(HotelSearchRequest req);

        Task<(bool Success, string Message)> CopyHotelToSession(CopyRequest req);
        Task<PagedResult<HostelRoomViewModel>> GetAllHostelRoomlWithPage(HotelSearchRequest req);
        Task<List<HostelRoomRateViewModel>> GetHostelRoomRateByID(int id);
        List<RoomTypeViewModel> GetAllRoomOccupancyByRoomTypesWise(int roomTypeId);
        List<HostelSummary> GetHostelSummary(int hostelID);
        Task<List<HostelReportResponse>> GetAllHostelReportWithPage(HotelReportSearchRequest request);
        Task<(bool Success, string Message)> CopyRoomCoolingTypeToSession(CopyRequest req);
    }
}
