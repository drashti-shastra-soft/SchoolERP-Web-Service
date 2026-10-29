using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class ExamMarksheetDownloadPermissionServices: IExamMarksheetDownloadPermissionServices
    {
        private readonly IConfiguration _configuration;
        public ExamMarksheetDownloadPermissionServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<ExamMarksheetDownloadPermissionViewModel>> GetAllExamMarksheetDownloadPermission(ExamGroupGetAllRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionId", request.SessionId);
                parameters.Add("@ClassId", request.ClassId);
                parameters.Add("@ExamGroupId", request.ExamGroupId);
                parameters.Add("@PageNo", request.PageNo);
                parameters.Add("@PageSize", request.PageSize);

                var result = conn.Query<ExamMarksheetDownloadPermissionViewModel>(
                    "sp_ExamMarksheetDownloadPermission_GetAll",
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

        public async Task<ApiResponse> SavePermissionAsync(PermissionRequest request)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@PermissionId", request.PermissionId);
            parameters.Add("@CompanyID", request.CompanyID);
            parameters.Add("@SessionId", request.SessionId);
            parameters.Add("@ClassId", request.ClassId);
            parameters.Add("@ExamGroupId", request.ExamGroupId);
            parameters.Add("@DueMonthNo", request.DueMonthNo);
            parameters.Add("@MaxBalance", request.MaxBalance);
            parameters.Add("@AllowMarksheet", request.AllowMarksheet);
            parameters.Add("@AllowStripsheet", request.AllowStripsheet);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@UserName", request.UserName);

            var result = await connection.QueryFirstOrDefaultAsync<ApiResponse>(
                "sp_ExamMarksheetDownloadPermission_Save",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new ApiResponse
            {
                Result = 0,
                Message = "No response returned from stored procedure."
            };
        }

        public async Task<ExamMarksheetDownloadPermissionSetAccessResponse> SetAccessAsync(
    ExamMarksheetDownloadPermissionSetAccessRequest request)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@Ids", request.Ids);
            parameters.Add("@Target", request.Target);
            parameters.Add("@Allow", request.Allow);
            parameters.Add("@CompanyID", request.CompanyID);
            parameters.Add("@UserId", request.UserId);
            parameters.Add("@UserName", request.UserName);

            var result = await connection.QueryFirstOrDefaultAsync<
                ExamMarksheetDownloadPermissionSetAccessResponse>(
                "dbo.sp_ExamMarksheetDownloadPermission_SetAccess",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? new ExamMarksheetDownloadPermissionSetAccessResponse
            {
                Success = false,
                Message = "No response returned from stored procedure.",
                Id = 0
            };
        }

        public (bool success, string message) Delete(DeleteRequest request,int userId)
        {
            try
            {
                
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", request.Ids);

                var parameters = new DynamicParameters();
                parameters.Add("@Ids", id);
                parameters.Add("@CompanyID", request.CompanyId);
                parameters.Add("@UserId", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_ExamMarksheetDownloadPermission_Delete",
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
