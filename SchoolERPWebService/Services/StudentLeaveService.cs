using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service manages leave applications submitted by students.
    /// It handles the submission of requests, status updates (approvals/rejections), 
    /// and any supporting documents or reasons provided.
    /// </summary>
    public class StudentLeaveService: IStudentLeaveService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FrontOfficeService> _logger;
        public StudentLeaveService(IConfiguration configuration, ILogger<FrontOfficeService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all student leave applications based on class, section, and status filters.
        /// </summary>
        public List<StudentLeaveViewModel> GetLeaveApplications(int? classId, int? sectionId, int? status, int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
               _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@Action", "LIST");
                parameters.Add("@CompanyID", companyId);

                if (classId.HasValue && classId > 0)
                    parameters.Add("@ClassID", classId.Value);

                if (sectionId.HasValue && sectionId > 0)
                    parameters.Add("@SectionID", sectionId.Value);

                if (status.HasValue && status >= 0)
                    parameters.Add("@Status", status.Value);

                return conn.Query<StudentLeaveViewModel>(
                    "sp_Student_LeaveApp_CRUD",
                    parameters,
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex,ex.Message);
                return null;
            }
           
        }
        /// <summary>
        /// Updates the status of a student's leave application.
        /// </summary>
        public (bool Success, string Message) UpdateLeaveStatus(
            int leaveAppId,
            int status,
            int companyId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@Action", "UPDATE_STATUS");
                parameters.Add("@LeaveAppID", leaveAppId);
                parameters.Add("@Status", status);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Student_LeaveApp_CRUD",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    return (true, Convert.ToString(result.Message));
                }

                return (false, "Failed to update status");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return (false, "Failed to update status");
            }
            
        }
        /// <summary>
        /// Creates a new leave application or updates an existing leave application.
        /// </summary>
        public SpLeaveApplicationResult UpsertLeaveApplication(
            StudentLeaveUpsertRequest req,
            int companyId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@Action", "UPSERT");
                parameters.Add("@LeaveAppID", req.LeaveAppID);
                parameters.Add("@StudentID", req.StudentID);
                parameters.Add("@FromDate", req.FromDate);
                parameters.Add("@ToDate", req.ToDate);
                parameters.Add("@ApplyDate", req.ApplyDate);
                parameters.Add("@Reason", req.Reason);
                parameters.Add("@Attachment", req.Attachment, DbType.Binary);
                parameters.Add("@AttachmentType", req.AttachmentType);
                parameters.Add("@AttachmentName", req.AttachmentName);
                parameters.Add("@Status", req.Status);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpLeaveApplicationResult>(
                    "sp_Student_LeaveApp_CRUD",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;

                
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return null;
            }
            
        }
        /// <summary>
        /// Retrieves the attachment file associated with a leave application.
        /// </summary>
        public (byte[]? Bytes, string? FileName, string? ContentType)
            GetLeaveAttachment(int leaveAppId, int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@Action", "GET_ATTACHMENT");
                parameters.Add("@LeaveAppID", leaveAppId);
                parameters.Add("@CompanyID", companyId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Student_LeaveApp_CRUD",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result != null && result.Attachment != null)
                {
                    return (
                        (byte[])result.Attachment,
                        Convert.ToString(result.AttachmentName),
                        Convert.ToString(result.AttachmentType)
                    );
                }

                return (null, null, null);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return (null, null, null);
            }
            
        }

        public async Task<PagedResult<StudentLeaveViewModel>> GetAllLeaveApplicationsWithPage(StudentLeaveSearchRequest req)
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
                param.Add("@CLASSID", req.ClassID);
                param.Add("@SECTIONID", req.SectionID);
                param.Add("@STATUS", req.Status);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<StudentLeaveViewModel>(
                "SP_STUDENT_LEAVEAPP_LIST_PAGED",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<StudentLeaveViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentLeaveViewModel>
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


        /// <summary>
        /// Retrieves all student leave applications based on class, section, and status filters.
        /// </summary>
        public StudentLeaveViewModel? GetLeaveApplicationsById(int? leaveAppId, int? companyId)
        {
            try
            {
                using var conn = new SqlConnection(
               _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@LEAVEAPPID", leaveAppId);
                parameters.Add("@COMPANYID", companyId);


                return conn.QueryFirst<StudentLeaveViewModel>(
                    "SP_STUDENT_LEAVEAPP_GETBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return null;
            }

        }


        /// <summary>
        /// Updates/replaces the attachment file for an existing student leave application
        /// without affecting the other leave application fields.
        /// </summary>
        /// <param name="req">
        /// Request payload containing the leave application ID and the new attachment
        /// (file data, type, and name).
        /// </param>
        /// <param name="userId">ID of the user performing the update (for audit/history).</param>
        /// <returns>
        /// A tuple indicating whether the update succeeded (<c>success</c>) and a corresponding
        /// user-friendly <c>message</c>.
        /// </returns>
        public (bool success, string message) UpsertLeaveApplicationAttachmentFile(
     LeaveApplicationAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@LEAVEAPPID", req.LeaveAppId);
                parameters.Add("@ATTACHMENT", req.Attachment);
                parameters.Add("@ATTACHMENTTYPE", req.FileType);
                parameters.Add("@ATTACHMENTNAME", req.FileName);
                parameters.Add("@COMPANYID", req.CompanyId);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_STUDENT_LEAVEAPP_UPDATE_ATTACHMENT",
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


        /// <summary>
        /// Deletes the specified Leave Application 
        /// </summary>
        public (bool Success, string Message) DeleteLeaveApplication(
            List<int> ids,
            int userId,int companyId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No leave application selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", ids);

                var param = new DynamicParameters();

                param.Add("@LEAVEAPPIDS", id);
                param.Add("@COMPANYID", companyId);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_STUDENT_LEAVEAPP_DELETE",
                    param,
                    commandType: CommandType.StoredProcedure);

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

    }
}
