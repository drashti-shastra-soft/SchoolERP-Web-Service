using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class PaperMarksDetailsService : IPaperMarksDetailsService
    {
        private readonly IConfiguration _configuration;
        public PaperMarksDetailsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (bool success, string message) UpsertPaperMarksDetails(PaperMarksDetailsSaveRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var rowsTable = new DataTable();

                rowsTable.Columns.Add("SubjectID", typeof(int));
                rowsTable.Columns.Add("TheoryMaxMark", typeof(decimal));
                rowsTable.Columns.Add("TheoryMinMark", typeof(decimal));
                rowsTable.Columns.Add("PracticalMaxMark", typeof(decimal));
                rowsTable.Columns.Add("PracticalMinMark", typeof(decimal));

                foreach (var row in request.Rows)
                {
                    rowsTable.Rows.Add(
                        row.SubjectID,
                        row.TheoryMaxMark,
                        row.TheoryMinMark,
                        row.PracticalMaxMark,
                        row.PracticalMinMark
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
                    rowsTable.AsTableValuedParameter("dbo.udt_PaperMarksDetailRow")
                );

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);


                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_PaperMarksDetails_Save",
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

        public PaperMarksDetailsViewModel? GetPaperMarksDetailsByID(int paperMarksDetailID)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<PaperMarksDetailsViewModel>(
                    "sp_PaperMarksDetails_GetByID",
                    new
                    {
                        PaperMarksDetailID = paperMarksDetailID
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeletePaperMarksDetails(List<int> paperMarksDetailID, int userId, string ipAddress)
        {
            try
            {
                if (paperMarksDetailID == null || !paperMarksDetailID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", paperMarksDetailID);

                var parameters = new DynamicParameters();
                parameters.Add("@PaperMarksDetailID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_PaperMarksDetails_Delete",
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

        public (bool success, string message) CopyPaperMarksDetails(PaperMarksDetailsCopyRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);

                parameters.Add("@SourceClassID", request.SourceClassID);
                parameters.Add("@SourceSectionID", request.SourceSectionID);
                parameters.Add("@SourceExamID", request.SourceExamID);

                parameters.Add("@TargetClassID", request.TargetClassID);
                parameters.Add("@TargetSectionID", request.TargetSectionID);
                parameters.Add("@TargetExamID", request.TargetExamID);

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);


                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_PaperMarksDetails_Copy",
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
