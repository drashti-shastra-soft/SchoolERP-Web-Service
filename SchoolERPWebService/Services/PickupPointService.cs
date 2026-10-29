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
    /// This service handles the actual work of managing transport pickup points, such as saving locations where the school bus stops.
    /// </summary>
    public class PickupPointService: IPickupPointService
    {
        private readonly IConfiguration _configuration;
        public PickupPointService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all pickup points for the specified company and session.
        /// </summary>
        public List<PickupPointViewModel> GetAllPickupPoints(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<PickupPointViewModel>(
                    "sp_Mst_PickupPoint_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                return new List<PickupPointViewModel>();
            }
        }

        /// <summary>
        /// Retrieves pickup point details by pickup point ID.
        /// </summary>
        public PickupPointViewModel? GetPickupPointByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<PickupPointViewModel>(
                "sp_Mst_PickupPoint_GetByID",
                new
                {
                    PickupPointID = id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Inserts or updates a pickup point record.
        /// </summary>
        public (bool Success, string Message) UpsertPickupPoint( PickupPointUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_PickupPoint_Upsert",
                    new
                    {
                        req.PickupPointID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        req.PickupPointName,
                        req.PickupPointLatitude,
                        req.PickupPointLongitude,
                        req.IsActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return (false, "No response received from the database. Please check the stored procedure.");
                }

                return (
                    result.Result == 1,
                    result.Message ?? "Operation completed."
                );
            }
            catch (SqlException sqlEx)
            {
                // Log sqlEx somewhere (ILogger) so you can see it server-side too
                return (false, $"Database error: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Deletes a pickup point record.
        /// </summary>
        public (bool Success, string Message) DeletePickupPoint(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string pickupPointIDs = string.Join(",", id);

                var result = conn.QueryFirstOrDefault(
                    "sp_Mst_PickupPoint_Delete",
                    new
                    {
                        PickupPointID = pickupPointIDs,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure
                );

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates or deactivates a pickup point.
        /// </summary>
        //public (bool Success, string Message) TogglePickupPointStatus(int id, bool isActive, int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault(
        //            "sp_Mst_PickupPoint_ToggleStatus",
        //            new
        //            {
        //                PickupPointID = id,
        //                IsActive = isActive,
        //                UserID = userId
        //            },
        //            commandType: CommandType.StoredProcedure
        //        );

        //        return (
        //            Convert.ToInt32(result.Result) == 1,
        //            Convert.ToString(result.Message) ?? string.Empty
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}

        /// <summary>
        /// Activates or deactivates a pickup point.
        /// </summary>
        public (bool Success, string Message) TogglePickupPointStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@PICKUPPOINTID", request.Ids);   
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_PickupPoint_ToggleStatus",
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
