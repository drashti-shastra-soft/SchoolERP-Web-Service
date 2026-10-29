using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    public class VoucherHeaderService: IVoucherHeaderService
    {
        private readonly IConfiguration _configuration;
        public VoucherHeaderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SpResult UpsertVoucherHeaderAsync(VoucherHeaderRequest request)
        {
            var response = new SpResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@VoucherHeaderID", request.VoucherHeaderID);
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@VoucherId", request.VoucherId);
                parameters.Add("@VoucherNo", request.VoucherNo);
                parameters.Add("@VoucherDate", request.VoucherDate);
                parameters.Add("@VNo", request.VNo);
                parameters.Add("@CostCenterID", request.CostCenterID);
                parameters.Add("@TotalAmount", request.TotalAmount);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherHeader_Upsert",
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

        public async Task<PagedResult<VoucherHeaderModel>> GetAllVoucherHeaderWithPage(VoucherHeaderSearchRequest req)
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
                param.Add("@CostCenterID", req.CostCenterID);
                param.Add("@FROMDATE", req.FROMDATE);
                param.Add("@TODATE", req.TODATE);
                param.Add("@ISACTIVE", req.ISACTIVE);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<VoucherHeaderModel>(
                "SP_VoucherHeader_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<VoucherHeaderModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<VoucherHeaderModel>
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
        

        public async Task<VoucherHeaderFullResult?> GetVoucherHeaderIDByIdAsync(int voucherHeaderID)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@VoucherHeaderID", voucherHeaderID);

                using var multi = await conn.QueryMultipleAsync(
                    "SP_VoucherHeader_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // Result set 1: Header row — null if not found / deleted
                var header = await multi.ReadFirstOrDefaultAsync<VoucherHeaderModel>();
                if (header is null) return null;

                // Result set 2: Header field values
                var headerValues = (await multi.ReadAsync<VoucherHeaderFieldValueDto>()).ToList();

                // Result set 3: Detail rows
                var details = (await multi.ReadAsync<VoucherDetailDto>()).ToList();

                // Result set 4: Detail field values
                var detailValues = (await multi.ReadAsync<VoucherDetailValueDto>()).ToList();

                // Flatten header values into FieldID -> string, same shape
                // window.dynamicFieldValues already expects in the view.
                var dynamicFieldValues = headerValues.ToDictionary(
                    v => v.FieldID,
                    v => v.ValueText
                         ?? v.ValueNumber?.ToString()
                         ?? v.ValueDate?.ToString("yyyy-MM-dd")
                         ?? v.ValueBit?.ToString()
                         ?? "");

                return new VoucherHeaderFullResult
                {
                    Header = header,
                    DynamicFieldValues = dynamicFieldValues,
                    Details = details,
                    DetailValues = detailValues
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public (bool success, string message) DeleteVoucherHeader(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string voucherHeaderId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@VoucherHeaderIds", voucherHeaderId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_VoucherHeader_Delete",
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

        public (bool success, string message) ToggleVoucherHeaderStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@VoucherHeaderIds", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherHeader_ToggleStatus",
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

        #region Voucher Header Value
        public SpResult UpsertVoucherHeaderValueAsync(VoucherHeaderValueRequest request)
        {
            var response = new SpResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@ValueID", request.ValueID);
                parameters.Add("@VoucherHeaderID", request.VoucherHeaderID);
                parameters.Add("@FieldID", request.FieldID);
                parameters.Add("@ValueText", request.ValueText);
                parameters.Add("@ValueNumber", request.ValueNumber);
                parameters.Add("@ValueDate", request.ValueDate);
                parameters.Add("@ValueBit", request.ValueBit);
                parameters.Add("@RefID", request.RefID);
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherHeaderValue_Upsert",
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

        public List<VoucherHeaderValue> GetVoucherHeaderValueList(int companyID,int sessionID,int fieldID,bool? isActive)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", companyID);
                parameters.Add("@SessionID", sessionID);
                parameters.Add("@FieldID", fieldID);
                parameters.Add("@ISACTIVE", isActive);
                parameters.Add("@INCLUDEDELETED", false);
                var result = conn.Query<VoucherHeaderValue>(
                    "SP_VoucherHeaderValue_GetByCompanySessionField",
                    new
                    { parameters },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<VoucherHeaderValue?> GetVoucherHeaderValueByIdAsync(int valueID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ValueID", valueID);

                return await conn.QueryFirstOrDefaultAsync<VoucherHeaderValue>(
                    "SP_VoucherHeaderValue_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #endregion

        #region Voucher Details
        public SpResult UpsertVoucherDetailAsync(VoucherDetailRequest request)
        {
            var response = new SpResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@DetailID", request.DetailID);
                parameters.Add("@VoucherHeaderID", request.VoucherHeaderID);
                parameters.Add("@RowNo", request.RowNo);
                parameters.Add("@AccID", request.AccID);
                parameters.Add("@Narration", request.Narration);
                parameters.Add("@Amount", request.Amount);
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherDetail_Upsert",
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

        public List<VoucherDetail> GetVoucherDetailList(int companyID, int sessionID, int voucherHeaderID, bool? isActive)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", companyID);
                parameters.Add("@SessionID", sessionID);
                parameters.Add("@VoucherHeaderID", voucherHeaderID);
                parameters.Add("@ISACTIVE", isActive);
                parameters.Add("@INCLUDEDELETED", false);
                var result = conn.Query<VoucherDetail>(
                    "SP_VoucherDetail_GetByCompanySessionHeader",
                    new
                    { parameters },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<VoucherDetail?> GetVoucherDetailByIdAsync(int detailID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@DetailID", detailID);

                return await conn.QueryFirstOrDefaultAsync<VoucherDetail>(
                    "SP_VoucherDetail_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion

        #region Voucher Details Value
        public SpResult UpsertVoucherDetailValueAsync(VoucherDetailValueRequest request)
        {
            var response = new SpResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@ValueID", request.ValueID);
                parameters.Add("@DetailID", request.DetailID);
                parameters.Add("@FieldID", request.FieldID);
                parameters.Add("@ValueText", request.ValueText);
                parameters.Add("@ValueNumber", request.ValueNumber);
                parameters.Add("@ValueDate", request.ValueDate);
                parameters.Add("@ValueBit", request.ValueBit);
                parameters.Add("@RefID", request.RefID);
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_VoucherDetailValue_Upsert",
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
        
        public List<VoucherDetailValue> GetVoucherDetailValueList(VoucherValueSearchRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@VoucherHeaderID", request.VoucherHeaderID);
                parameters.Add("@DetailID", request.DetailID);
                parameters.Add("@FieldID", request.FieldID);
                parameters.Add("@ISACTIVE", request.ISACTIVE);
                parameters.Add("@INCLUDEDELETED",false);
                var result = conn.Query<VoucherDetailValue>(
                    "SP_VoucherDetailValue_GetByCompanySessionHeader",
                    new
                    { parameters },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<VoucherDetailValue?> GetVoucherDetailValueByIdAsync(int valueID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ValueID", valueID);

                return await conn.QueryFirstOrDefaultAsync<VoucherDetailValue>(
                    "SP_VoucherDetailValue_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        #endregion

        public async Task<VoucherSaveFullResult> SaveVoucherFullAsync(
    VoucherSaveFullRequest request)
        {
            var response = new VoucherSaveFullResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@VoucherHeaderID", request.VoucherHeaderID);
                parameters.Add("@VoucherId", request.VoucherId);
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@VoucherNo",request.VoucherHeaderID > 0 ? request.VoucherNo : null);
                parameters.Add("@VoucherDate", request.VoucherDate);
                parameters.Add("@VNo", request.VNo);
                parameters.Add("@CostCenterID", request.CostCenterID);
                parameters.Add("@ForType", request.ForType);
                parameters.Add("@ForRefId", request.ForRefId);

                parameters.Add(
                    "@HeaderValuesJson",
                    JsonSerializer.Serialize(request.HeaderValues));

                parameters.Add(
                    "@DetailsJson",
                    JsonSerializer.Serialize(request.Details));

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<VoucherSaveFullResult>(
                    "SP_Voucher_SaveFull",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result ?? new VoucherSaveFullResult
                {
                    Result = 0,
                    Message = "Unable to save voucher. Please try again."
                };
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save voucher. Please try again.";
                response.TECHNICALMESSAGE = ex.Message;

                return response;
            }
        }

        public List<DropdownModel> GetStaffDropdownList(int companyID, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "tbl_HR_Staff_Field_GetAllDropDownBind",
                new
                {
                    CompanyID = companyID,
                    SessionID = sessionId
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

        public List<DropdownModel> GetStudentsDropdownList(int companyID, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "tbl_Mst_Students_GetAllDropDownBind",
                new
                {
                    CompanyID = companyID,
                    SessionID = sessionId
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

        public (bool success, string message) UpsertVoucherHeaderAttachmentFile(
    VoucherHeaderAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@VoucherHeaderID", req.VoucherHeaderID);
                parameters.Add("@ATTACHMENT", req.Attachment);
                parameters.Add("@FILENAME", req.FileName);
                parameters.Add("@FILETYPE", req.FileType);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_VoucherHeader_Update_Attachment",
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

        public List<VoucherDetailClearingResponse> GetForClearing(VoucherBankClearingRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", req.CompanyID, DbType.Int32);
                parameters.Add("@BankFieldValue", req.BankFieldValue, DbType.Int32);
                parameters.Add("@DateFrom", req.DateFrom, DbType.DateTime);
                parameters.Add("@DateTo", req.DateTo, DbType.DateTime);
                parameters.Add("@Cleared", req.Cleared, DbType.Boolean);

                var result = conn.Query<VoucherDetailClearingResponse>(
                    "SP_VoucherDetail_GetForClearing",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();               

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<SpResult> SaveVoucherClearingAsync(
    VoucherBankClearingSaveRequest request)
        {
            var response = new VoucherSaveFullResult();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@DetailIdsJson", request.DetailIdsJson, DbType.String);
                parameters.Add("@ClearingDate", request.ClearingDate, DbType.DateTime);
                parameters.Add("@Cleared", request.Cleared, DbType.Boolean);
                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@UserId", request.UserId, DbType.Int32);
                parameters.Add("@IPAddress", request.IPAddress, DbType.String);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_VoucherDetail_UpdateClearing",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result ?? new SpResult
                {
                    Result = 0,
                    Message = "Unable to save voucher. Please try again."
                };
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save voucher. Please try again.";
                response.TECHNICALMESSAGE = ex.Message;

                return response;
            }
        }
    }

}
