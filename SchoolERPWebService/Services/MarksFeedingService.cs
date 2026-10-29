using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class MarksFeedingService: IMarksFeedingService
    {
        private readonly IConfiguration _configuration;
        public MarksFeedingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<MarksFeedingResponse>> GetAllMarksFeedingBindGridAsync(MarksFeedingGetRequest Req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", Req.CompanyID);
                parameters.Add("@SessionID", Req.SessionID);
                parameters.Add("@ClassID", Req.ClassID);
                parameters.Add("@SectionID", Req.SectionID);
                parameters.Add("@SubjectID", Req.SubjectID);
                parameters.Add("@ExamID", Req.ExamID);

                var result = conn.Query<MarksFeedingResponse>(
                    "sp_MarksFeeding_GetBindGrid",
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

        public (bool success, string message) DeleteMarksFeeding(List<int> marksFeedingID, int userId, string ipAddress)
        {
            try
            {
                if (marksFeedingID == null || !marksFeedingID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", marksFeedingID);

                var parameters = new DynamicParameters();
                parameters.Add("@MarksFeedingID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_MarksFeeding_Delete",
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

        public (bool success, string message) UpsertMarksFeeding(MarksFeedingRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var rowsTable = new DataTable();

                rowsTable.Columns.Add("StudentID", typeof(int));
                rowsTable.Columns.Add("AttendanceStatus", typeof(decimal));
                rowsTable.Columns.Add("Marks", typeof(decimal));
                rowsTable.Columns.Add("PracticalMarks", typeof(decimal));
                rowsTable.Columns.Add("Grade", typeof(decimal));

                foreach (var row in request.Rows)
                {
                    rowsTable.Rows.Add(
                        row.StudentID,
                        row.AttendanceStatus,
                        row.Marks,
                        row.PracticalMarks,
                        row.Grade
                    );
                }
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@SubjectID", request.SubjectID);
                parameters.Add("@ExamID", request.ExamID);

                parameters.Add(
                    "@Rows",
                    rowsTable.AsTableValuedParameter("dbo.udt_MarksFeedingRow")
                );

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);


                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_MarksFeeding_Save",
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
    }
}
