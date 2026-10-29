using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class StudentReExamService: IStudentReExamService
    {
        private readonly IConfiguration _configuration;
        public StudentReExamService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<StudentReExamViewModel>> GetStudentReExamAsync(StudentReExamRequestModel request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@ExamGroupID", request.ExamGroupID);



                var result = conn.Query<StudentReExamViewModel>(
                  "sp_StudentReExam_GetBindGrid",
                  parameters,
                  commandType: CommandType.StoredProcedure
              ).ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) SaveStudentReExamAsync(
    StudentReExamRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var rowsTable = new DataTable();

                rowsTable.Columns.Add("StudentID", typeof(int));
                rowsTable.Columns.Add("RollNo", typeof(string));
                rowsTable.Columns.Add("IsReExam", typeof(bool));

                foreach (var row in request.Rows ?? new List<StudentReExamRow>())
                {
                    rowsTable.Rows.Add(
                        row.StudentID,
                        string.IsNullOrWhiteSpace(row.RollNo)
                            ? DBNull.Value
                            : row.RollNo,
                        row.IsReExam
                    );
                }

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@ExamGroupID", request.ExamGroupID);

                parameters.Add(
                    "@Rows",
                    rowsTable.AsTableValuedParameter(
                        "dbo.udt_StudentReExamRow"
                    )
                );

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                var result =  conn.QueryFirstOrDefault<SpResult>(
                    "sp_StudentReExam_Save",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? ""
                );
            }
            catch (Exception ex)
            {
                var result = new SpResult();
                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? ""
                );
            }
        }

        public (bool success, string message) DeleteStudentReExamAsync(List<int> reExamID, int userId, string ipAddress)
        {
            try
            {
                if (reExamID == null || !reExamID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", reExamID);

                var parameters = new DynamicParameters();
                parameters.Add("@ReExamID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_StudentReExam_Delete",
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
