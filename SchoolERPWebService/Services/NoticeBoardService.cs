using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class NoticeBoardService: INoticeBoardService
    {
        private readonly IConfiguration _configuration;
        public NoticeBoardService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================================================
        // UPSERT (CREATE / UPDATE)  ->  SP_NoticeBoard_Upsert
        // ============================================================
        public async Task<NoticeBoardSaveResponse> UpsertNoticeBoardAsync(NoticeBoardRequestModel model)
        {
            var response = new NoticeBoardSaveResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@NoticeBoardID", model.NoticeBoardId);
                parameters.Add("@Title", model.Title);
                parameters.Add("@NoticeDate", model.NoticeDate);
                parameters.Add("@PublishDate", model.PublishDate);
                parameters.Add("@Message", model.NoticeMessage);
                parameters.Add("@Attachment", model.Attachment);
                parameters.Add("@AttachmentName", model.AttachmentName);
                parameters.Add("@AttachmentType", model.AttachmentType);
                parameters.Add("@RoleIds", string.Join(",", model.RoleIds ?? new List<int>()));
                parameters.Add("@IsActive", model.IsActive);
                parameters.Add("@CompanyID", model.CompanyID);
                parameters.Add("@SessionID", model.SessionID);
                parameters.Add("@UserId", model.UserID);
                parameters.Add("@IPAddress", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_NoticeBoard_Upsert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.NoticeBoardId = result.NOTICEBOARDID != null ? (int)result.NOTICEBOARDID : 0;
                }
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save notice. Please try again.";
                response.TechnicalMessage = ex.Message;
            }

            return response;
        }

        // ============================================================
        // GET BY ID  ->  SP_NoticeBoard_GetById
        // ============================================================
        public async Task<NoticeBoardModel?> GetNoticeBoardByIdAsync(int noticeBoardId, int? companyId = null, int? sessionId = null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@NoticeBoardID", noticeBoardId);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);

                return await conn.QueryFirstOrDefaultAsync<NoticeBoardModel>(
                    "SP_NoticeBoard_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ============================================================
        // GET ALL WITH PAGE (ADMIN + ROLE WISE)  ->  SP_NoticeBoard_GetAllPageIndex
        // ============================================================
        public async Task<PagedResult<NoticeBoardModel>> GetAllNoticeBoardWithPage(NoticeBoardSearchRequest req)
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

                param.Add("@COMPANYID", req.CompanyId);
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@ROLEID", req.RoleId);
                param.Add("@ISSUPERADMIN", req.IsSuperAdmin);//req.IsSuperAdmin
                param.Add("@FROMDATE", req.FromDate);
                param.Add("@TODATE", req.ToDate);
                param.Add("@ISACTIVE", req.IsActive);
                param.Add("@FILTERROLEID", req.FilterRoleId);
                param.Add("@INCLUDEDELETED", req.IncludeDeleted);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@USERID", req.UserID);

                var result = (await conn.QueryAsync<NoticeBoardModel>(
                    "SP_NoticeBoard_GetAllPageIndex",
                    param,
                    commandType: CommandType.StoredProcedure)).ToList();

                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageIndex ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var pagedModel = new PagedResult<NoticeBoardModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                // SP returned RESULT = 0 (no data / validation error)
                if (res == 0)
                {
                    pagedModel = new PagedResult<NoticeBoardModel>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return pagedModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Soft-deletes one or more notices (and their role mapping).
        /// </summary>
        /// <param name="ids">Notice board IDs.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <param name="ipAddress">Caller IP for audit.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) DeleteNoticeBoard(List<int> ids, int userId, string? ipAddress = null)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No notices selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string noticeBoardIds = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@NoticeBoardIDs", noticeBoardIds);
                parameters.Add("@UserId", userId);
                parameters.Add("@IPAddress", ipAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_NoticeBoard_Delete",
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

        /// <summary>
        /// Activates or deactivates one or more notices.
        /// </summary>
        /// <param name="request">Ids (comma separated), IsActive, DoneBy (user id).</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) ToggleNoticeBoardStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@NOTICEBOARDID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_NoticeBoard_ToggleStatus",
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
        // UPDATE ATTACHMENT (SP_NoticeBoard_UpdateAttachment)
        // ---------------------------------------------------------------------
        public async Task<ApiResponse> UpdateNoticeBoardAttachmentAsync(NoticeBoardAttachmentRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@NoticeBoardId", model.NoticeBoardID);
                parameters.Add("@Attachment", model.Attachment);
                parameters.Add("@AttachmentType", model.AttachmentType);
                parameters.Add("@AttachmentName", model.AttachmentName);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@IPAddress", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_NoticeBoard_UpdateAttachment",
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

    }
}
