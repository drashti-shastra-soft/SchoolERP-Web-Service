using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class ExamGroupService: IExamGroupService
    {
        private readonly IConfiguration _configuration;
        public ExamGroupService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PagedResult<ExamGroupViewModel>> GetAllExamGroupWithPage(ExamListRequest req)
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<ExamGroupViewModel>(
                "SP_EXAMGROUP_GETPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<ExamGroupViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ExamGroupViewModel>
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

        public async Task<List<ExamGroupViewModel>> GetAllExamGroupAsync(int companyId, int sessionId, bool isActive = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@IsActive", null);
                parameters.Add("@SearchKeyword", null);
                parameters.Add("@IncludeDeleted", null);

                var result = conn.Query<ExamGroupViewModel>(
                    "SP_EXAMGROUP_GETALL",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ExamGroupViewModel? GetExamGroupByID(int examGroupID)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<ExamGroupViewModel>(
                    "SP_EXAMGROUP_GETBYID",
                    new
                    {
                        EXAMGROUPID = examGroupID
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) UpsertExamGroup(ExamGroupRequestModel request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@EXAMGROUPID", request.ExamGroupID, DbType.Int32);
                parameters.Add("@COMPANYID", request.CompanyID, DbType.Int32);
                parameters.Add("@SESSIONID", request.SessionID, DbType.Int32);
                parameters.Add("@EXAMGROUPNAME", request.ExamGroupName, DbType.String);
                parameters.Add("@EXAMGROUPSHORTCODE", request.ExamGroupShortCode, DbType.String);
                parameters.Add("@DisplayLabel", request.DisplayLabel, DbType.String);
                parameters.Add("@DESCRIPTION", request.Description, DbType.String);
                parameters.Add("@ISACTIVE", request.IsActive, DbType.Boolean);
                parameters.Add("@USERID", request.UserID, DbType.Int32);
                parameters.Add("@IPADDRESS", request.IPAddress, DbType.String);



                var result = conn.QueryFirstOrDefault<SpResult>(
               "SP_EXAMGROUP_UPSERT",
               parameters,
               commandType: CommandType.StoredProcedure);



                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? ""
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool success, string message) DeleteExamGroup(List<int> examGroupIDs, int userId, string ipAddress)
        {
            try
            {
                if (examGroupIDs == null || !examGroupIDs.Any())
                {
                    return (false, "No exam group selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string examGroupID = string.Join(",", examGroupIDs);

                var parameters = new DynamicParameters();
                parameters.Add("@ExamGroupID", examGroupID);
                parameters.Add("@UserId", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_ExamGroup_Delete",
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

        public (bool success, string message) ToggleExamGroupStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ExamGroupID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_ExamGroup_ToggleStatus",
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

        public async Task<(bool Success, string Message)> CopyExamGroupToSession(CopyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FromCompanyID", req.FromCompanyId);
                parameters.Add("@FromSessionID", req.FromSessionId);
                parameters.Add("@ToCompanyID", req.ToCompanyId);
                parameters.Add("@ToSessionID", req.ToSessionId);
                parameters.Add("@UserId", req.UserID);
                parameters.Add("@IPAddress", null);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_ExamGroup_Copy",
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
