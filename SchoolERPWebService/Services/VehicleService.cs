using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing school vehicles, such as saving, updating, or deleting vehicle records in the database.
    /// </summary>
    public class VehicleService: IVehicleService
    {
        private readonly IConfiguration _configuration;
        public VehicleService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all vehicle records for the specified company and session.
        /// </summary>
        public List<VehicleViewModel> GetAllVehicles(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<VehicleViewModel>(
                    "sp_Mst_Vehicles_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                return new List<VehicleViewModel>();
            }
        }

        /// <summary>
        /// Looks up the details of a specific vehicle using its unique ID.
        /// </summary>
        public VehicleViewModel? GetVehicleByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<VehicleViewModel>(
                "sp_Mst_Vehicles_GetByID",
                new
                {
                    VehicleID = id
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>   
        /// Saves a new vehicle or updates an existing vehicle record.
        /// </summary>

        public (bool Success, string Message) UpsertVehicle(VehicleUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Vehicles_Upsert",
                    new
                    {
                        req.VehicleID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        req.VehicleNumber,
                        req.VehicleModel,
                        req.VehicleYearMade,
                        req.VehicleRegNo,
                        req.VehicleChasisNo,
                        req.VehicleMaxCapicity,
                        req.VehicleDriverName,
                        req.VehicleDriverLicense,
                        req.VehicleDriverContact,

                        // Driver Photo
                        req.VehicleDriverPhotoAttach,
                        req.VehicleDriverPhotoName,
                        req.VehicleDriverPhotoType,

                        req.VehicleNote,
                        req.IsActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return (false, "No response received from the database.");
                }

                return (
                    result.Result == 1,
                    result.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        //public (bool Success, string Message) UpsertVehicle(VehicleUpsertRequest req, int companyId, int sessionId, int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault<SpResult>(
        //            "sp_Mst_Vehicles_Upsert",
        //            new
        //            {
        //                req.VehicleID,
        //                CompanyID = companyId,
        //                SessionID = sessionId,
        //                req.VehicleNumber,
        //                req.VehicleModel,
        //                req.VehicleYearMade,
        //                req.VehicleRegNo,
        //                req.VehicleChasisNo,
        //                req.VehicleMaxCapicity,
        //                req.VehicleDriverName,
        //                req.VehicleDriverLicense,
        //                req.VehicleDriverContact,
        //                req.VehicleDriverPhotoAttach,
        //                req.VehicleDriverPhotoName,
        //                req.VehicleDriverPhotoType,
        //                req.VehicleNote,
        //                req.IsActive,
        //                UserID = userId
        //            },
        //            commandType: CommandType.StoredProcedure);

        //        if (result == null)
        //        {
        //            return (false, "No response received from the database.");
        //        }

        //        return (result.Result == 1, result.Message ?? "Operation completed.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}

        //public async Task<(bool Success, string Message)> UpsertVehicleAsync( VehicleUpsertRequest req, int companyId, int sessionId, int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var parameters = new DynamicParameters();
        //        parameters.Add("VehicleID", req.VehicleID);
        //        parameters.Add("CompanyID", companyId);
        //        parameters.Add("SessionID", sessionId);
        //        parameters.Add("VehicleNumber", req.VehicleNumber, DbType.String, size: 50);
        //        parameters.Add("VehicleModel", req.VehicleModel, DbType.String, size: 100);
        //        parameters.Add("VehicleYearMade", req.VehicleYearMade, DbType.String, size: 10);
        //        parameters.Add("VehicleRegNo", req.VehicleRegNo, DbType.String, size: 50);
        //        parameters.Add("VehicleChasisNo", req.VehicleChasisNo, DbType.String, size: 50);
        //        parameters.Add("VehicleMaxCapicity", req.VehicleMaxCapicity);
        //        parameters.Add("VehicleDriverName", req.VehicleDriverName, DbType.String, size: 200);
        //        parameters.Add("VehicleDriverLicense", req.VehicleDriverLicense, DbType.String, size: 100);
        //        parameters.Add("VehicleDriverContact", req.VehicleDriverContact, DbType.String, size: 20);
        //        parameters.Add("VehicleDriverPhotoAttach", req.VehicleDriverPhotoAttach, DbType.Binary);
        //        parameters.Add("VehicleDriverPhotoName", req.VehicleDriverPhotoName, DbType.String, size: 200);
        //        parameters.Add("VehicleDriverPhotoType", req.VehicleDriverPhotoType, DbType.String, size: 50);
        //        parameters.Add("VehicleNote", req.VehicleNote, DbType.String);
        //        parameters.Add("IsActive", req.IsActive);
        //        parameters.Add("UserID", userId);

        //        var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
        //            "sp_Mst_Vehicles_Upsert",
        //            parameters,
        //            commandType: CommandType.StoredProcedure,
        //            commandTimeout: 30);

        //        if (result == null)
        //        {
        //            return (false, "No response received from the database.");
        //        }

        //        return (result.Result == 1, result.Message ?? "Operation completed.");
        //    }
        //    catch (SqlException sqlEx) when (sqlEx.Number is 2627 or 2601)
        //    {
        //        return (false, "A vehicle with this Vehicle Number already exists.");
        //    }
        //    catch (Exception ex)
        //    {
        //        //LogError(ex, "UpsertVehicle failed for CompanyID={CompanyId}", companyId);
        //        return (false, "An error occurred while saving the vehicle. Please try again.");
        //    }
        //}
        /// <summary>
        /// Deletes a vehicle record from the system.
        /// </summary>
        /// 
        public (bool Success, string Message) DeleteVehicle(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No vehicles selected for deletion."); // fixed message
                }
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                string vehicleIDs = string.Join(",", id);
                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Mst_Vehicles_Delete",
                    new
                    {
                        VehicleID = vehicleIDs,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);
                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        //public (bool Success, string Message) DeleteVehicle(List<int> id, int userId)
        //{
        //    try
        //    {
        //        if (id == null || !id.Any())
        //        {
        //            return (false, "No students selected for deletion.");
        //        }

        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        string vehicleIDs = string.Join(",", id);

        //        var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
        //            "sp_Mst_Vehicles_Delete",
        //            new
        //            {
        //                VehicleID = vehicleIDs,
        //                UserID = userId
        //            },
        //            commandType: CommandType.StoredProcedure);

        //        return (result.Result == 1, result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}
        /// <summary>
        /// Updates the active or inactive status of a vehicle.
        /// </summary>
        //public (bool Success, string Message) ToggleVehicleStatus(
        //    int id,
        //    bool isActive,
        //    int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
        //            "sp_Mst_Vehicles_ToggleStatus",
        //            new
        //            {
        //                VehicleID = id,
        //                IsActive = isActive,
        //                UserID = userId
        //            },
        //            commandType: CommandType.StoredProcedure);

        //        return (result.Result == 1, result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}

        /// <summary>
        /// Activates or deactivates one or more transport vehicles.
        /// </summary>
        /// <param name="request">
        /// The request containing the vehicle identifier(s) (<see cref="StatusUpdateRequest.Ids"/>),
        /// the desired status (<see cref="StatusUpdateRequest.IsActive"/>), and the user performing
        /// the operation (<see cref="StatusUpdateRequest.DoneBy"/>).
        /// </param>
        /// <returns>
        /// A tuple containing:
        /// <c>success</c> — <c>true</c> if the status update completed successfully; otherwise <c>false</c>.
        /// <c>message</c> — a message describing the result of the operation, or the exception message if an error occurred.
        /// </returns>
        public (bool success, string message) ToggleVehicleStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@VEHICLEID", request.Ids);   // fixed typo: single @
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Vehicles_ToggleStatus",
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

    }
}
