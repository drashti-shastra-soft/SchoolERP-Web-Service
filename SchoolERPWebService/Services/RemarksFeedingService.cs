using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class RemarksFeedingService :IRemarksFeedingService

    {
        private readonly IConfiguration _configuration;
        public RemarksFeedingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<RemarksFeedingViewModel>> GetRemarksFeedingAsync(AttendanceFeedingRequest request)
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
                parameters.Add("@ExamID", request.ExamID);



                var result = conn.Query<RemarksFeedingViewModel>(
                  "sp_RemarksFeeding_GetBindGrid",
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

       
        public (bool success, string message) UpsertRemarksFeedingAsync(RemarksFeedingRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var rowsTable = new DataTable();

                rowsTable.Columns.Add("StudentID", typeof(int));
                rowsTable.Columns.Add("Remark", typeof(string));

                foreach (var row in request.Rows)
                {
                    rowsTable.Rows.Add(
                        row.StudentID,
                        string.IsNullOrEmpty(row.Remark)
                            ? DBNull.Value
                            : row.Remark
                    );
                }
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@ExamID", request.ExamID);

                parameters.Add(
                    "@Rows",
                    rowsTable.AsTableValuedParameter("dbo.udt_RemarksFeedingRow")
                );

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);


                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_RemarksFeeding_Save",
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

       

        public (bool success, string message) DeleteRemarksFeedingAsync(List<int> remarksFeedingID, int userId, string ipAddress)
        {
            try
            {
                if (remarksFeedingID == null || !remarksFeedingID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", remarksFeedingID);

                var parameters = new DynamicParameters();
                parameters.Add("@RemarksFeedingID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_RemarksFeeding_Delete",
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
