using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace SchoolERPWebService.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IConfiguration _configuration;
        
        public EmailTemplateService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ---------------------------------------------------------------------
        // UPSERT  (+ attachment flow)
        //   1) validate file  2) SP_EmailTemplate_Upsert -> EmailTemplateId
        //   3) save file using that id  4) SP_EmailTemplate_UpdateAttachment
        // ---------------------------------------------------------------------
        public async Task<EmailTemplateSaveResponse> UpsertEmailTemplateAsync(EmailTemplateSaveRequest model)
        {
            var response = new EmailTemplateSaveResponse();
            try
            {
                
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                // ---- STEP 1 : upsert template ----
                var parameters = new DynamicParameters();
                parameters.Add("@EmailTemplateId", model.EmailTemplateId);
                parameters.Add("@CompanyID", model.CompanyID);
                parameters.Add("@Title", model.Title);
                parameters.Add("@Message", model.Message);
                parameters.Add("@DisplayLabel", model.DisplayLabel);
                parameters.Add("@IsActive", model.IsActive);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@IPAddress", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<EmailTemplateSpResult>(
                    "SP_EmailTemplate_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return response;

                response.Result = result.Result;
                response.Message = result.Message;
                response.TechnicalMessage = result.TECHNICALMESSAGE;
                response.EmailTemplateId = result.EmailTemplateId;

                if (result.Result != 1)
                    return response;

            }
            catch (Exception ex)
            {                
                response.Result = 0;
                response.Message = "Unable to save email template. Please try again.";
                response.TechnicalMessage = ex.Message;
            }

            return response;
        }

        // ---------------------------------------------------------------------
        // UPDATE ATTACHMENT (SP_EmailTemplate_UpdateAttachment)
        // ---------------------------------------------------------------------
        public async Task<ApiResponse> UpdateEmailTemplateAttachmentAsync(EmailTemplateAttachmentRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@EmailTemplateId", model.EmailTemplateId);
                parameters.Add("@Attachment", model.Attachment);
                parameters.Add("@AttachmentType", model.AttachmentType);
                parameters.Add("@AttachmentName", model.AttachmentName);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@IPAddress", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_EmailTemplate_UpdateAttachment",
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
                response.Message = "Unable to update attachment. Please try again.";
                response.TechnicalMessage = ex.Message;
            }

            return response;
        }

        // ---------------------------------------------------------------------
        // GET ALL (paged)
        // ---------------------------------------------------------------------
        public async Task<PagedResult<EmailTemplateViewModel>> GetAllEmailTemplateWithPage(SearchRequest req)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            if (req.PageNumber == 0 && req.PageSize == 0)
            {
                req.PageNumber = 1;
                req.PageSize = 10;
            }

            var param = new DynamicParameters();
            param.Add("@ISACTIVE", req.IsActive);
            param.Add("@COMPANYID", req.CompanyID);
            param.Add("@SEARCHTEXT", req.SearchKeyword);
            param.Add("@PAGEINDEX", req.PageNumber);
            param.Add("@PAGESIZE", req.PageSize);
            param.Add("@INCLUDEDELETED", false);

            var result = (await conn.QueryAsync<EmailTemplateViewModel>(
                "SP_EmailTemplate_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();

            var first = result.FirstOrDefault();
            int res = first?.Result ?? 0;

            return new PagedResult<EmailTemplateViewModel>
            {
                Data = res == 0 ? null : result,
                TotalRecords = first?.TOTALRECORDS ?? 0,
                PageNumber = first?.PageIndex ?? 0,
                PageSize = first?.PageSize ?? 0
            };
        }

        // ---------------------------------------------------------------------
        // GET BY ID
        // ---------------------------------------------------------------------
        public async Task<EmailTemplateDetailViewModel?> GetEmailTemplateByIdAsync(int emailTemplateId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@EmailTemplateId", emailTemplateId);

            return await conn.QueryFirstOrDefaultAsync<EmailTemplateDetailViewModel>(
                "SP_EmailTemplate_GetById",
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        // ---------------------------------------------------------------------
        // DELETE (soft)
        // ---------------------------------------------------------------------
        public async Task<(bool success, string message)> DeleteEmailTemplateAsync(
            List<int> ids, int userId, string ipAddress)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return (false, "No email template selected for deletion.");

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@EmailTemplateIds", string.Join(",", ids));
                parameters.Add("@UserId", userId);
                parameters.Add("@IPAddress", ipAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_EmailTemplate_Delete",
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

        // ---------------------------------------------------------------------
        // TOGGLE STATUS
        // ---------------------------------------------------------------------
        public async Task<(bool success, string message)> ToggleEmailTemplateStatusAsync(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@EmailTemplateIds", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_EmailTemplate_ToggleStatus",
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


        public List<EmailTempletDropdwon> GetEmailTemplateToDropdownList(int companyId, int userId)
        {
            try
            {

                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<EmailTempletDropdwon>(
                    "SP_EmailTemplate_GetAllDropDownBind",
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