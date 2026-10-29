using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing hostel facilities, such as saving, updating, or deleting hostel buildings and room records in the database.
    /// </summary>
    public class HostelService: IHostelService
    {
        private readonly IConfiguration _configuration;
        public HostelService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ─── ROOM TYPE ──────────────────────────────────────────

        /// <summary>
        /// Retrieves all room types for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="includeDeleted">Whether to include deleted records.</param>
        /// <returns>List of room types.</returns>
        public List<RoomTypeViewModel> GetAllRoomTypes(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@IncludeDeleted", includeDeleted);

                var result = conn.Query<RoomTypeViewModel>(
                    "sp_Mst_RoomType_GetAll",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// Retrieves room type details by its unique ID.
        /// </summary>
        /// <param name="id">Room Type ID.</param>
        /// <returns>Room type details if found; otherwise null.</returns>
        public RoomTypeViewModel? GetRoomTypeByID(int id)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@RoomTypeID", id);

            var result= conn.QueryFirstOrDefault<RoomTypeViewModel>(
                "sp_Mst_RoomType_GetByID",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // If SP returned no rows at all
            if (result == null) return null;

            // If SP returned rows but RESULT != 1 (failure case)
            if (result.Result != 1) return null;

            return result;
        }

        /// <summary>
        /// Creates a new room type or updates an existing room type.
        /// </summary>
        /// <param name="req">Room type information.</param>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) UpsertRoomType(
            RoomTypeUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@RoomTypeID", req.RoomTypeID);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@RoomTypeTitle", req.RoomTypeTitle);
                parameters.Add("@DisplayLabel", req.DisplayLabel);
                parameters.Add("@RoomTypeDescription", req.RoomTypeDescription);
                parameters.Add("@BedCapacity", req.BedCapacity);
                parameters.Add("@RoomCoolingTypeId", req.RoomCoolingTypeId);
                parameters.Add("@IsActive", req.IsActive);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpRoomTypeResult>(
                    "sp_Mst_RoomType_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );

                //Rate
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a room type record by its unique ID.
        /// </summary>
        /// <param name="id">Room Type ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) DeleteRoomType(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string roomTypeIDs = string.Join(",", id);

                var parameters = new DynamicParameters();
                parameters.Add("@RoomTypeID", roomTypeIDs);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Mst_RoomType_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates or deactivates a room type.
        /// </summary>
        /// <param name="id">Room Type ID.</param>
        /// <param name="isActive">Status to set.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) ToggleRoomTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@RoomTypeID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_RoomType_ToggleStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        /// <summary>
        /// Retrieves all room occupancy types for the specified Room Type wise like AC,Non/AC etc.
        /// </summary>
        public List<RoomTypeViewModel> GetAllRoomOccupancyByRoomTypesWise(int roomTypeId)
        {
            try
            {
                using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@RoomTypeId", roomTypeId);

                var result = conn.Query<RoomTypeViewModel>(
                    "SP_MST_ROOMOCCUPANCYTYPEROOMTYPEWISE_GETALL",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        // ─── HOSTEL ─────────────────────────────────────────────

        /// <summary>
        /// Retrieves all hostels for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="includeDeleted">Whether to include deleted records.</param>
        /// <returns>List of hostels.</returns>
        public List<HostelViewModel> GetAllHostels(int companyId, int sessionId, bool includeDeleted = false)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionID", sessionId);
            parameters.Add("@IncludeDeleted", includeDeleted);

            var result= conn.Query<HostelViewModel>(
                "sp_Mst_Hostel_GetAll",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            // If SP returned no rows at all
            if (!result.Any()) return null;

            // If SP returned rows but RESULT != 1 (failure case)
            if (result.First().Result != 1) return null;

            return result;
        }

        /// <summary>
        /// Retrieves hostel details by its unique ID.
        /// </summary>
        /// <param name="id">Hostel ID.</param>
        /// <returns>Hostel details if found; otherwise null.</returns>
        public HostelViewModel? GetHostelByID(int id)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@HostelID", id);

            return conn.QueryFirstOrDefault<HostelViewModel>(
                "sp_Mst_Hostel_GetByID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        // ─── HOSTEL ─────────────────────────────────────────────

        /// <summary>
        /// Creates a new hostel or updates an existing hostel record.
        /// </summary>
        /// <param name="req">Hostel information to save.</param>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) UpsertHostel(
            HostelUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@HostelID", req.HostelID);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@HostelName", req.HostelName);                
                parameters.Add("@DisplayLabel", req.DisplayLabel);                
                parameters.Add("@RoomTypeID", req.RoomTypeID);
                parameters.Add("@HostelAddress", req.HostelAddress);
                parameters.Add("@HostelIntake", req.HostelIntake);
                parameters.Add("@HostelDescription", req.HostelDescription);
                parameters.Add("@IsActive", req.IsActive);
                parameters.Add("@UserId", userId);
                parameters.Add("@HostelCode", req.HostelCode);
                parameters.Add("@HostelTypeID", req.HostelTypeID);
                parameters.Add("@PostalCode", req.PostalCode);
                parameters.Add("@CountryId", req.CountryId);
                parameters.Add("@StateId", req.StateId);
                parameters.Add("@CityId", req.CityId);
                parameters.Add("@HostelRules", req.HostelRules);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Hostel_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a hostel record by its unique ID.
        /// </summary>
        /// <param name="id">Hostel ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) DeleteHostel(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string hostelIDs = string.Join(",", id);

                var parameters = new DynamicParameters();
                parameters.Add("@HostelID", hostelIDs);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Hostel_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                   result?.Result == 1,
                   result?.Message ?? "Operation completed."
               );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates or deactivates a hostel record.
        /// </summary>
        /// <param name="id">Hostel ID.</param>
        /// <param name="isActive">Status to set.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) ToggleHostelStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@HostelID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Hostel_ToggleStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // ─── HOSTEL ROOM ────────────────────────────────────────

        /// <summary>
        /// Retrieves all hostel rooms for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="includeDeleted">Whether to include deleted records.</param>
        /// <returns>List of hostel rooms.</returns>
        public List<HostelRoomViewModel> GetAllHostelRooms(
            int companyId,
            int sessionId,
            bool includeDeleted = false)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionID", sessionId);
            parameters.Add("@IncludeDeleted", includeDeleted);

            var result= conn.Query<HostelRoomViewModel>(
                "sp_Mst_HostelRoom_GetAll",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            // If SP returned no rows at all
            if (!result.Any()) return null;

            // If SP returned rows but RESULT != 1 (failure case)
            if (result.First().Result != 1) return null;

            return result;
        }

        /// <summary>
        /// Retrieves hostel room details by its unique ID.
        /// </summary>
        /// <param name="id">Room ID.</param>
        /// <returns>Hostel room details if found; otherwise null.</returns>
        public HostelRoomViewModel? GetHostelRoomByID(int id)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@RoomId", id);

            return conn.QueryFirstOrDefault<HostelRoomViewModel>(
                "sp_Mst_HostelRoom_GetByID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Creates a new hostel room or updates an existing hostel room.
        /// </summary>
        /// <param name="req">Hostel room information.</param>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) UpsertHostelRoom(
            HostelRoomUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@RoomId", req.RoomId);
                parameters.Add("@HostelID", req.HostelID);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@RoomTypeID", req.RoomCoolingTypeId);
                parameters.Add("@RoomTitle", req.RoomTitle);
                parameters.Add("@NoOfBed", req.NoOfBed);
                parameters.Add("@CostPerBed", req.CostPerBed);
                parameters.Add("@RoomDescription", req.RoomDescription);
                parameters.Add("@IsActive", req.IsActive);
                parameters.Add("@UserId", userId);
                parameters.Add("@FloorNumber", req.FloorNumber);
                parameters.Add("@AllowExtraBed", req.AllowExtraBed);
                parameters.Add("@MaxExtraBeds", req.MaxExtraBeds);
                
                var result = conn.QueryFirstOrDefault<SpHostelRoomResult>(
                    "sp_Mst_HostelRoom_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);


                if (result?.Result == 1)
                {
                    if (result?.ROOMID != null && result?.ROOMID != 0)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("@RateID", 0);
                        parameters.Add("@RoomId", result?.ROOMID);
                        parameters.Add("@RoomTypeID", req?.RoomTypeID);
                        parameters.Add("@HostelID", req?.HostelID);
                        parameters.Add("@RoomCoolingTypeId", req?.RoomCoolingTypeId);
                        parameters.Add("@CostPerBed", req.CostPerBed);
                        parameters.Add("@NoOfBeds", req.NoOfBed);
                        parameters.Add("@SecurityAmount", req.SecurityAmount);
                        parameters.Add("@EffectiveFrom", req.EffectiveFrom);
                        parameters.Add("@CompanyID", companyId);
                        parameters.Add("@SessionID", sessionId);
                        parameters.Add("@IsActive", req.IsActive);
                        parameters.Add("@UserId", userId);

                        var roomresult = conn.QueryFirstOrDefault<SpRoomTypeResult>(
                            "SP_MST_HOSTELROOMRATE_UPSERT",
                            parameters,
                            commandType: CommandType.StoredProcedure);

                    }
                }

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a hostel room record by its unique ID.
        /// </summary>
        /// <param name="id">Room ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) DeleteHostelRoom(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string roomIds = string.Join(",", id);

                var parameters = new DynamicParameters();
                parameters.Add("@RoomId", roomIds);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_HostelRoom_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                   result?.Result == 1,
                   result?.Message ?? "Operation completed."
               );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates or deactivates a hostel room.
        /// </summary>
        /// <param name="id">Room ID.</param>
        /// <param name="isActive">Status to set.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) ToggleHostelRoomStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@RoomId", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_HostelRoom_ToggleStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                   result?.Result == 1,
                   result?.Message ?? "Operation completed."
               );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        
        public async Task<PagedResult<RoomTypeViewModel>> GetAllRoomTypeWithPage(ClassSearchRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }


                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<RoomTypeViewModel>(
                "SP_MST_ROOMTYPE_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<RoomTypeViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<RoomTypeViewModel>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return userModel;

            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public async Task<(bool Success, string Message)> CopyRoomTypeToSession(CopyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROMCOMPANYID", req.FromCompanyId);
                parameters.Add("@FROMSESSIONID", req.FromSessionId);
                parameters.Add("@TOCOMPANYID", req.ToCompanyId);
                parameters.Add("@TOSESSIONID", req.ToSessionId);
                parameters.Add("@USERID", req.UserID);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_MST_ROOMTYPE_COPYSESSION",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                SpResult model = new SpResult();
                model.Result = 0;
                model.Message = ex.Message;
                return (
                    Convert.ToInt32(model.Result) == 1,
                    Convert.ToString(model.Message) ?? string.Empty
                );
            }

        }

        public async Task<PagedResult<HostelViewModel>> GetAllHostelWithPage(HotelSearchRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }


                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@ROOMTYPEID ", req.RoomTypeID);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HostelViewModel>(
                "SP_MST_HOSTEL_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HostelViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HostelViewModel>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return userModel;

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<(bool Success, string Message)> CopyHotelToSession(CopyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROMCOMPANYID", req.FromCompanyId);
                parameters.Add("@FROMSESSIONID", req.FromSessionId);
                parameters.Add("@TOCOMPANYID", req.ToCompanyId);
                parameters.Add("@TOSESSIONID", req.ToSessionId);
                parameters.Add("@USERID", req.UserID);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_MST_HOSTEL_COPYSESSION",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                SpResult model = new SpResult();
                model.Result = 0;
                model.Message = ex.Message;
                return (
                    Convert.ToInt32(model.Result) == 1,
                    Convert.ToString(model.Message) ?? string.Empty
                );
            }

        }

        public async Task<PagedResult<HostelRoomViewModel>> GetAllHostelRoomlWithPage(HotelSearchRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }


                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@HOSTELID ", req.HostelID);
                param.Add("@ROOMTYPEID ", req.RoomTypeID);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HostelRoomViewModel>(
                "SP_MST_HOSTELROOM_GETALLPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HostelRoomViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HostelRoomViewModel>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return userModel;

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<HostelRoomRateViewModel>> GetHostelRoomRateByID(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ROOMTYPEID", id);

                var result = (await conn.QueryAsync<HostelRoomRateViewModel>(
                "SP_tbl_Mst_Map_HostelRoom_Rate_GETRoomTypeBYID",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public List<HostelSummary> GetHostelSummary(int hostelID)
        {
            try
            {
                using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@HostelID", hostelID);

                var result = conn.Query<HostelSummary>(
                    "SP_tbl_Mst_Hostel_GetHostelSummary",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        
        public async Task<List<HostelReportResponse>> GetAllHostelReportWithPage(HotelReportSearchRequest request)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var param = new DynamicParameters();

            param.Add("@Mode", string.IsNullOrWhiteSpace(request.Mode) ? "REPORT" : request.Mode);
            param.Add("@CompanyID", request.CompanyID);
            param.Add("@SessionID", request.SessionID);

            param.Add("@HostelID", request.HostelID);
            param.Add("@HostelTypeID", request.HostelTypeID);
            param.Add("@RoomTypeID", request.RoomTypeID);
            param.Add("@RoomCoolingTypeId", request.RoomCoolingTypeId);
            param.Add("@RoomTitle", request.RoomTitle);
            param.Add("@SearchText", request.SearchText);
            param.Add("@IsActive", request.IsActive);

            param.Add("@AsOnDate", request.AsOnDate);
            param.Add("@RoomId", request.RoomId);

            param.Add("@PageNumber", request.PageNumber <= 0 ? 1 : request.PageNumber);
            param.Add("@PageSize", request.PageSize <= 0 ? 10 : request.PageSize);

            await conn.OpenAsync();

            var result = await conn.QueryAsync<HostelReportResponse>(
                "usp_Hostel_Manage",
                param,
                commandType: CommandType.StoredProcedure,
                commandTimeout: 60);

            return result.ToList();
        }

        public async Task<(bool Success, string Message)> CopyRoomCoolingTypeToSession(CopyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROMCOMPANYID", req.FromCompanyId);
                parameters.Add("@FROMSESSIONID", req.FromSessionId);
                parameters.Add("@TOCOMPANYID", req.ToCompanyId);
                parameters.Add("@TOSESSIONID", req.ToSessionId);
                parameters.Add("@USERID", req.UserID);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_MST_ROOMCOOLINGTYPE_COPYSESSION",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                SpResult model = new SpResult();
                model.Result = 0;
                model.Message = ex.Message;
                return (
                    Convert.ToInt32(model.Result) == 1,
                    Convert.ToString(model.Message) ?? string.Empty
                );
            }

        }
    }
}
