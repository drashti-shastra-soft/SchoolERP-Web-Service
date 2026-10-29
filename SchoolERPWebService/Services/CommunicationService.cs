
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;

public class CommunicationService : ICommunicationService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailQueueSignal _signal;
    public CommunicationService(IConfiguration configuration, IEmailQueueSignal signal)
    {
        _configuration = configuration;
        _signal = signal;
    }

    public async Task<CommunicationSaveResponse> SaveCommunicationAsyncold(
        CommunicationSaveRequest model)
    {
        var response = new CommunicationSaveResponse();

        try
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@COMPANYID", model.CompanyID);
            parameters.Add("@SESSIONID", model.SessionID);
            parameters.Add("@USERID", model.UserID);

            parameters.Add("@TITLE", model.Title);
            parameters.Add("@MESSAGE", model.Message);
            parameters.Add("@TEMPLATEID", model.TemplateID);
            parameters.Add("@DLTTEMPLATEID", model.DLTTemplateID);

            parameters.Add("@ATTACHMENT", model.Attachment);
            parameters.Add("@ATTACHMENTNAME", model.AttachmentName);
            parameters.Add("@ATTACHMENTTYPE", model.AttachmentType);

            parameters.Add("@ISEMAIL", model.IsEmail);
            parameters.Add("@ISSMS", model.IsSMS);
            parameters.Add("@ISMOBILEAPP", model.IsMobileApp);

            parameters.Add("@SENDMODE", model.SendMode);
            parameters.Add("@SCHEDULEDATETIME", model.ScheduleDateTime);

            parameters.Add("@RECIPIENTSJSON", model.RecipientsJson);

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "SP_Communication_Save",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                response.Result = Convert.ToInt32(result.RESULT);
                response.Message = result.MESSAGE?.ToString()
                    ?? string.Empty;

                response.CommunicationID =
                    result.COMMUNICATIONID == null
                        ? 0
                        : Convert.ToInt32(result.COMMUNICATIONID);

                response.SendMode = result.SENDMODE?.ToString();

                response.SendNow =
                    result.SENDNOW != null &&
                    Convert.ToBoolean(result.SENDNOW);

                response.TechnicalMessage =
                    result.TECHNICALMESSAGE?.ToString();

                response.ErrorLine =
                    result.ERRORLINE == null
                        ? null
                        : Convert.ToInt32(result.ERRORLINE);
            }
        }
        catch (Exception ex)
        {
            response.Result = 0;
            response.Message =
                "Unable to save communication. Please try again.";
            response.TechnicalMessage = ex.Message;
        }

        return response;
    }

    public async Task<CommunicationSaveResponse> SaveCommunicationAsync(CommunicationSaveRequest model)
    {
        var response = new CommunicationSaveResponse();

        try
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@COMPANYID", model.CompanyID);
            parameters.Add("@SESSIONID", model.SessionID);
            parameters.Add("@USERID", model.UserID);
            parameters.Add("@TITLE", model.Title);
            parameters.Add("@MESSAGE", model.Message);
            parameters.Add("@TEMPLATEID", model.TemplateID);
            parameters.Add("@DLTTEMPLATEID", model.DLTTemplateID);
            parameters.Add("@ATTACHMENT", model.Attachment);
            parameters.Add("@ATTACHMENTNAME", model.AttachmentName);
            parameters.Add("@ATTACHMENTTYPE", model.AttachmentType);
            parameters.Add("@ISEMAIL", model.IsEmail);
            parameters.Add("@ISSMS", model.IsSMS);
            parameters.Add("@ISMOBILEAPP", model.IsMobileApp);
            parameters.Add("@SENDMODE", model.SendMode);
            parameters.Add("@SCHEDULEDATETIME", model.ScheduleDateTime);
            parameters.Add("@RECIPIENTSJSON", model.RecipientsJson);

            // 1) save header + recipients (Status = PENDING)
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "SP_Communication_Save", parameters, commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                response.Result = Convert.ToInt32(result.RESULT);
                response.Message = result.MESSAGE?.ToString() ?? string.Empty;
                response.CommunicationID = result.COMMUNICATIONID == null ? 0 : Convert.ToInt32(result.COMMUNICATIONID);
                response.SendMode = result.SENDMODE?.ToString();
                response.SendNow = result.SENDNOW != null && Convert.ToBoolean(result.SENDNOW);
                response.TechnicalMessage = result.TECHNICALMESSAGE?.ToString();
                response.ErrorLine = result.ERRORLINE == null ? null : Convert.ToInt32(result.ERRORLINE);
            }

            // 2) SEND NOW -> expand recipients and fill EmailQueue / SmsQueue.
            //    SCHEDULE   -> nothing here; usp_Communication_ProcessDue queues it when due.
            if (response.Result == 1 && response.SendNow && response.CommunicationID > 0)
            {
                try
                {
                    var q = await conn.QueryFirstOrDefaultAsync<dynamic>(
                        "usp_Communication_Queue",
                        new { 
                            COMMUNICATIONID = response.CommunicationID, 
                            INCLUDEUNLINKEDUSERS = false, 
                            SILENT = false },
                        commandType: CommandType.StoredProcedure);

                    if (q != null && Convert.ToInt32(q.RESULT) == 1)
                    {
                        int emails = Convert.ToInt32(q.EMAILQUEUED);
                        int sms = Convert.ToInt32(q.SMSQUEUED);
                        response.Message = $"Message queued: {emails} email(s), {sms} SMS.";
                        _signal.Notify();
                    }
                    else
                    {
                        // saved, but nobody could be reached (no valid email / mobile)
                        response.Result = 0;
                        response.Message = q?.MESSAGE?.ToString() ?? "Message saved but could not be queued.";
                        response.TechnicalMessage = q?.TECHNICALMESSAGE?.ToString();
                        _signal.Notify();
                    }
                }
                catch (Exception qex)
                {
                    // Header is saved as PENDING, so usp_Communication_ProcessDue will retry it.
                    response.TechnicalMessage = qex.Message;
                    response.Message += " Queuing will be retried automatically.";
                }
            }
        }
        catch (Exception ex)
        {
            response.Result = 0;
            response.Message = "Unable to save communication. Please try again.";
            response.TechnicalMessage = ex.Message;
        }

        return response;
    }


    public async Task<List<BirthdayTodayResponse>> GetTodayBirthdaysAsync(
    BirthdayTodayRequest request)
    {
        using var conn = new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));

        var parameters = new DynamicParameters();

        parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
        parameters.Add("@RoleID", request.RoleID, DbType.Int32);
        parameters.Add("@PersonType", request.PersonType, DbType.String);
        parameters.Add("@OnDate", request.OnDate?.Date, DbType.Date);

        var result = await conn.QueryAsync<BirthdayTodayResponse>(
            "dbo.usp_Birthday_Today_Get",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task<PagedResult<CommunicationLogResponse>>
    GetCommunicationLogWithPageAsync(CommunicationLogRequest req)
    {
        using var conn = new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));

        if (req.PageNumber <= 0 && req.PageSize <= 0)
        {
            req.PageNumber = 1;
            req.PageSize = 10;
        }
        else
        {
            if (req.PageNumber <= 0)
                req.PageNumber = 1;

            if (req.PageSize <= 0)
                req.PageSize = 10;
        }

        var param = new DynamicParameters();

        param.Add("@COMPANYID", req.CompanyID, DbType.Int32);
        param.Add("@SESSIONID", req.SessionID, DbType.Int32);
        param.Add("@LOGTYPE", req.LogType, DbType.String);
        param.Add("@SEARCHKEYWORD", req.SearchKeyword, DbType.String);
        param.Add("@FROMDATE", req.FromDate?.Date, DbType.Date);
        param.Add("@TODATE", req.ToDate?.Date, DbType.Date);
        param.Add("@SORTCOLUMN", req.SortColumn, DbType.String);
        param.Add("@SORTDIR", req.SortDir, DbType.String);
        param.Add("@PAGENUMBER", req.PageNumber, DbType.Int32);
        param.Add("@PAGESIZE", req.PageSize, DbType.Int32);

        var result = (await conn.QueryAsync<CommunicationLogResponse>(
            "dbo.SP_Communication_Log_GetAllWithPageIndex",
            param,
            commandType: CommandType.StoredProcedure)).ToList();

        var first = result.FirstOrDefault();

        int totalRecords = first?.TOTALRECORDS ?? 0;
        int pageNumber = first?.CURRENTPAGE ?? req.PageNumber;
        int pageSize = first?.PAGESIZE ?? req.PageSize;

        return new PagedResult<CommunicationLogResponse>
        {
            Data = first?.Result == 1 ? result : null,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
