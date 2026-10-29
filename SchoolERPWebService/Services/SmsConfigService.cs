using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using System.Data;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing SMS configuration, including saving and retrieving gateway settings from the database.
    /// </summary>
    public class SmsConfigService: ISmsConfigService
    {
        private readonly IConfiguration _configuration;
        public SmsConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves SMS configuration details for the specified company.
        /// </summary>
        public MstSmsConfigViewModel? GetSmsConfig(int companyId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstSmsConfigViewModel>(
                "sp_SmsConfig_Get",
                new
                {
                    CompanyId = companyId
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Saves or updates SMS gateway settings (gateway name, API URL, API key, status, etc.)
        /// for the specified company and returns the operation result.
        /// </summary>
        public (bool success, string message) UpsertSmsConfig(
            MstSmsConfigUpsertRequest request,
            int userId,
            int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_SmsConfig_Upsert",
                    new
                    {
                        CompanyId = companyId,
                        SmsId = request.SmsId,
                        SmsGateway = request.SmsGateway,
                        SmsStatus = request.SmsStatus,
                        SmsApiUrl = request.SmsApiUrl,
                        SmsApiKey = request.SmsApiKey,
                        SMSTypeId = request.SMSTypeId,
                        IsActive = request.IsActive,
                        UserId = userId
                    },
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

        public Dictionary<string, string> GetSmsConfigFields(int companyId, int smsTypeId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
            var rows = conn.Query<FieldKeyValueRow>(
                "dbo.usp_GetSmsConfigFields",
                new { CompanyID = companyId, SMSTypeId = smsTypeId },
                commandType: CommandType.StoredProcedure);

            var fields = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var row in rows)
            {
                // If duplicate FieldKey rows exist for the same company (as seen
                // in the screenshot — duplicated "Api Key" fields), the LAST
                // row returned (per the SP's ORDER BY DisplayOrder) wins. Fix
                // the duplicate rows at the source when you get a chance; this
                // just prevents a crash.
                fields[row.FieldKey] = row.FieldValue ?? "";
            }

            return fields;
        }

        private class FieldKeyValueRow
        {
            public string FieldKey { get; set; } = "";
            public string? FieldValue { get; set; }
        }
    }
}

