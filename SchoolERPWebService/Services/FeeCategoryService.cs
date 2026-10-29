using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class FeeCategoryService: IFeeCategoryService
    {
        private readonly IConfiguration _configuration;
        public FeeCategoryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse> UpsertFeeCategoryAsync(FeeCategoryRequest request)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@FeeCategoryId", request.FeeCategoryId, DbType.Int32);
                parameters.Add("@CategoryGroupId", request.CategoryGroupId, DbType.Int32);
                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@SessionID", request.SessionID, DbType.Int32);
                parameters.Add("@FeeCategoryName", request.FeeCategoryName, DbType.String);
                parameters.Add("@DisplayLabel", request.DisplayLabel, DbType.String);
                parameters.Add("@IsActive", request.IsActive, DbType.Boolean);
                parameters.Add("@UserId", request.UserId, DbType.Int32);
                parameters.Add("@IPAddress", request.IPAddress, DbType.String);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_FeeCategory_Upsert",
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

        public async Task<PagedResult<FeeCategoryViewModel>> GetAllFeeCategoryWithPage(SearchRequest req)
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
                param.Add("@CATEGORYGROUPID", null);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<FeeCategoryViewModel>(
                "SP_FeeCategory_GetAllWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<FeeCategoryViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<FeeCategoryViewModel>
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

        public async Task<FeeCategoryViewModel?> GetFeeCategoryByIdAsync(int feeCategoryId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FEECATEGORYID", feeCategoryId);

                return await conn.QueryFirstOrDefaultAsync<FeeCategoryViewModel>(
                    "SP_FeeCategory_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeleteFeeCategory(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string feeCategoryId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@FEECATEGORYID", feeCategoryId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_FeeCategory_Delete",
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

        public (bool success, string message) ToggleFeeCategoryStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FEECATEGORYID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                parameters.Add("@IPADDRESS", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_FeeCategory_ToggleStatus",
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

        public async Task<(bool Success, string Message)> CopyFeeCategoryToSession(CopyRequest req)
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
                    "SP_FeeCategory_CopySession",
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

        public async Task<List<FeeCategoryViewModel>> GetAllFeeCategoryAsync(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@INCLUDEDELETED", includeDeleted);

                var result = conn.Query<FeeCategoryViewModel>(
                    "SP_FeeCategory_GetAll",
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

        public List<DropdownModel> GetFeeCategoryDropdownList(int companyId, int sessionID, int userId, int? categoryGroupId)
        {
            try
            {

                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "SP_FeeCategory_GetAllDropDownBind",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionID,
                        CategoryGroupId = categoryGroupId,
                        UserId = userId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
