using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class PaymentTypeConfigService : IPaymentTypeConfigService
    {
        private readonly IConfiguration _configuration;
        public PaymentTypeConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SpPaymentType UpsertPaymentTypeConfigAsync(PaymentTypeConfigRequest request)
        {
            var response = new SpPaymentType();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@PaymentTypeId", request.PaymentTypeId);
                parameters.Add("@UserId", request.UserId);
                parameters.Add("@UserName", request.UserName);
                parameters.Add("@IPAddress", request.IPAddress);
                var fieldsTable = new DataTable();

                fieldsTable.Columns.Add("FieldId", typeof(int));
                fieldsTable.Columns.Add("FieldValue", typeof(string));

                foreach (var field in request.Fields)
                {
                    fieldsTable.Rows.Add(
                        field.FieldId.HasValue
                            ? field.FieldId.Value
                            : DBNull.Value,

                        field.FieldValue ?? (object)DBNull.Value
                    );
                }

                parameters.Add(
                    "@Fields",
                    fieldsTable.AsTableValuedParameter("dbo.PaymentTypeConfigTVP")
                );
                var result = conn.QueryFirstOrDefault<SpPaymentType>(
                    "sp_PaymentTypeConfig_SaveBatch",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save cost center. Please try again.";
                response.TECHNICALMESSAGE = ex.Message;
            }

            return response;
        }

        public async Task<(bool Result, string Message)> SavePaymentTypeConfigBatchAsync(
    int? companyId, int paymentTypeId, List<ConfigFieldItem> fields, int? userId, string? userName)
        {
            var table = new DataTable();
            table.Columns.Add("FieldID", typeof(int));
            table.Columns.Add("FieldValue", typeof(string));
            foreach (var f in fields) table.Rows.Add(f.FieldID, f.FieldValue);

            using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));
            var p = new DynamicParameters();
            p.Add("@CompanyID", companyId);
            p.Add("@PaymentTypeId", paymentTypeId);
            p.Add("@Fields", table.AsTableValuedParameter("dbo.PaymentTypeConfigTVP"));
            p.Add("@UserId", userId);
            //p.Add("@UserName", userName);

            var row = await conn.QuerySingleAsync(
                "dbo.sp_PaymentTypeConfig_SaveBatch", p, commandType: CommandType.StoredProcedure);
            return ((int)row.Result == 1, (string)row.Message);
        }

        public async Task<List<FieldConfigResponse>> GetByCompanyPaymentTypeAsync(int paymentTypeId,int companyID)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyID);
                parameters.Add("@PaymentTypeId", paymentTypeId);

                var result = conn.Query<FieldConfigResponse>(
                    "sp_PaymentTypeConfig_GetByCompanyPaymentType",
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
