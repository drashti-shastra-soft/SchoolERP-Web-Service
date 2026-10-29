namespace SchoolERPWebService.Interfaces
{
    public record GatewayCheckoutRequest(int OnlinePaymentId, decimal Amount, string Currency, string Description,
                                         string? PayerEmail, string SuccessUrl, string CancelUrl);

    public record GatewaySession(string OrderId, string RedirectUrl, string RequestJson);

    public record GatewayPaymentInfo(int OnlinePaymentId, bool IsPaid, string? PaymentId, decimal Amount,
                                     string? Method, string RawJson);


    // ---- Gateway-level contracts (used by IPaymentGateway) --------------------

    /// Request to refund a completed gateway payment. Amount = null means "refund in full".
    public record GatewayRefundRequest(string PaymentId, decimal? Amount, string? Reason);

    /// Result of a gateway refund call.
    public record GatewayRefundResult(string RefundId, string Status, decimal Amount, string RawJson);

    /// Info extracted from a "refund happened" webhook (e.g. refunded via the Stripe dashboard,
    /// not through this app).
    public record GatewayRefundWebhookInfo(
        string GatewayPaymentId,
        string RefundId,
        decimal AmountRefunded,
        bool FullyRefunded,
        string RawJson);

    // ---- Extend the existing webhook event record ------------------------------
    // Replace your current GatewayWebhookEvent declaration with this one
    // (adds an optional RefundInfo slot; existing callers using the
    // 3-arg constructor keep compiling unchanged).
    public record GatewayWebhookEvent(
        string Kind,
        GatewayPaymentInfo? Info,
        string? Reason,
        GatewayRefundWebhookInfo? RefundInfo = null);

    /// <summary>Kind: Paid | Failed | Expired | Ignored</summary>
    //public record GatewayWebhookEvent(string Kind, GatewayPaymentInfo? Info, string? Reason);

    /// <summary>One implementation per gateway. Register each one; the service picks by Name.</summary>
    public interface IPaymentGateway
    {
        string Name { get; }
        Task<GatewaySession> CreateCheckoutAsync(GatewayCheckoutRequest request);
        Task<GatewayPaymentInfo?> GetPaymentAsync(string orderId);
        /// <summary>Verifies the signature (throws if invalid) and maps the gateway event.</summary>
        GatewayWebhookEvent ParseWebhook(string body, string signatureHeader);
        Task<GatewayRefundResult> RefundAsync(GatewayRefundRequest request);
        Task ConfigureAsync(IReadOnlyDictionary<string, string> settings);
    }
}