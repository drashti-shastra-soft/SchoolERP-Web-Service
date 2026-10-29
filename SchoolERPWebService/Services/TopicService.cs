using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class TopicService:ITopicService
    {
        private readonly IConfiguration _configuration;
        public TopicService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves lesson plan details by LessonId ID.
        /// </summary>
        public TopicViewModel? GetByID(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<TopicViewModel>(
                    "SP_MST_TOPIC_GETBYID",
                    new
                    {
                        TopicID = id
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
        /// Saves a new lesson plan assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> Upsert(
            TopicModelRequest request,
            int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                
               
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@TopicID", request.TopicId);
                parameters.Add("@CLASSID", request.ClassID);
                parameters.Add("@SECTIONID", request.SectionID);
                parameters.Add("@SUBJECTGROUPID", request.SubjectGroupID);
                parameters.Add("@SUBJECTID", request.SubjectID);               
                parameters.Add("@LessonMapId", request.LessonId);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@MAP_JSON", request.TopicJson);
                parameters.Add("@USERID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_MST_TOPIC_INSERTUPDATE",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.TopicID;
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

                string topicId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@TopicIds", topicId);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_TOPIC_DELETE",
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
        /// Activates or deactivates a lesson plan record.
        /// </summary>
        /// <param name="id">LessonId.</param>
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
                parameters.Add("@TopicIds", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_TOPIC_TOGGLESTATUS",
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


        public async Task<PagedResult<TopicViewModel>> GetAllTopicWithPage(TopicSearchRequest req)
        {
            var response = new PagedResult<TopicViewModel>();
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
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);
                param.Add("@ClassID", req.ClassID);
                param.Add("@SectionID", req.SectionID);
                param.Add("@LessonID", req.LessonID);
                param.Add("@SubjectGroupID", req.SubjectGroupID);
                param.Add("@SubjectID", req.SubjectID);

                var result = (await conn.QueryAsync<TopicViewModel>(
                "SP_MST_TOPIC_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                response = new PagedResult<TopicViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    response = new PagedResult<TopicViewModel>
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
                response.Data = new List<TopicViewModel>();
                return response;
            }
        }

        public List<TopicViewModel> GetAll(int companyId, int sessionId,int userID, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<TopicViewModel>(
                    "SP_MST_TOPIC_GETALL",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        UserID= userID,
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

        public List<TopicMap> GetAllMapTopic(int companyId, int sessionId, int topicId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<TopicMap>(
                    "TBL_MST_MAP_TOPIC_GETBYID",
                    new
                    {
                        TopicID = topicId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public async Task<PagedResult<LessonSyllabusStatusResponse>> GetAllTopicSyllaBussStatusWithPage(TopicSearchRequest req)
        {
            var response = new PagedResult<LessonSyllabusStatusResponse>();
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
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);
                param.Add("@ClassID", req.ClassID);
                param.Add("@SectionID", req.SectionID);
                param.Add("@SubjectGroupID", req.SubjectGroupID);
                param.Add("@SubjectID", req.SubjectID);

                var result = (await conn.QueryAsync<LessonSyllabusStatusResponse>(
                "SP_MST_LESSON_GETSYLLABUSSTATUS",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.CurrentPage ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                response = new PagedResult<LessonSyllabusStatusResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    response = new PagedResult<LessonSyllabusStatusResponse>
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
                response.Data = new List<LessonSyllabusStatusResponse>();
                return response;
            }
        }


        public (bool success, string message) ToggleTopicCompleteStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@TopicMapIds", request.Ids);
                parameters.Add("@IsCompleted", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_TOPIC_TOGGLECOMPLETIONSTATUS",
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

        public async Task<List<TopicDropDwonResponse>> BindTopicDropDwonList(int lessonMapId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                param.Add("@LessonMapId", lessonMapId);
                var result = (await conn.QueryAsync<TopicDropDwonResponse>(
               "USP_GetTopicMap_ByLessonMapId",
               param,
               commandType: CommandType.StoredProcedure)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
