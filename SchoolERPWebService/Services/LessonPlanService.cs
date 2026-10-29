using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class LessonPlanService:ILessonPlanService
    {
        private readonly IConfiguration _configuration;
        public LessonPlanService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves lesson plan details by LessonId ID.
        /// </summary>
        public LessonPlanModel? GetByID(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<LessonPlanModel>(
                    "SP_MST_LESSON_GETBYID",
                    new
                    {
                        LessonID = id
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
        public async Task<ApiResponse> Upsert(LessonPlanModelRequest request,int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@LessonID", request.LessonId);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@CLASSID", request.ClassID);
                parameters.Add("@SECTIONID", request.SectionID);
                parameters.Add("@SUBJECTGROUPID", request.SubjectGroupID);
                parameters.Add("@SUBJECTID", request.SubjectID);
                parameters.Add("@MAP_JSON", request.LessonJson);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_MST_LESSON_INSERTUPDATE",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.LessonId;
                }


            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save Lesson Plan. Please try again.";
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

                string lessionId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@LessonIds", lessionId);
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
                parameters.Add("@LessonIds", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_LESSON_TOGGLESTATUS",
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

        public async Task<PagedResult<LessonPlanModel>> GetAllLessonPlanWithPage(LessonPlanSearchRequest req)
        {
            var response = new PagedResult<LessonPlanModel>();
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
                param.Add("@SubjectGroupID", req.SubjectGroupID);
                param.Add("@SubjectID", req.SubjectID);

                var result = (await conn.QueryAsync<LessonPlanModel>(
                "SP_MST_LESSON_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                response = new PagedResult<LessonPlanModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    response = new PagedResult<LessonPlanModel>
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
                response.Data = new List<LessonPlanModel>();
                return response;
            }
        }

        public List<LessonPlanModel> GetAll(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<LessonPlanModel>(
                    "SP_MST_LESSON_GETALL",
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

        public List<LessonPlanMap> GetAllMapLesson(int companyId, int sessionId, int lessonId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<LessonPlanMap>(
                    "TBL_MST_MAP_LESSON_GETBYID",
                    new
                    {
                        LessonID = lessonId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        public async Task<List<LessonDropDwonResponse>> BindLessonDropDwonList(LessonDropDwonReq req,int? userId=null)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@ClassID", req.ClassID);
                param.Add("@SectionID", req.SectionID);
                param.Add("@SubjectGroupID", req.SubjectGroupID);
                param.Add("@SubjectID", req.SubjectID);
                param.Add("@UserID", userId);
                var result = (await conn.QueryAsync<LessonDropDwonResponse>(
               "SP_MST_LESSON_GETDROPDOWN",
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
