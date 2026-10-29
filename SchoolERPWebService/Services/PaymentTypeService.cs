using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class PaymentTypeService: IPaymentTypeService
    {
        private readonly IConfiguration _configuration;
        public PaymentTypeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SpPaymentType UpsertPaymentTypeAsync(PaymentTypeRequest request)
        {
            var response = new SpPaymentType();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@PaymentTypeId", request.PaymentTypeId);
                parameters.Add("@PaymentType", request.PaymentType);
                parameters.Add("@PaymentCode", request.PaymentCode);
                parameters.Add("@LogoUrl", request.LogoUrl);
                parameters.Add("@WebsiteUrl", request.WebsiteUrl);
                parameters.Add("@Description", request.Description);
                parameters.Add("@DisplayOrder", request.DisplayOrder);
                parameters.Add("@IsSystemPayment", request.IsSystemPayment);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpPaymentType>(
                    "SP_PaymentType_Upsert",
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

        public async Task<PagedResult<PaymentTypeModel>> GetAllPaymentTypeWithPage(SearchRequest req)
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


                param.Add("@ISACTIVE", null);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<PaymentTypeModel>(
                "SP_PaymentType_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<PaymentTypeModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<PaymentTypeModel>
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

        public async Task<PaymentTypeModel?> GetPaymentTypeByIdAsync(int paymentTypeId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@PaymentTypeId", paymentTypeId);

                return await conn.QueryFirstOrDefaultAsync<PaymentTypeModel>(
                    "SP_PaymentType_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeletePaymentType(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string paymentTypeId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@PaymentTypeIds", paymentTypeId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_PaymentType_Delete",
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

        public (bool success, string message) TogglePaymentTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@PaymentTypeIds", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                parameters.Add("@IPADDRESS", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_PaymentType_ToggleStatus",
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

        public async Task<List<PaymentTypeModel>> GetAllPaymentTypeAsync(bool? isActive = null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ISACTIVE", isActive);

                var result = conn.Query<PaymentTypeModel>(
                    "SP_PaymentType_GetAll",
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

        public (bool success, string message) UpsertPaymentTypeAttachmentFile(
 PaymentTypeAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@PaymentTypeId", req.PaymentTypeId);
                parameters.Add("@LogoUrl", req.LogoUrl);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_PaymentType_UpsertLogo",
                    parameters,
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


        public List<DropdownModel> GetPaymenTypeDropdownList()
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "tbl_Mst_Payment_Type_GetAllDropDownBind",
                    new
                    {},
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
