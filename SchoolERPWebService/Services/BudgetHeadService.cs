using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class BudgetHeadService: IBudgetHeadService
    {
        private readonly IConfiguration _configuration;
        public BudgetHeadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ApiResponse> UpsertBudgetHeadAsync(BudgetHeadModelRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@BudgetHeadId", model.BudgetHeadId);
                parameters.Add("@CompanyID", model.CompanyID);
                parameters.Add("@SessionID", model.SessionID);
                parameters.Add("@BudgetName", model.BudgetName);
                parameters.Add("@DisplayLabel", model.DisplayLabel);
                parameters.Add("@BudgetLimit", model.BudgetLimit);
                parameters.Add("@IsActive", model.IsActive);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@IPAddress", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_BudgetHead_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.Result;
                    response.Message = result.Message;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                }
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save cost center. Please try again.";
                response.TechnicalMessage = ex.Message;
            }

            return response;
        }

        public async Task<PagedResult<BudgetHeadModel>> GetAllBudgetHeadWithPage(SearchRequest req)
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
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<BudgetHeadModel>(
                "SP_BudgetHead_GetAllWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<BudgetHeadModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<BudgetHeadModel>
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

        public async Task<BudgetHeadModel?> GetBudgetHeadByIdAsync(int budgetHeadId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@BUDGETHEADID", budgetHeadId);

                return await conn.QueryFirstOrDefaultAsync<BudgetHeadModel>(
                    "SP_BudgetHead_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeleteBudgetHead(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string budgetHeadId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@BUDGETHEADID", budgetHeadId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_BudgetHead_Delete",
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

        public (bool success, string message) ToggleBudgetHeadStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@BUDGETHEADID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                parameters.Add("@IPADDRESS", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_BudgetHead_ToggleStatus",
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

        public async Task<(bool Success, string Message)> CopyBudgetHeadToSession(CopyRequest req)
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
                    "SP_BudgetHead_CopySession",
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

        public async Task<List<BudgetHeadModel>> GetAllBudgetHeadAsync(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@INCLUDEDELETED", includeDeleted);

                var result = conn.Query<BudgetHeadModel>(
                    "SP_MST_ACC_BUDGETHEAD_GETALL",
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
    }
}
