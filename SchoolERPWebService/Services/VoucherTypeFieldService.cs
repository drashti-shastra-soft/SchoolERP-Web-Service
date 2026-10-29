using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.ComponentModel.Design;

namespace SchoolERPWebService.Services
{
    public class VoucherTypeFieldService: IVoucherTypeFieldService
    {
        private readonly IConfiguration _configuration;
        public VoucherTypeFieldService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SpResult UpsertVoucherFieldAsync(VoucherFieldRequest request)
        {
            var response = new SpResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@FieldID", request.FieldID);
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@VoucherId", request.VoucherId);
                parameters.Add("@FieldLabel", request.FieldLabel);
                parameters.Add("@FieldCode", request.FieldCode);
                parameters.Add("@FieldType", request.FieldType ?? "TEXT");
                parameters.Add("@Section", request.Section);
                parameters.Add("@DisplayOrder", request.DisplayOrder);
                parameters.Add("@IsRequired", request.IsRequired);
                parameters.Add("@IsReadOnly", request.IsReadOnly);
                parameters.Add("@MaxLength", request.MaxLength);
                parameters.Add("@DecimalPlaces", request.DecimalPlaces);
                parameters.Add("@DefaultValue", request.DefaultValue);
                parameters.Add("@SourceType", request.SourceType);
                parameters.Add("@SourceTable", request.SourceTable);
                parameters.Add("@SourceValueCol", request.SourceValueCol);
                parameters.Add("@SourceTextCol", request.SourceTextCol);
                parameters.Add("@SourceFilter", request.SourceFilter);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherTypeField_Upsert",
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

        public async Task<PagedResult<VoucherTypeFieldModel>> GetAllVoucherTypeFielddWithPage(SearchRequest req)
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


                param.Add("@VoucherId", req.VoucherId);
                param.Add("@CompanyID", req.CompanyID);
                param.Add("@ISACTIVE", null);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<VoucherTypeFieldModel>(
                "SP_VoucherTypeField_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<VoucherTypeFieldModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<VoucherTypeFieldModel>
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

        public async Task<VoucherTypeFieldModel?> GetVoucherTypeFieldByIdAsync(int fieldID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FieldID", fieldID);

                return await conn.QueryFirstOrDefaultAsync<VoucherTypeFieldModel>(
                    "SP_VoucherTypeField_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeleteVoucherTypeField(List<int> ids, int userId, string ipAddress)
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
                parameters.Add("@FieldIds", fieldID);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_VoucherTypeField_Delete",
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

        public (bool success, string message) ToggleVoucherTypeFieldStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FieldIds", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherTypeField_ToggleStatus",
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

        public List<DropdownModel> GetVoucherTypeFieldDropdownList(int companyID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "tbl_Mst_VoucherType_Field_GetAllDropDownBind",
                new
                    {
                        CompanyID= companyID
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

        public async Task<List<VoucherDynamicFieldResponse>>
    GetVoucherDynamicFieldsAsync(
        int companyId,
        int voucherId,int sessionId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@CompanyID", companyId);
            parameters.Add("@VoucherId", voucherId);
            parameters.Add("@SessionId", sessionId);

            var result = await conn.QueryAsync<VoucherDynamicFieldResponse>(
                "SP_VoucherType_GetDynamicFields",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }
    }

    
}
