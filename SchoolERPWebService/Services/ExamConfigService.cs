using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.ComponentModel.Design;

namespace SchoolERPWebService.Services
{
    public class ExamConfigService:IExamConfigService
    {
        private readonly IConfiguration _configuration;
        public ExamConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (bool success, string message) UpsertExamConfig(ExamConfigRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@ExamConfigID", request.ExamConfigID, DbType.Int32);
                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@SessionID", request.SessionID, DbType.Int32);
                parameters.Add("@ClassID", request.ClassID, DbType.Int32);
                parameters.Add("@ExamGroupID", request.ExamGroupID, DbType.Int32);
                parameters.Add("@ExamID", request.ExamID, DbType.Int32);

                parameters.Add("@MargeExam", request.MargeExam, DbType.String);

                parameters.Add("@Percentage", request.Percentage, DbType.Decimal);
                parameters.Add("@Sequence", request.Sequence, DbType.Int32);

                parameters.Add("@UserId", request.UserId, DbType.Int32);
                parameters.Add("@IPAddress", request.IPAddress, DbType.String);



                var result = conn.QueryFirstOrDefault<SpResult>(
               "SP_EXAMCONFIG_UPSERT",
               parameters,
               commandType: CommandType.StoredProcedure);



                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? ""
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool success, string message) DeleteExamConfig(List<int> examConfigID, int userId, string ipAddress)
        {
            try
            {
                if (examConfigID == null || !examConfigID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", examConfigID);

                var parameters = new DynamicParameters();
                parameters.Add("@ExamConfigID", id);
                parameters.Add("@UserId", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_ExamConfig_Delete",
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

        public async Task<(bool Success, string Message)> CopyExamConfigToSession(ExamConfigCopyRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@FromSessionID", request.FromSessionID, DbType.Int32);
                parameters.Add("@FromClassID", request.FromClassID, DbType.Int32);
                parameters.Add("@ToSessionID", request.ToSessionID, DbType.Int32);
                parameters.Add("@ToClassIDs", request.ToClassIDs, DbType.String);
                parameters.Add("@UserId", request.UserId, DbType.Int32);
                parameters.Add("@IPAddress", request.IPAddress, DbType.String);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_EXAMCONFIG_COPY",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                SpResult model = new SpResult();
                model.Result = 0;
                model.Message = ex.Message;
                return (
                    Convert.ToInt32(model.Result) == 1,
                    Convert.ToString(model.Message) ?? string.Empty
                );
            }

        }

        public async Task<List<ExamConfigViewModel>> GetAllExamConfigByClassIdAsync(int companyId, int sessionId, int classID)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@ClassID", classID);

                var result = conn.Query<ExamConfigViewModel>(
                    "SP_EXAMCONFIG_GETBYCLASS",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public ExamConfigViewModel? GetExamConfigByID(int examConfigId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<ExamConfigViewModel>(
                    "SP_EXAMCONFIG_GETBYID",
                    new
                    {
                        EXAMCONFIGID = examConfigId
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<PagedResult<ExamConfigViewModel>> GetAllExamConfigWithPage(ExamConfigListRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@COMPANYID", request.CompanyID, DbType.Int32);
                parameters.Add("@SESSIONID", request.SessionID, DbType.Int32);
                parameters.Add("@CLASSID", request.ClassID, DbType.Int32);
                parameters.Add("@SEARCHTEXT", request.SearchText, DbType.String);
                parameters.Add("@ISACTIVE", request.IsActive, DbType.Boolean);

                parameters.Add("@PAGEINDEX", request.PageIndex, DbType.Int32);
                parameters.Add("@PAGESIZE", request.PageSize, DbType.Int32);

                parameters.Add("@SORTCOLUMN", request.SortColumn, DbType.String);
                parameters.Add("@SORTDIRECTION", request.SortDirection, DbType.String);

                parameters.Add(
                    "@TOTALRECORDS",
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output
                );

                var result = (await conn.QueryAsync<ExamConfigViewModel>(
                "SP_EXAMCONFIG_GETPAGEINDEX",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<ExamConfigViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ExamConfigViewModel>
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

        public (bool success, string message) ToggleExamStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ExamConfigID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_ExamConfig_ToggleStatus",
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

        public async Task<List<SubjectWithPaperMarksResponse>> GetAllExamSubjectPaperMarksListAsyc(int companyId, 
            int sessionId, int classID,int sectionId,int examId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@CLASSID", classID);
                parameters.Add("@SECTIONID", sectionId);
                parameters.Add("@EXAMID", examId);

                var result = conn.Query<SubjectWithPaperMarksResponse>(
                    "SP_GET_SUBJECTS_WITH_PAPERMARKS_BY_CLASS_SECTION_EXAM",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<List<SubjectWithPaperMarksResponse>> GetAllExamBySubjectIdListAsyc(int companyId,
            int sessionId, int classID, int subjectID)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@ClassID", classID);
                parameters.Add("@SubjectID", subjectID);

                var result = conn.Query<SubjectWithPaperMarksResponse>(
                    "SP_EXAMCONFIG_GETBYSUBJECT",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
