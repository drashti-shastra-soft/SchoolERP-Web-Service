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
    /// This service handles the actual work of managing payment method details, such as saving API keys and secrets for payment gateways in the database.
    /// </summary>
    public class PaymentMethodService: IPaymentMethodService
    {
        private readonly IConfiguration _configuration;
        public PaymentMethodService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all payment methods from the database.
        /// Optionally includes deleted records.
        /// </summary>
        public List<MstPaymentMethodViewModel> GetAllPaymentMethods(bool includeDeleted = false)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<MstPaymentMethodViewModel>(
                "sp_PaymentMethods_GetAll",
                new { IncludeDeleted = includeDeleted },
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Retrieves a payment method by its unique identifier.
        /// </summary>
        public MstPaymentMethodViewModel? GetPaymentMethodById(int paymentId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstPaymentMethodViewModel>(
                "sp_PaymentMethods_GetByID",
                new { PaymentId = paymentId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Retrieves the payment method configuration for a specific company.
        /// </summary>
        public MstPaymentMethodViewModel? GetPaymentMethodByCompany(int companyId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstPaymentMethodViewModel>(
                "sp_PaymentMethods_GetByCompany",
                new { CompanyId = companyId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Inserts or updates payment method details.
        /// Returns success status and message from the stored procedure.
        /// </summary>
        public (bool success, string message) UpsertPaymentMethod(MstPaymentMethodUpsertRequest request, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_PaymentMethods_Upsert",
                    new
                    {
                        request.PaymentId,
                        request.CompanyId,
                        request.PaymentTypeId,
                        request.PaymentKey,
                        request.PaymentSecret,
                        request.IsActive,
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

        /// <summary>
        /// Deletes a payment method.
        /// Returns success status and message from the stored procedure.
        /// </summary>
        public (bool success, string message) DeletePaymentMethod(int paymentId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_PaymentMethods_Delete",
                    new
                    {
                        PaymentId = paymentId,
                        UserId = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (Convert.ToInt32(result.Result) == 1,
                        result.Message?.ToString() ?? "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Enables or disables a payment method.
        /// Returns success status and message from the stored procedure.
        /// </summary>
        public (bool success, string message) TogglePaymentMethodStatus(int paymentId, bool isActive, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_PaymentMethods_ToggleStatus",
                    new
                    {
                        PaymentId = paymentId,
                        IsActive = isActive,
                        UserId = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (Convert.ToInt32(result.Result) == 1,
                        result.Message?.ToString() ?? "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<ResolvedPaymentGatewayConfig?> GetConfigAsync(int companyId, int? paymentTypeId = null)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
            using var multi = await conn.QueryMultipleAsync(
                "dbo.SP_PaymentGateway_GetConfig",
                new { CompanyID = companyId, PaymentTypeId = paymentTypeId },
                commandType: CommandType.StoredProcedure);

            var paymentType = await multi.ReadFirstOrDefaultAsync<PaymentTypeInfo>();
            if (paymentType == null || paymentType.PaymentTypeId == 0)
                return null; // "NO ACTIVE PAYMENT METHOD" branch — that row doesn't map to PaymentTypeInfo

            var fields = (await multi.ReadAsync<(string FieldKey, string? FieldValue, bool IsSecret)>()).ToList();

            return new ResolvedPaymentGatewayConfig
            {
                PaymentType = paymentType,
                Settings = fields.ToDictionary(f => f.FieldKey, f => f.FieldValue ?? "", StringComparer.OrdinalIgnoreCase)
            };
        }
    }
}
