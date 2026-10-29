using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class VoucherFieldOptionService: IVoucherFieldOptionService
    {
        private readonly IConfiguration _configuration;
        public VoucherFieldOptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public SpResult UpsertVoucherFieldOptionAsync(VoucherFieldOptionRequest request)
        {
            var response = new SpResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@OptionID", request.OptionID);
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@FieldID", request.FieldID);
                parameters.Add("@OptionLabel", request.OptionLabel);
                parameters.Add("@OptionValue", request.OptionValue);
                parameters.Add("@DisplayOrder", request.DisplayOrder);
                parameters.Add("@IsDefault", request.IsDefault);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherTypeFieldOption_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save cost center. Please try again.";
                response.TECHNICALMESSAGE = ex.Message;
            }

            return response;
        }

        public async Task<PagedResult<VoucherFieldOptionModel>> GetAllVoucherFieldOptionWithPage(SearchRequest req)
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


                param.Add("@FieldID", req.VoucherId);
                param.Add("@CompanyID", req.CompanyID);
                param.Add("@ISACTIVE", null);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<VoucherFieldOptionModel>(
                "SP_VoucherTypeFieldOption_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<VoucherFieldOptionModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<VoucherFieldOptionModel>
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

        public async Task<VoucherFieldOptionModel?> GetVoucherFieldOptionIDByIdAsync(int optionID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@OptionID", optionID);

                return await conn.QueryFirstOrDefaultAsync<VoucherFieldOptionModel>(
                    "SP_VoucherTypeFieldOption_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public (bool success, string message) DeleteVoucherFieldOption(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string optionId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@OptionIds", optionId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_VoucherTypeFieldOption_Delete",
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

        public (bool success, string message) ToggleVoucherFieldOptionStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@OptionIds", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherTypeFieldOption_ToggleStatus",
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
        public async Task<List<VoucherFieldOptionResponse>>
    GetLinkedOptionsAsync(
        int fieldId,
        string optionValue,
        int? companyId,
        int? sessionId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@FieldID", fieldId);
            parameters.Add("@OptionValue", optionValue);
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionId", sessionId);

            var result = await conn.QueryAsync<VoucherFieldOptionResponse>(
                "SP_VoucherFieldOption_GetLinkedOptions",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }
}
