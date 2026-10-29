using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;
using System.ComponentModel.Design;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing academic sessions, such as saving session names and tracking which session a user is currently logged into in the database.
    /// </summary>
    public class SessionService: ISessionService
    {
        private readonly IConfiguration _configuration;
        public SessionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all academic sessions.
        /// </summary>
        /// <param name="includeDeleted">
        /// Indicates whether deleted sessions should be included in the result.
        /// </param>
        /// <returns>A list of academic sessions.</returns>
        public List<MstSessionViewModel> GetAllSessions(bool includeDeleted = false,int? companyId=null)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<MstSessionViewModel>(
            "sp_Sessions_GetAll",
                new { IncludeDeleted = includeDeleted, COMPANYID=companyId },
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Retrieves an academic session by its unique identifier.
        /// </summary>
        /// <param name="sessionId">The session identifier.</param>
        /// <returns>
        /// The matching session if found; otherwise, <c>null</c>.
        /// </returns>
        public MstSessionViewModel? GetSessionByID(int sessionId)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstSessionViewModel>(
                "sp_Sessions_GetByID",
                new { SessionId = sessionId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Creates a new academic session or updates an existing session.
        /// </summary>
        /// <param name="request">The session information to save.</param>
        /// <param name="userId">The identifier of the user performing the operation.</param>
        /// <param name="companyId">The identifier of the company associated with the session.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool success, string message) UpsertSession(
            MstSessionUpsertRequest request,
            int userId,
            int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Sessions_Upsert",
                    new
                    {
                        request.SessionId,
                        CompanyId = companyId,
                        request.SessionTitle,
                        request.IsActive,
                        UserId = userId
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
        /// Deletes an academic session.
        /// </summary>
        /// <param name="sessionId">The identifier of the session to delete.</param>
        /// <param name="userId">The identifier of the user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool success, string message) DeleteSession(List<int> sessionId, int userId)
        {
            try
            {

                if (sessionId == null || !sessionId.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string sessionIds = string.Join(",", sessionId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Sessions_Delete",
                    new
                    {
                        SessionId = sessionIds,
                        UserId = userId
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
        /// Activates or deactivates an academic session.
        /// </summary>
        /// <param name="sessionId">The identifier of the session.</param>
        /// <param name="isActive">The status to apply.</param>
        /// <param name="userId">The identifier of the user performing the operation.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool success, string message) ToggleSessionStatus(int sessionId, bool isActive, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Sessions_ToggleStatus",
                    new
                    {
                        SessionId = sessionId,
                        IsActive = isActive,
                        UserId = userId
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
        /// Updates the currently selected academic session for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <param name="sessionId">The identifier of the session to set as current.</param>
        /// <returns>
        /// A tuple containing the operation status and result message.
        /// </returns>
        public (bool success, string message) UpdateUserCurrentSession(int userId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Users_UpdateCurrentSession",
                    new
                    {
                        UserID = userId,
                        DoneBy = userId,
                        CurrentSessionId = sessionId
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
        /// Retrieves the currently selected academic session for a user.
        /// </summary>
        /// <param name="userId">The identifier of the user.</param>
        /// <returns>
        /// The current session identifier if found; otherwise, <c>null</c>.
        /// </returns>
        public int? GetUserCurrentSession(int userId,int? companyId=null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Users_GetCurrentSession",
                    new { UserID = userId , COMPANYID = companyId },
                    commandType: CommandType.StoredProcedure);


                return result?.SESSIONID;
            }
            catch
            {
                return null;
            }
        }


        public async Task<PagedResult<MstSessionViewModel>> GetAllSessionWithPage(SearchRequest req)
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


                param.Add("@COMPANYID", req.CompanyID);
                param.Add("@INCLUDEDELETED", 0);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);                
                param.Add("@USERID", req.UserId);

                var result = (await conn.QueryAsync<MstSessionViewModel>(
                "sp_Sessions_GetAllPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstSessionViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstSessionViewModel>
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

        public async Task<(bool Success, string Message)> CopySession(CopyForCompanyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROMCOMPANYID", req.FromCompanyId);
                parameters.Add("@TOCOMPANYID", req.ToCompanyId);
                parameters.Add("@USERID", req.UserID);
                parameters.Add("@IPADDRESS", req.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_MST_Sessions_COPYCOMPANY",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
