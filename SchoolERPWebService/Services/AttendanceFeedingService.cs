using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class AttendanceFeedingService : IAttendanceFeedingService
    {
        private readonly IConfiguration _configuration;
        public AttendanceFeedingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<AttendanceFeedingViewModel>> GetAttendanceFeedingAsync(AttendanceFeedingRequest request)
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

               

                var result = conn.Query<AttendanceFeedingViewModel>(
                  "sp_AttendanceFeeding_GetBindGrid",
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

        public (bool success, string message) DeleteAttendanceFeeding(List<int> attendanceFeedingID, int userId, string ipAddress)
        {
            try
            {
                if (attendanceFeedingID == null || !attendanceFeedingID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", attendanceFeedingID);

                var parameters = new DynamicParameters();
                parameters.Add("@AttendanceFeedingID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_AttendanceFeeding_Delete",
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

        public (bool success, string message) UpsertAttendanceFeeding(AttendanceFeedingAddRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var rowsTable = new DataTable();

                rowsTable.Columns.Add("StudentID", typeof(int));
                rowsTable.Columns.Add("Attendance", typeof(decimal));
                rowsTable.Columns.Add("SchoolDays", typeof(decimal));

                foreach (var row in request.Rows)
                {
                    rowsTable.Rows.Add(
                        row.StudentID,
                        row.Attendance.HasValue ? row.Attendance.Value : DBNull.Value,
                        row.SchoolDays.HasValue ? row.SchoolDays.Value : DBNull.Value
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
                    rowsTable.AsTableValuedParameter("dbo.udt_AttendanceFeedingRow")
                );

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);

                
                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_AttendanceFeeding_Save",
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
