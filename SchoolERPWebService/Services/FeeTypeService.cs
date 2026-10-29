using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class FeeTypeService:IFeeTypeService
    {
        private readonly IConfiguration _configuration;
        public FeeTypeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse> UpsertFeeTypeAsync(FeeTypeRequest request)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));


                var parameters = new DynamicParameters();

                parameters.Add("@FeeTypeId", request.FeeTypeId, DbType.Int32);
                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@SessionID", request.SessionID, DbType.Int32);
                parameters.Add("@FeeTypeName", request.FeeTypeName, DbType.String);
                parameters.Add("@FeeTypeCode", request.FeeTypeCode, DbType.String);
                parameters.Add("@DisplayLabel", request.DisplayLabel, DbType.String);
                parameters.Add("@FeeHeadId", request.FeeHeadId, DbType.Int32);
                parameters.Add("@IsRefund", request.IsRefund, DbType.Boolean);
                parameters.Add("@IsDiscount", request.IsDiscount, DbType.Boolean);
                parameters.Add("@IsActive", request.IsActive, DbType.Boolean);
                parameters.Add("@UserId", request.UserId, DbType.Int32);
                parameters.Add("@IPAddress", request.IPAddress, DbType.String);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_FeeType_Upsert",
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

        public async Task<PagedResult<FeeTypeViewModel>> GetAllFeeTypeWithPage(SearchRequest req)
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

                var result = (await conn.QueryAsync<FeeTypeViewModel>(
                "SP_FeeType_GetAllWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<FeeTypeViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<FeeTypeViewModel>
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

        public async Task<FeeTypeViewModel?> GetFeeTypeByIdAsync(int feeHeadId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FEEHEADID", feeHeadId);

                return await conn.QueryFirstOrDefaultAsync<FeeTypeViewModel>(
                    "SP_FeeType_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeleteFeeType(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string feeTypeId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@FEETYPEID", feeTypeId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_FeeType_Delete",
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

        public (bool success, string message) ToggleFeeTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FEETYPEID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                parameters.Add("@IPADDRESS", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_FeeType_ToggleStatus",
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

        public List<DropdownModel> GetFeeTypeDropdownList(int companyId, int sessionID, int userId)
        {
            try
            {

                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "SP_FeeType_GetAllDropDownBind",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionID,
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
