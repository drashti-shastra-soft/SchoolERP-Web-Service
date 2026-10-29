using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of linking transport routes to specific pickup points and managing trip details like distance and fees.
    /// </summary>
    public class RoutePickupPointService: IRoutePickupPointService
    {
        private readonly IConfiguration _configuration;
        public RoutePickupPointService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all route-to-pickup point mappings for the specified company and session.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>A list of route-to-pickup point mappings.</returns>
        public List<RoutePickupPointViewModel> GetAllRoutePickupPoints(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<RoutePickupPointViewModel>(
                    "sp_Mst_RoutePickupPoints_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a route-to-pickup point mapping by its unique identifier.
        /// </summary>
        /// <param name="id">The route pickup point identifier.</param>
        /// <returns>
        /// The matching route pickup point mapping if found; otherwise, <c>null</c>.
        /// </returns>
        public RoutePickupPointViewModel? GetRoutePickupPointByID(int id)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<RoutePickupPointViewModel>(
                "sp_Mst_RoutePickupPoints_GetByID",
                new { RoutePickupPointID = id },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Creates a new route-to-pickup point mapping or updates an existing mapping.
        /// </summary>
        /// <param name="req">The route pickup point details.</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) UpsertRoutePickupPoint(
            RoutePickupPointUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_RoutePickupPoints_Upsert",
                    new
                    {
                        req.RoutePickupPointID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        req.RouteID,
                        req.PickupPointID,
                        req.Distance,
                        req.PickupTime,
                        req.MonthlyFees,
                        req.IsActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message ?? "No response from database");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a route pickup point mapping.
        /// </summary>
        /// <param name="id">The route pickup point identifier.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteRoutePickupPoint(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string routePickupPointIDs = string.Join(",", id);


                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_RoutePickupPoints_Delete",
                    new
                    {
                        RoutePickupPointID = routePickupPointIDs,
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
        /// Activates or deactivates a route pickup point mapping.
        /// </summary>
        /// <param name="id">The route pickup point identifier.</param>
        /// <param name="isActive">The status to apply.</param>
        /// <param name="userId">The user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        //public (bool Success, string Message) ToggleRoutePickupPointStatus(int id, bool isActive, int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault<SpResult>(
        //            "sp_Mst_RoutePickupPoints_ToggleStatus",
        //            new
        //            {
        //                RoutePickupPointID = id,
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


        public (bool success, string message) ToggleRoutePickupPointStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var ids = request.Ids
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();

                foreach (var id in ids)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@RoutePickupPointID", id);
                    parameters.Add("@IsActive", request.IsActive);
                    parameters.Add("@UserId", request.DoneBy);

                    conn.Execute(
                        "sp_Mst_RoutePickupPoints_ToggleStatus",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }

                return (true, "Status updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
