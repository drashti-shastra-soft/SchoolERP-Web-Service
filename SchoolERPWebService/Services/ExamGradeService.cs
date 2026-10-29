using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    public class ExamGradeService: IExamGradeService
    {
        private readonly IConfiguration _configuration;
        public ExamGradeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<ExamGradeViewModel>> GetByFilterAsync(int companyId, int sessionId, 
            int subjectTypeID,int classID)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@SubjectTypeID", subjectTypeID);
                parameters.Add("@ClassID", classID);

                var result = conn.Query<ExamGradeViewModel>(
                    "SP_EXAMGRADE_GETBYFILTER",
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

        public async Task<ExamGradeResponse?> UpsertExamGradeAsync(
    ExamGradeRequest request)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var gradeRowsJson = JsonSerializer.Serialize(request.GradeRows);

            var parameters = new DynamicParameters();

            parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
            parameters.Add("@SessionID", request.SessionID, DbType.Int32);
            parameters.Add("@SubjectTypeID", request.SubjectTypeID, DbType.Int32);
            parameters.Add("@ClassID", request.ClassID, DbType.Int32);

            parameters.Add(
                "@GradeRowsJSON",
                gradeRowsJson,
                DbType.String);

            parameters.Add("@UserId", request.UserId, DbType.Int32);

            parameters.Add(
                "@IPAddress",
                request.IPAddress,
                DbType.String);

            var result = await connection.QueryFirstOrDefaultAsync<ExamGradeResponse>(
                "SP_EXAMGRADE_UPSERT",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<ExamGradeCopyResponse?> CopyExamGradeAsync(
    ExamGradeCopyRequest request)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add(
                "@CompanyID",
                request.CompanyID,
                DbType.Int32);

            parameters.Add(
                "@SessionID",
                request.SessionID,
                DbType.Int32);

            parameters.Add(
                "@FromSubjectTypeID",
                request.FromSubjectTypeID,
                DbType.Int32);

            parameters.Add(
                "@FromClassID",
                request.FromClassID,
                DbType.Int32);

            parameters.Add(
                "@ToSubjectTypeID",
                request.ToSubjectTypeID,
                DbType.String);
            parameters.Add(
               "@ToClassID",
               request.ToClassID,
               DbType.String);

            parameters.Add(
                "@UserId",
                request.UserId,
                DbType.Int32);

            parameters.Add(
                "@IPAddress",
                request.IPAddress,
                DbType.String);

            var result = await connection.QueryFirstOrDefaultAsync<ExamGradeCopyResponse>(
                "SP_ExamGrade_Copy",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<List<DropdownModel>> GetExamGradeByClassListAsync(
    int companyId,
    int sessionId,
    int classId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionID", sessionId);
            parameters.Add("@ClassID", classId);

            var result = await conn.QueryAsync<DropdownModel>(
                "dbo.SP_ExamGrade_GetByClass",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
    }
}
