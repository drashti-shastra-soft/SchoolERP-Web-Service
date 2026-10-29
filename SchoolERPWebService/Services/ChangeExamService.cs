using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class ChangeExamService: IChangeExamService
    {
        private readonly IConfiguration _configuration;
        public ChangeExamService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<StudentResponse>> GetChangeExamAsync(ChangeExamRequest request)
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
                parameters.Add("@MarkTypeID", request.MarkTypeID);
                parameters.Add("@ExamID", request.ExamID);



                var result = conn.Query<StudentResponse>(
                  "sp_ChangeExam_GetBindGrid",
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

        public (bool success, string message) UpsertChangeExamAsync(ExamMappingRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@MarkTypeID", request.MarkTypeID);
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@OldExamID", request.OldExamID);
                parameters.Add("@NewExamID", request.NewExamID);
                parameters.Add("@StudentIDsJson", request.StudentIDsJson);
                parameters.Add("@UserID", request.UserID);


                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_ChangeExam_Save",
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
