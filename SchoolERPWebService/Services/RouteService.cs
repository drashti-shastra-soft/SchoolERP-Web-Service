using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing transport routes, such as saving, updating, or deleting route records in the database.
    /// </summary>
    public class RouteService: IRouteService
    {
        private readonly IConfiguration _configuration;
        public RouteService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all transport routes for the specified company and session.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>A list of transport routes.</returns>
        public List<RouteViewModel> GetAllRoutes(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<RouteViewModel>(
                    "sp_Mst_Routes_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<RouteViewModel>();
            }
        }

        /// <summary>
        /// Retrieves a transport route by its unique identifier.
        /// </summary>
        /// <param name="id">The route identifier.</param>
        /// <returns>
        /// The matching route if found; otherwise, <c>null</c>.
        /// </returns>
        public RouteViewModel? GetRouteByID(int id)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<RouteViewModel>(
                "sp_Mst_Routes_GetByID",
                new { RouteID = id },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Creates a new transport route or updates an existing route.
        /// </summary>
        /// <param name="req">The route information to save.</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) UpsertRoute(
            RouteUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Routes_Upsert",
                    new
                    {
                        req.RouteID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        req.RouteName,
                        req.IsActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a transport route.
        /// </summary>
        /// <param name="id">The route identifier.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteRoute(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string routeIDs = string.Join(",", id);


                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Routes_Delete",
                    new
                    {
                        RouteID = routeIDs,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates or deactivates a transport route.
        /// </summary>
        /// <param name="id">The route identifier.</param>
        /// <param name="isActive">The status to apply.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        //public (bool Success, string Message) ToggleRouteStatus(int id, bool isActive, int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault<SpResult>(
        //            "sp_Mst_Routes_ToggleStatus",
        //            new
        //            {
        //                RouteID = id,
        //                IsActive = isActive,
        //                UserID = userId
        //            },
        //            commandType: CommandType.StoredProcedure);

        //        return (result?.Result == 1, result?.Message ?? string.Empty);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}


        /// <summary>
        /// Activates or deactivates a transport route.
        /// </summary>
        /// <param name="id">The route identifier.</param>
        /// <param name="isActive">The status to apply.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation's success status (<c>true</c> or <c>false</c>)
        /// and a message describing the result.
        /// </returns> 
        public (bool success, string message) ToggleRoutesStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ROUTEID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Routes_ToggleStatus",
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
