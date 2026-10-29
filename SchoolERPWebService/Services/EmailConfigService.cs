using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using System.Data;
using System.Reflection;
using SchoolERPWebService.Domain.Common;
using System.Threading.Tasks;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing email configuration, including saving and retrieving server settings from the database.
    /// </summary>
    public class EmailConfigService: IEmailConfigService
    {
        private readonly IConfiguration _configuration;
        public EmailConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves the current email server settings from the database.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>
        /// Returns the email configuration if found; otherwise, null.
        /// </returns>
        public MstEmailConfigViewModel? GetEmailConfig(int companyId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstEmailConfigViewModel>(
                "sp_EmailConfig_Get",
                new
                {
                    CompanyId = companyId
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Inserts or updates email configuration details.
        /// Calls sp_EmailConfig_Upsert and returns operation status with message.
        /// </summary>
        public async Task<(bool success, string message)> UpsertEmailConfig(MstEmailConfigUpsertRequest request, int userId,int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyId", companyId);
                parameters.Add("@EmailId", request.EmailId);
                parameters.Add("@EmailType", request.EmailType);
                parameters.Add("@SMTPServer", request.SMTPServer);
                parameters.Add("@SMTPPort", request.SMTPPort);
                parameters.Add("@SMTPUsername", request.SMTPUsername);
                parameters.Add("@SMTPPassword", request.SMTPPassword);
                parameters.Add("@SslTls", request.SslTls);
                parameters.Add("@SMTPAuth", request.SMTPAuth);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", userId);

                var result =await  conn.QueryFirstOrDefaultAsync<SpResult>(
                    "sp_EmailConfig_Upsert",
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
