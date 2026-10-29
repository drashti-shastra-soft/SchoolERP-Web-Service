using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class ManageLessonPlanService: IManageLessonPlanService
    {
        private readonly IConfiguration _configuration;
        public ManageLessonPlanService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Saves a new lesson plan assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> Upsert(
            ManageLessonPlanRequest request,
            int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@LESSONPLANID", request.LessonPlanId);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@CLASSID", request.ClassID);
                parameters.Add("@SECTIONID", request.SectionID);
                parameters.Add("@SUBJECTGROUPID", request.SubjectGroupID);
                parameters.Add("@SUBJECTID", request.SubjectID);
                parameters.Add("@LESSONMAPID", request.LessonMapId);
                parameters.Add("@TOPICMAPID", request.TopicMapId);
                parameters.Add("@SUBTOPIC", request.SubTopic);
                parameters.Add("@TimeTableID", request.TimeTableID);
                parameters.Add("@PLANDATE", request.PlanDate);
                parameters.Add("@TIMEFROM", request.TimeFrom);
                parameters.Add("@TIMETO", request.TimeTo);
                parameters.Add("@LECTUREYOUTUBEURL", request.LectureYoutubeUrl);
                parameters.Add("@LECTUREVIDEOPATH", request.LectureVideoPath);
                parameters.Add("@ATTACHMENTPATH", request.AttachmentPath);
                parameters.Add("@TEACHINGMETHOD", request.TeachingMethod);
                parameters.Add("@GENERALOBJECTIVES", request.GeneralObjectives);
                parameters.Add("@PREVIOUSKNOWLEDGE", request.PreviousKnowledge);
                parameters.Add("@COMPREHENSIVEQUESTIONS", request.ComprehensiveQuestions);
                parameters.Add("@PRESENTATION", request.Presentation);
                parameters.Add("@STATUS", request.Status);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", request.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_MST_LESSON_PLAN_UPSERT",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.LESSONPLANID;
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
        /// Retrieves lesson plan details by LessonId ID.
        /// </summary>
        public ManageLessonPlanViewModel? GetByID(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<ManageLessonPlanViewModel>(
                    "SP_MST_LESSON_PLAN_GETBYID",
                    new
                    {
                        LessonplanId = id
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<PagedResult<ManageLessonPlanViewModel>> GetAllLessonPlanWithPage(ManageLessonPlanSearchRequest request)
        {
            var response = new PagedResult<ManageLessonPlanViewModel>();
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                if (request.PageNumber == 0 && request.PageSize == 0)
                {
                    request.PageNumber = 1;
                    request.PageSize = 10;
                }
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@CLASSID", request.ClassID);
                parameters.Add("@SECTIONID", request.SectionID);
                parameters.Add("@SUBJECTID", request.SubjectID);
                parameters.Add("@INCLUDEDELETED", request.IncludeDeleted);
                parameters.Add("@SEARCHKEYWORD", request.SearchKeyword);
                parameters.Add("@PAGENUMBER", request.PageNumber);
                parameters.Add("@PAGESIZE", request.PageSize);
                parameters.Add("@USERID", request.UserID);
                parameters.Add("@IPADDRESS", request.IPAddress);
                var result = (await conn.QueryAsync<ManageLessonPlanViewModel>(
                "SP_MST_LESSON_PLAN_GETALLWITHPAGEINDEX",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                response = new PagedResult<ManageLessonPlanViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    response = new PagedResult<ManageLessonPlanViewModel>
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
                response.Message = "Unable to fetch lesson plan list. Please try again.";
                response.Data = new List<ManageLessonPlanViewModel>();
                return response;
            }
        }

        /// <summary>
        /// Deletes a lesson plan record by LessonId.
        /// </summary>
        /// <param name="id">LessonId.</param>
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

                string lessonplanId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@LessonplanId", lessonplanId);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_LESSON_DELETE",
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

        public async Task<ApiResponse> UpsertAttachment(
            ManageLessonPlanAttachmentUpsertRequest request, int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@LESSONPLANID", request.LessonPlanId);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@LECTUREVIDEOPATH", request.LectureVideoPath);
                parameters.Add("@ATTACHMENTPATH", request.AttachmentPath);
                parameters.Add("@USERID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_MST_LESSON_PLAN_UPDATE_ATTACHMENT_PATHS",
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
        /// Retrieves lesson plan details by LessonId ID.
        /// </summary>
        public LessonPlanViewModel? GetLessonPlanDetailById(int lessonPlanId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<LessonPlanViewModel>(
                    "USP_GetLessonPlanDetailById",
                    new
                    {
                        LessonPlanId = lessonPlanId
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public async Task<ApiResponse> UpsertLessonPlanCommit(
           LessonPlanCommentRequest request,
           int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@LESSONPLANID", request.LessonPlanId);
                parameters.Add("@COMMENTTEXT", request.CommentText);
                parameters.Add("@COMMENTEDBY", request.CommentedBy ?? userId);
                parameters.Add("@COMMENTERTYPE", request.CommenterType);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", request.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_MST_LESSON_PLAN_COMMENT_INSERT",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.LESSONPLANID;
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

        public List<LessonPlanCommentResponse> GetAllCommentList(int companyId, int sessionId,int lessonPlanId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));


                var parameters = new DynamicParameters();
                parameters.Add("@LESSONPLANID", lessonPlanId);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);

                return conn.Query<LessonPlanCommentResponse>(
                    "SP_MST_LESSON_PLAN_COMMENT_LIST",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
