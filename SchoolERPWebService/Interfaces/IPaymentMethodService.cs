using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing the different ways payments can be made in the system (like Razorpay or other gateways).
    /// </summary>
    public interface IPaymentMethodService
    {
        /// <summary>
        /// Gets a list of all payment methods available in the system.
        /// </summary>
        List<MstPaymentMethodViewModel> GetAllPaymentMethods(bool includeDeleted = false);

        /// <summary>
        /// Finds the details of a specific payment method using its unique ID.
        /// </summary>
        MstPaymentMethodViewModel? GetPaymentMethodById(int paymentId);

        /// <summary>
        /// Finds the details of the payment method configured for a specific company.
        /// </summary>
        MstPaymentMethodViewModel? GetPaymentMethodByCompany(int companyId);

        /// <summary>
        /// Adds a new payment method or updates an existing one with keys and secrets.
        /// </summary>
        (bool success, string message) UpsertPaymentMethod(MstPaymentMethodUpsertRequest request, int userId);

        /// <summary>
        /// Removes a payment method from the system.
        /// </summary>
        (bool success, string message) DeletePaymentMethod(int paymentId, int userId);

        /// <summary>
        /// Turns a payment method's active status on or off.
        /// </summary>
        (bool success, string message) TogglePaymentMethodStatus(int paymentId, bool isActive, int userId);

        Task<ResolvedPaymentGatewayConfig?> GetConfigAsync(int companyId, int? paymentTypeId = null);
    }
}
