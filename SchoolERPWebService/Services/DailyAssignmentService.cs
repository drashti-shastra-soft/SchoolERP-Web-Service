using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    public class DailyAssignmentService:IDailyAssignmentService
    {
        private readonly IConfiguration _configuration;
        public DailyAssignmentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Retrieves homework details by homework ID.
        /// </summary>
        public DailyAssignmentModel? GetByID(int assignmentID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<DailyAssignmentModel>(
                    "SP_TBL_MST_DAILY_ASSIGNMENT_GETBYID",
                    new
                    {
                        AssignmentID = assignmentID
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
        /// Saves a new Dealy assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> Upsert(
            AssignmentUpsertRequest request,
            int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@ASSIGNMENTID", request.AssignmentID);
                parameters.Add("@STUDENTID", request.StudentID);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@CLASSID", request.ClassID);
                parameters.Add("@SECTIONID", request.SectionID);
                parameters.Add("@SUBJECTGROUPID", request.SubjectGroupID);
                parameters.Add("@SUBJECTID", request.SubjectID);
                parameters.Add("@TITLE", request.Title);
                parameters.Add("@DESCRIPTION", request.Description);
                parameters.Add("@ASSIGNMENTDATE", request.AssignmentDate);
                parameters.Add("@REMARK", request.Remark);
                parameters.Add("@EVALUATIONDATE", request.EvaluationDate);
                parameters.Add("@EVALUATEDBY", request.EvaluationDate.HasValue ? userId : (int?)null);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", request.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_DAILY_ASSIGNMENT_UPSERT",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                    response.Data = result.AssignmentId;
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
        /// Saves a new homework daily assignment or updates an existing one.
        /// It records the subject, description, attachment, marks, and submission date.
        /// </summary>
        public async Task<ApiResponse> UpsertAttachment(
            AssignmentAttachmentUpsertRequest request, int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@ASSIGNMENTID", request.AssignmentID);
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@ATTACHMENTS_JSON", request.AttachmentsJson);
                parameters.Add("@USERID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "SP_DAILY_ASSIGNMENT_ATTACHMENTS_UPSERT",
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


        public async Task<PagedResult<DailyAssignmentModel>> GetAllDailyAssignmentWithPage(DailyAssignmentSearchRequest req, int userId)
        {
            var response = new PagedResult<DailyAssignmentModel>();
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
                param.Add("@STUDENTID", req.StudentId);
                param.Add("@USERID", userId);
                var result = (await conn.QueryAsync<DailyAssignmentModel>(
                "SP_DAILY_ASSIGNMENT_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                var first = result.FirstOrDefault();
               
                

                // Deserialize each row's attachments JSON into AttachmentList
                // Deserialize each row's attachments JSON into AttachmentList
                foreach (var row in result)
                {
                    if (!string.IsNullOrWhiteSpace(row.AttachmentsJson))
                    {
                        row.AttachmentList = JsonSerializer.Deserialize<List<DailyAssignmentAttachmentDto>>(
                            row.AttachmentsJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        ) ?? new List<DailyAssignmentAttachmentDto>();
                    }
                }

                // "No records found" row has AssignmentID = NULL — filter it out of Items
                response.Data = result.Where(r => r.AssignmentID.HasValue).ToList();
                response.TotalRecords = first?.TotalRecords ?? 0;
                response.PageSize = first?.PageSize ?? req.PageSize;

                
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Unable to fetch homework list. Please try again.";
                response.Data = new List<DailyAssignmentModel>();
                return response;
            }
        }


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

                string assignmentId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@ASSIGNMENTID", assignmentId);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_DAILY_ASSIGNMENT_DELETE",
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
    }
}
