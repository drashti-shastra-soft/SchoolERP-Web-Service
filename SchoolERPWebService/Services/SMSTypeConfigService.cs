using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class SMSTypeConfigService:ISMSTypeConfigService
    {
        private readonly IConfiguration _configuration;
        public SMSTypeConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool Result, string Message)> SaveSMSTypeConfigBatchAsync(
    int? companyId, int smsTypeId, List<ConfigFieldItem> fields, int? userId)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("FieldID", typeof(int));
                table.Columns.Add("FieldValue", typeof(string));
                foreach (var f in fields) table.Rows.Add(f.FieldID, f.FieldValue);

                using var conn = new SqlConnection(
                         _configuration.GetConnectionString("DefaultConnection"));
                var p = new DynamicParameters();
                p.Add("@CompanyID", companyId);
                p.Add("@SMSTypeId", smsTypeId);
                p.Add("@Fields", table.AsTableValuedParameter("dbo.SMSTypeConfigTVP"));
                p.Add("@UserId", userId);

                var row = await conn.QuerySingleAsync(
                    "dbo.sp_SMSTypeConfig_SaveBatch", p, commandType: CommandType.StoredProcedure);
                return ((int)row.Result == 1, (string)row.Message);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task<List<FieldConfigResponse>> GetByCompanysmsTypeAsync(int smsTypeId, int companyID)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyID);
                parameters.Add("@SMSTypeId", smsTypeId);

                var result = conn.Query<FieldConfigResponse>(
                    "sp_SMSTypeConfig_GetByCompanySMSType",
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
    }
}
