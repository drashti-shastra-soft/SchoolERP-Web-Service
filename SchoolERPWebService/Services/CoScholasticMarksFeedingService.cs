using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class CoScholasticMarksFeedingService: ICoScholasticMarksFeedingService
    {
        private readonly IConfiguration _configuration;
        public CoScholasticMarksFeedingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (bool success, string message) UpsertCoScholasticMarksFeeding(CoScholasticMarksFeedingRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var rowsTable = new DataTable();

                rowsTable.Columns.Add("StudentID", typeof(int));
                rowsTable.Columns.Add("ExamGradeID", typeof(decimal));

                foreach (var row in request.Rows)
                {
                    rowsTable.Rows.Add(
                        row.StudentID,
                        row.ExamGradeID
                    );
                }
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@CoScholasticID", request.CoScholasticID);
                parameters.Add("@ExamID", request.ExamID);

                parameters.Add(
                    "@Rows",
                    rowsTable.AsTableValuedParameter("dbo.udt_CoScholasticMarksFeedingRow_v2")
                );

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);


                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_CoScholasticMarksFeeding_Save",
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

        public async Task<List<CoScholasticMarksFeedingResponse>> GetAllCoScholasticMarksFeedingAsync(CoScholasticMarksFeedingGetRequest Req)
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
                parameters.Add("@CoScholasticID", Req.CoScholasticID);
                parameters.Add("@ExamID", Req.ExamID);

                var result = conn.Query<CoScholasticMarksFeedingResponse>(
                    "sp_CoScholasticMarksFeeding_GetBindGrid",
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

        public (bool success, string message) DeleteMarksFeeding(List<int> coScholasticMarksFeedingID, int userId, string ipAddress)
        {
            try
            {
                if (coScholasticMarksFeedingID == null || !coScholasticMarksFeedingID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", coScholasticMarksFeedingID);

                var parameters = new DynamicParameters();
                parameters.Add("@CoScholasticMarksFeedingID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_CoScholasticMarksFeeding_Delete",
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
