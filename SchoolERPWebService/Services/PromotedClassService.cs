using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class PromotedClassService:IPromotedClassService
    {
        private readonly IConfiguration _configuration;
        public PromotedClassService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<PromotedClassViewModel>> GetPromotedToClassAsync(PromotedToClassGetAllRequest request)
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



                var result = conn.Query<PromotedClassViewModel>(
                  "sp_PromotedToClass_GetBindGrid",
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


        public (bool success, string message) UpsertPromotedToClassAsync(PromotedToClassRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var rowsTable = new DataTable();

                rowsTable.Columns.Add("StudentID", typeof(int));
                rowsTable.Columns.Add("PromotedToClassID", typeof(int));
                rowsTable.Columns.Add("PromotedToSectionID", typeof(int));
                rowsTable.Columns.Add("Result", typeof(string));

                foreach (var row in request.Rows)
                {
                    rowsTable.Rows.Add(
                        row.StudentID,
                        row.PromotedToClassID.HasValue
                            ? row.PromotedToClassID.Value
                            : DBNull.Value,
                        row.PromotedToSectionID.HasValue
                            ? row.PromotedToSectionID.Value
                            : DBNull.Value,
                        string.IsNullOrEmpty(row.Result)
                            ? DBNull.Value
                            : row.Result
                    );
                }
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@TargetSessionID", request.TargetSessionID);

                parameters.Add(
                    "@Rows",
                    rowsTable.AsTableValuedParameter("dbo.udt_PromotedToClassRow")
                );

                parameters.Add("@UserId", request.UserId);
                parameters.Add("@IPAddress", request.IPAddress);


                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_PromotedToClass_Save",
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



        public (bool success, string message) DeletePromotedToClassAsync(List<int> promotionID, int userId, string ipAddress)
        {
            try
            {
                if (promotionID == null || !promotionID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", promotionID);

                var parameters = new DynamicParameters();
                parameters.Add("@PromotionID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_PromotedToClass_Delete",
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
