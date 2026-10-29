using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class VehicleAssignService : IVehicleAssignService
    {
        private readonly IConfiguration _configuration;
        public VehicleAssignService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ─── VEHICLE ASSIGNMENT ─────────────────────────────────

        /// <summary>
        /// Retrieves all vehicle-route assignments for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <returns>List of vehicle assignments.</returns>
        public List<VehicleAssignViewModel> GetAllAssignments(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);

                return conn.Query<VehicleAssignViewModel>(
                    "sp_Mst_VehicleAssign_GetAll",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                return new List<VehicleAssignViewModel>();
            }
        }

        /// <summary>
        /// Retrieves vehicle assignment details by its unique ID.
        /// </summary>
        /// <param name="id">Vehicle Assignment ID.</param>
        /// <returns>Vehicle assignment details if found; otherwise null.</returns>
        public VehicleAssignViewModel? GetAssignmentByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@VehicleAssignID", id);

            return conn.QueryFirstOrDefault<VehicleAssignViewModel>(
                "sp_Mst_VehicleAssign_GetByID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Assigns one or more vehicles to a route.
        /// </summary>
        /// <param name="req">Vehicle assignment information.</param>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) UpsertAssignments(
            VehicleAssignUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                int successCount = 0;
                string lastMessage = "No vehicles selected";

                foreach (var vehicleId in req.VehicleIDs)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@VehicleAssignID", 0);
                    parameters.Add("@CompanyID", companyId);
                    parameters.Add("@SessionID", sessionId);
                    parameters.Add("@RouteID", req.RouteID);
                    parameters.Add("@VehicleID", vehicleId);
                    parameters.Add("@IsActive", req.IsActive);
                    parameters.Add("@UserID", userId);

                    var result = conn.QueryFirstOrDefault<SpResult>(
                        "sp_Mst_VehicleAssign_Upsert",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    if (result != null)
                    {
                        lastMessage = result.Message;

                        if (result.Result == 1)
                            successCount++;
                    }
                }

                return successCount > 0
                    ? (true, $"{successCount} vehicle(s) assigned successfully.")
                    : (false, lastMessage);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a vehicle assignment record.
        /// </summary>
        /// <param name="id">Vehicle Assignment ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool Success, string Message) DeleteAssignment(
            List<int> id,
            int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string vehicleAssignIDs = string.Join(",", id);

                var parameters = new DynamicParameters();
                parameters.Add("@VehicleAssignID", vehicleAssignIDs);
                parameters.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_VehicleAssign_Delete",
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
        /// Activates or deactivates a vehicle assignment.
        /// </summary>
        /// <param name="id">Vehicle Assignment ID.</param>
        /// <param name="isActive">Status to set.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        //public (bool Success, string Message) ToggleAssignmentStatus(
        //    int id,
        //    bool isActive,
        //    int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var parameters = new DynamicParameters();
        //        parameters.Add("@VehicleAssignID", id);
        //        parameters.Add("@IsActive", isActive);
        //        parameters.Add("@UserID", userId);

        //        var result = conn.QueryFirstOrDefault<SpResult>(
        //            "sp_Mst_VehicleAssign_ToggleStatus",
        //            parameters,
        //            commandType: CommandType.StoredProcedure);

        //        return (
        //            result?.Result == 1,
        //            result?.Message ?? "Operation completed."
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}

        public (bool success, string message) ToggleAssignmentStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@VEHICLEASSIGNID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_VehicleAssign_ToggleStatus",
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
        /// Retrieves all vehicle-route assignments for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <returns>List of vehicle assignments.</returns>
        public async Task<StudentTransportDetailsModel?> GetTransportDetailsByStudents(
            int companyId,
            int sessionId,int studentId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@STUDENTID", studentId);

                return await conn.QueryFirstAsync<StudentTransportDetailsModel>(
                        "SP_MST_STUDENT_TRANSPORTDETAILS_GETBYSTUDENT",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
            }
            catch(Exception ex)
            {
                return new StudentTransportDetailsModel();
            }
        }
    }
}
