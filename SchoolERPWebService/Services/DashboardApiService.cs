using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class DashboardApiService: IDashboardApiService
    {
        private readonly IConfiguration _configuration;
        public DashboardApiService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // ── HELPER ────────────────────────────────────────────────
        private IDbConnection CreateConnection() => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public async Task<List<DashboardModel>> GetAllAsync()
        {
            using var con = CreateConnection();

            var rows = await con.QueryAsync<DashboardModel>(
                "SP_Dashboard_GetAll",
                commandType: CommandType.StoredProcedure
            );

            var list = rows.AsList();

            // If SP returned RESULT = 0 (validation failed / no data)
            // the row will have DashboardID = 0 and Result = 0
            // → return empty list, don't treat it as real data
            if (!list.Any() || list.First().Result == 0)
                return new List<DashboardModel>();

            return list;
        }

        // ─────────────────────────────────────────────────────────
        // GET BY ID
        // ─────────────────────────────────────────────────────────
        public async Task<DashboardModel?> GetByIdAsync(int dashboardId)
        {
            using var con = CreateConnection();

            return await con.QueryFirstOrDefaultAsync<DashboardModel>(
                "SP_Dashboard_GetById",
                new { DASHBOARDID = dashboardId },
                commandType: CommandType.StoredProcedure
            );
        }

        // ─────────────────────────────────────────────────────────
        // INSERT / UPDATE
        // ─────────────────────────────────────────────────────────
        public async Task<(bool Success, string Message, int DashboardID)> InsertUpdateAsync(
            DashboardRequestModel request, int userId, string ipAddress)
        {
            using var con = CreateConnection();

            var result = await con.QueryFirstOrDefaultAsync<SpResultdas>(
                "SP_DASHBOARD_INSERTUPDATE",
                new
                {
                    DASHBOARDID = request.DashboardID,
                    DASHBOARDTITAL = request.DashboardTital,
                    DASHBOARDURL = request.DashboardURL,
                    ROLEID = request.RoleId,
                    ISACTIVE = request.IsActive,
                    USERID = userId,
                    IPADDRESS = ipAddress
                },
                commandType: CommandType.StoredProcedure
            );

            if (result == null)
                return (false, "No response from server.", 0);

            return (result.RESULT == 1, result.MESSAGE, result.DASHBOARDID ?? 0);
        }

        // ─────────────────────────────────────────────────────────
        // TOGGLE STATUS
        // ─────────────────────────────────────────────────────────
        public async Task<(bool Success, string Message)> ToggleStatusAsync(StatusUpdateRequest request)
        {
            using var con = CreateConnection();

            var result = await con.QueryFirstOrDefaultAsync<SpResultdas>(
                "SP_Dashboard_ToggleStatus",
                new
                {
                    DASHBOARDID = request.Ids,
                    ISACTIVE = request.IsActive,
                    USERID = request.DoneBy,
                    IPADDRESS = request.IpAddress
                },
                commandType: CommandType.StoredProcedure
            );

            if (result == null)
                return (false, "No response from server.");

            return (result.RESULT == 1, result.MESSAGE);
        }

        // ─────────────────────────────────────────────────────────
        // DELETE MULTIPLE
        // ─────────────────────────────────────────────────────────
        public async Task<(bool Success, string Message)> DeleteMultipleAsync(
            List<int> dashboardIds, int userId, string ipAddress)
        {
            try
            {
                using var con = CreateConnection();
                if (dashboardIds == null || !dashboardIds.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string dashboardId = string.Join(",", dashboardIds);

                // Pass as comma-separated string; SP splits internally
                var result = await con.QueryFirstOrDefaultAsync<SpResultdas>(
                    "SP_Dashboard_Delete",
                    new
                    {
                        DASHBOARDID = string.Join(",", dashboardId),
                        USERID = userId,
                        IPADDRESS = ipAddress
                    },
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                    return (false, "No response from server.");

                return (result.RESULT == 1, result.MESSAGE);
            }
            catch (Exception ex)
            {
                var result = new SpResultdas();
                return (result.RESULT == 0, result.MESSAGE= ex.Message); 
            }
            
        }


        // ─────────────────────────────────────────────────────────
        // GET BY ID
        // ─────────────────────────────────────────────────────────
        public async Task<DashboardModel?> GetByRoleIdAsync(int roleId)
        {
            using var con = CreateConnection();

            return await con.QueryFirstOrDefaultAsync<DashboardModel>(
                "SP_Dashboard_GetByRoleId",
                new { RoleId = roleId },
                commandType: CommandType.StoredProcedure
            );
        }

        // ─────────────────────────────────────────────────────────
        //  PRIVATE  –  SP result row mapper
        // ─────────────────────────────────────────────────────────
        private class SpResultdas
        {
            public int RESULT { get; set; }
            public string MESSAGE { get; set; } = string.Empty;
            public int? DASHBOARDID { get; set; }   // only present on InsertUpdate
        }


        // ─────────────────────────────────────────────────────────
        // GET BY ID
        // ─────────────────────────────────────────────────────────
        public async Task<AttendanceCountModel> GetStudentAttendanceCount(ReqAttendanceCountModel req)
        {
            try
            {
                using var con = CreateConnection();
                var result = await con.QueryFirstOrDefaultAsync<AttendanceCountModel>(
                "SP_StudentAttendance_COUNT_GET",
                new
                {
                    COMPANYID = req.CompanyId,
                    SESSIONID = req.SessionId,
                    AttendanceStatus = req.AttendanceStatus,
                    ATTENDANCEDATE = req.AttendanceDate.Date,
                    USERID = req.UserId
                },
                commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public async Task<List<UserRoleWiseCountModel>> GetUserRoleWiseCount(int companyId,int sessionId,int userId)
        {
            try
            {
                using var con = CreateConnection();
                //var result = await con.QueryAsync<List<UserRoleWiseCountModel>>(
                //"SP_UserRoleWise_COUNT_GET",
                //new
                //{
                //    COMPANYID = companyId,
                //    SESSIONID = sessionId,
                //    USERID = userId
                //},

                var rows = await con.QueryAsync<UserRoleWiseCountModel>(
                "SP_UserRoleWise_COUNT_GET",
                new
                {
                    COMPANYID = companyId,
                    SESSIONID = sessionId,
                    USERID = userId
                },
                commandType: CommandType.StoredProcedure
           );


                return rows.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
