using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class PaymentTypeFieldService: IPaymentTypeFieldService
    {
        private readonly IConfiguration _configuration;
        public PaymentTypeFieldService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SpResult UpsertPaymentTypeFieldAsync(PaymentFieldRequest request)
        {
            var response = new SpResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@FieldID", request.FieldID);
                parameters.Add("@PaymentTypeId", request.PaymentTypeId);
                parameters.Add("@FieldKey", request.FieldKey);
                parameters.Add("@FieldLabel", request.FieldLabel);
                parameters.Add("@FieldType", request.FieldType ?? "TEXT");
                parameters.Add("@IsRequired", request.IsRequired);
                parameters.Add("@IsSecret", request.IsSecret);
                parameters.Add("@DisplayOrder", request.DisplayOrder);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_PaymentTypeField_Upsert",
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

        public async Task<PagedResult<PaymentTypeFieldModel>> GetAllPaymentTypeFieldWithPage(SearchRequest req)
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


                param.Add("@PaymentTypeId", req.PaymentTypeId);
                param.Add("@ISACTIVE", null);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<PaymentTypeFieldModel>(
                "SP_PaymentTypeField_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<PaymentTypeFieldModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<PaymentTypeFieldModel>
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
        public async Task<PaymentTypeFieldModel?> GetPaymentTypeFieldByIdAsync(int fieldID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FieldID", fieldID);

                return await conn.QueryFirstOrDefaultAsync<PaymentTypeFieldModel>(
                    "SP_PaymentTypeField_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeletePaymentTypeField(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string fieldID = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@FieldIDs", fieldID);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_PaymentTypeField_Delete",
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

        public (bool success, string message) TogglePaymentTypeFieldStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FieldIDs", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);
                parameters.Add("@IPADDRESS", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_PaymentTypeField_ToggleStatus",
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

        public async Task<List<PaymentTypeFieldModel>> GetAllPaymentTypeByIdAsync(int paymentTypeId, bool? isActive = null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@PaymentTypeId", paymentTypeId);
                parameters.Add("@ISACTIVE", isActive);

                var result = conn.Query<PaymentTypeFieldModel>(
                    "SP_PaymentTypeField_GetByPaymentType",
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
