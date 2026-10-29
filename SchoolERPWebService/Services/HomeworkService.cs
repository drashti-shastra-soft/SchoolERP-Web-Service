using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service manages student homework assignments.
    /// It allows teachers to create homework tasks, set deadlines, 
    /// and keep track of when homework should be submitted.
    /// </summary>
    public class HomeworkService: IHomeworkService
    {
        private readonly IConfiguration _configuration;
        public HomeworkService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all homework records for the specified company and session.
        /// Optionally includes deleted records.
        /// </summary>
        public List<HomeworkViewModel> GetAll(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HomeworkViewModel>(
                    "sp_Homework_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        IncludeDeleted = includeDeleted
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Retrieves homework details by homework ID.
        /// </summary>
        public HomeworkViewModel? GetByID(int id,int? studentId=null)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<HomeworkViewModel>(
                    "sp_Homework_GetByID",
                    new
                    {
                        HomeworkID = id,
                        StudentID=studentId
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        /// <summary>
        /// Saves a new homework assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> Upsert(HomeworkUpsertRequest request, int companyId, int sessionId, int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@HOMEWORKID", request.HomeworkID);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@CLASSID", request.ClassID);
                parameters.Add("@SECTIONID", request.SectionID);
                parameters.Add("@SUBJECTGROUPID", request.SubjectGroupID);
                parameters.Add("@SUBJECTID", request.SubjectID);
                parameters.Add("@HOMEWORKDATE", request.HomeworkDate);
                parameters.Add("@SUBMISSIONDATE", request.SubmissionDate);
                parameters.Add("@MAXMARKS", request.MaxMarks);
                parameters.Add("@ATTACHMENTPATH", request.AttachmentPath);
                parameters.Add("@DESCRIPTION", request.Description);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "sp_Homework_Upsert",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.HomeWorkId;
                }

                
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save hostel type. Please try again.";
                response.TechnicalMessage = ex.Message;
            }
            return response;
        }



        /// <summary>
        /// Saves a new homework assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> UpsertAttachment(
            HomeworkAttachmentUpsertRequest request,int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@HOMEWORKID", request.HomeworkID);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@ATTACHMENTS_JSON", request.AttachmentsJson);
                parameters.Add("@USERID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_HOMEWORK_ATTACHMENTS_UPSERT",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                }

            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save hostel type. Please try again.";
                response.TechnicalMessage = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// Deletes a homework record by Homework ID.
        /// </summary>
        /// <param name="id">Homework ID.</param>
        /// <param name="userId">User performing the delete action.</param>
        /// <returns>
        /// Tuple containing:
        /// success - Indicates whether the operation was successful.
        /// message - Status message returned from the stored procedure.
        /// </returns>
        public (bool success, string message) Delete(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No homework selected for deletion.");
                }


                using var conn = new SqlConnection(
                   _configuration.GetConnectionString("DefaultConnection"));

                string homeworkId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@HOMEWORKIDS", homeworkId);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "sp_Homework_Delete",
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
        /// Activates or deactivates a homework record.
        /// </summary>
        /// <param name="id">Homework ID.</param>
        /// <param name="isActive">Status to be set.</param>
        /// <param name="userId">User performing the action.</param>
        /// <returns>
        /// Tuple containing:
        /// success - Indicates whether the operation was successful.
        /// message - Status message returned from the stored procedure.
        /// </returns>       

        public (bool success, string message) ToggleStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@HOMEWORKIDS", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Homework_ToggleStatus",
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

        public async Task<PagedResult<HomeworkViewModel>> GetAllHomeWorkWithPage(SearchRequest req)
        {
            var response = new PagedResult<HomeworkViewModel>();
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@MODE", req.Mode);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);
                param.Add("@ClassID", req.ClassID);
                param.Add("@SectionID", req.SectionID);
                param.Add("@STUDENTID", req.StudentId);

                var result = (await conn.QueryAsync<HomeworkViewModel>(
                "SP_HOMEWORK_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                response = new PagedResult<HomeworkViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    response = new PagedResult<HomeworkViewModel>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Unable to fetch homework list. Please try again.";
                response.Data = new List<HomeworkViewModel>();
                return response;
            }
        }

        public List<HomeworkAttachmentViewModel> GetAllHomeWorkAttechmentById(int homeWorkId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HomeworkAttachmentViewModel>(
                    "SP_HOMEWORK_ATTACHMENT_GETBYHOMEWORKID",
                    new
                    {
                        HOMEWORKID = homeWorkId,
                        USERID = userId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #region Home work Submistion flow
        /// <summary>
        /// Submission a new homework assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> UpsertSubmission(
            HomeworkSubmissionUpsertRequest request,int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@SUBMISSIONID", request.SubmissionID);
                parameters.Add("@HOMEWORKID", request.HomeworkID);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@STUDENTID", request.StudentID);
                parameters.Add("@MESSAGE", request.Message);
                parameters.Add("@SUBMITTEDON", request.SubmittedOn);
                parameters.Add("@STATUS", request.Status);
                parameters.Add("@REMARK", request.Remark);
                parameters.Add("@MARKSOBTAINED", request.MarksObtained);
                parameters.Add("@EVALUATIONDATE", request.EvaluationDate);
                parameters.Add("@EVALUATEDBY", userId);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", request.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_HOMEWORK_SUBMISSION_UPSERT",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.SubmissionId;
                }


            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save Home Work Submission type. Please try again.";
                response.TechnicalMessage = ex.Message;
            }
            return response;
        }


        /// <summary>
        /// Submission a new homework assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> UpsertSubmissionAttachment(
            HomeworkSubmissionAttachmentUpsertRequest request, int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@SUBMISSIONID", request.SubmissionID);
                parameters.Add("@ATTACHMENTS_JSON", request.AttachmentsJson);
                parameters.Add("@USERID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_HOMEWORK_SUBMISSION_ATTACHMENTS_UPSERT",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                }

            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save hostel type. Please try again.";
                response.TechnicalMessage = ex.Message;
            }
            return response;
        }

        public async Task<ApiResponse> UpsertEvaluateHomework(
            HomeworkSubmissionEvaluateUpsertRequest request, int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@HOMEWORKID", request.HomeworkID);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@EVALUATIONDATE", request.EvaluationDate);
                parameters.Add("@EVALUATEDBY", userId);
                parameters.Add("@EVALUATIONS_JSON", request.EvaluationsJson);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", request.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_HOMEWORK_SUBMISSION_EVALUATE_UPSERT",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.SubmissionId;
                }


            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save Home Work Submission type. Please try again.";
                response.TechnicalMessage = ex.Message;
            }
            return response;
        }

        public async Task<PagedResult<HomeworkSubmissionListDto>> GetAllHomeWorkSubmissionWithPage(SearchRequest req)
        {
            var response = new PagedResult<HomeworkSubmissionListDto>
            {
                Data = new List<HomeworkSubmissionListDto>()
            };

            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }

                var param = new DynamicParameters();
                param.Add("@HOMEWORKID", req.HomeWorkId);   // was wrongly bound to req.CompanyID
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<HomeworkSubmissionListDto>(
                    "SP_HOMEWORK_SUBMISSION_GETLIST",
                    param,
                    commandType: CommandType.StoredProcedure)).ToList();

                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.CurrentPage ?? req.PageNumber.Value;
                int pageSize = result.FirstOrDefault()?.PageSize ?? req.PageSize;

                // Deserialize each row's Attachments JSON string into AttachmentList
                foreach (var row in result)
                {
                    if (!string.IsNullOrWhiteSpace(row.Attachments))
                    {
                        row.AttachmentList = JsonSerializer.Deserialize<List<HomeworkAttachmentDto>>(
                            row.Attachments,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        ) ?? new List<HomeworkAttachmentDto>();
                    }
                }

                response.Data = res == 0 ? new List<HomeworkSubmissionListDto>() : result;
                response.TotalRecords = totalRecords;
                response.PageNumber = pageIndex;
                response.PageSize = pageSize;
                //response.TotalPages = totalRecords > 0 && pageSize > 0
                //    ? (int)Math.Ceiling((double)totalRecords / pageSize)
                //    : 0;

                return response;
            }
            catch (Exception ex)
            {
                response.Data = new List<HomeworkSubmissionListDto>();
                return response;
            }
        }


        public async Task<List<HomeworkSubmissionDashboardDto>> GetHomeworkSubmissionDashboard(int companyId, int sessionId, int? studentId = null)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                param.Add("@COMPANYID", companyId);
                param.Add("@SESSIONID", sessionId);
                param.Add("@STUDENTID", studentId);

                var result = await conn.QueryAsync<HomeworkSubmissionDashboardDto>(
                    "SP_HOMEWORK_SUBMISSION_DASHBOARD",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch (Exception ex)
            {
                return new List<HomeworkSubmissionDashboardDto>();
            }
        }

        public List<HomeworkAttachmentViewModel> GetAllHomeWorkSubmissionAttechmentById(int submissionID, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HomeworkAttachmentViewModel>(
                    "TBL_HOMEWORK_SUBMISSION_ATTACHMENTS_GETBYHOMEWORKID",
                    new
                    {
                        SubmissionID = submissionID,
                        USERID = userId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        #endregion



    }
}
