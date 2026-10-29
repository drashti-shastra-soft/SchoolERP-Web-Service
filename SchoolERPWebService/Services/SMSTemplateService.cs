using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class SMSTemplateService: ISMSTemplateService
    {
        private readonly IConfiguration _configuration;
        public SMSTemplateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse> UpsertSMSTemplateAsync(SMSTemplateSaveRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@SMSTemplateId", model.SMSTemplateId);
                parameters.Add("@CompanyID", model.CompanyID);
                parameters.Add("@Title", model.Title);
                parameters.Add("@Message", model.Message);
                parameters.Add("@DisplayLabel", model.DisplayLabel);
                parameters.Add("@IsActive", model.IsActive);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@IPAddress", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_SMSTemplate_Upsert",
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

        public async Task<PagedResult<SMSTemplateViewModel>> GetAllSMSTemplateWithPage(SearchRequest req)
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


                param.Add("@ISACTIVE", req.IsActive);
                param.Add("@COMPANYID", req.CompanyID);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<SMSTemplateViewModel>(
                "SP_SMSTemplate_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<SMSTemplateViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<SMSTemplateViewModel>
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

        public async Task<SMSTemplateViewModel?> GetSMSTemplateByIdAsync(int smsTemplateId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@SMSTemplateId", smsTemplateId);

                return await conn.QueryFirstOrDefaultAsync<SMSTemplateViewModel>(
                    "SP_SMSTemplate_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeleteSMSTemplate(List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item store selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string smsTemplateId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@SMSTemplateIds", smsTemplateId);
                parameters.Add("@UserId", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_SMSTemplate_Delete",
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

        public (bool success, string message) ToggleSMSTemplateStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@SMSTemplateIds", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_SMSTemplate_ToggleStatus",
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

        public List<EmailTempletDropdwon> GetSMSTemplateToDropdownList(int companyId, int userId)
        {
            try
            {

                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<EmailTempletDropdwon>(
                    "SP_SMSTemplate_GetAllDropDownBind",
                    new
                    {
                        CompanyID = companyId,
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
