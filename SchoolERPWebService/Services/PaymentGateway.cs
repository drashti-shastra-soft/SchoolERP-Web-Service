// ============================================================================
// STEP 3: PaymentGateway.cs (Stripe) — full file, with refund support added.
// Additions vs. your original are marked with "// >>> NEW".
// ============================================================================
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;               // >>> NEW (GatewayRefundRequest / Result / RefundWebhookInfo)

namespace SchoolERPWebService.Services
{
    public class PaymentGateway : IPaymentGateway
    {
        private const string BaseUrl = "https://api.stripe.com/v1/";

        private readonly IConfiguration _configuration;
        private readonly HttpClient _http;
        private string _webhookSecret = "";
        public PaymentGateway(IConfiguration configuration, HttpClient http)
        {
            _configuration = configuration;
            _http = http;
            //_http.BaseAddress = new Uri(BaseUrl);
            //_http.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Bearer", configuration["PaymentGateways:Stripe:SecretKey"]);
        }

        

        public string Name => "Stripe";

        public Task ConfigureAsync(IReadOnlyDictionary<string, string> settings)
        {
            if (!settings.TryGetValue("STRIPE_API_SECRET_KEY", out var secretKey) || string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("STRIPE_API_SECRET_KEY is not configured for this company.");

            var baseUrl = settings.TryGetValue("BASEURL", out var b) && !string.IsNullOrWhiteSpace(b)
                ? b
                : BaseUrl;

            _http.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);

            // Per-company webhook secret if configured, else fall back to appsettings.
            _webhookSecret = settings.TryGetValue("STRIPE_WEBHOOK_SECRET", out var wh) && !string.IsNullOrWhiteSpace(wh)
                ? wh
                : _configuration["PaymentGateways:Stripe:WebhookSecret"] ?? "";

            return Task.CompletedTask;
        }

        public async Task<GatewaySession> CreateCheckoutAsync(GatewayCheckoutRequest r)
        {
            var unitAmount = (long)Math.Round(r.Amount * 100m, 0, MidpointRounding.AwayFromZero); // smallest unit

            var form = new List<KeyValuePair<string, string>>
            {
                new("mode", "payment"),
                new("success_url", r.SuccessUrl),          // contains {CHECKOUT_SESSION_ID}
                new("cancel_url", r.CancelUrl),
                new("client_reference_id", r.OnlinePaymentId.ToString()),
                new("metadata[OnlinePaymentId]", r.OnlinePaymentId.ToString()),
                new("payment_intent_data[metadata][OnlinePaymentId]", r.OnlinePaymentId.ToString()),
                new("line_items[0][quantity]", "1"),
                new("line_items[0][price_data][currency]", r.Currency.ToLower()),
                new("line_items[0][price_data][unit_amount]", unitAmount.ToString()),
                new("line_items[0][price_data][product_data][name]", r.Description)
            };
            if (!string.IsNullOrWhiteSpace(r.PayerEmail))
                form.Add(new("customer_email", r.PayerEmail));

            var resp = await _http.PostAsync("checkout/sessions", new FormUrlEncodedContent(form));
            var json = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Stripe create checkout session failed: {json}");

            using var doc = JsonDocument.Parse(json);
            var id = doc.RootElement.GetProperty("id").GetString()!;
            var url = doc.RootElement.GetProperty("url").GetString()!;

            var requestJson = JsonSerializer.Serialize(new { mode = "payment", r.Amount, r.Currency, r.OnlinePaymentId });
            return new GatewaySession(id, url, requestJson);
        }

        public async Task<GatewayPaymentInfo?> GetPaymentAsync(string orderId)
        {
            var resp = await _http.GetAsync($"checkout/sessions/{orderId}");
            var json = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode) return null;

            return ToInfo(JsonDocument.Parse(json).RootElement, json);
        }

        // >>> NEW ----------------------------------------------------------------
        // Refunds a captured PaymentIntent. r.PaymentId must be the Stripe
        // PaymentIntent id (that's what GatewayPaymentInfo.PaymentId holds).
        // Idempotency-Key means a retried call (e.g. after a timeout) won't
        // create a second refund for the same request.
        public async Task<GatewayRefundResult> RefundAsync(GatewayRefundRequest r)
        {
            var form = new List<KeyValuePair<string, string>>
            {
                new("payment_intent", r.PaymentId)
            };
            if (r.Amount.HasValue)
            {
                var amt = (long)Math.Round(r.Amount.Value * 100m, 0, MidpointRounding.AwayFromZero);
                form.Add(new("amount", amt.ToString()));
            }
            if (!string.IsNullOrWhiteSpace(r.Reason))
                form.Add(new("metadata[Reason]", r.Reason));

            using var req = new HttpRequestMessage(HttpMethod.Post, "refunds")
            {
                Content = new FormUrlEncodedContent(form)
            };
            req.Headers.Add("Idempotency-Key", $"refund-{r.PaymentId}-{r.Amount?.ToString("0.00") ?? "full"}");

            var resp = await _http.SendAsync(req);
            var json = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Stripe refund failed: {json}");

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            return new GatewayRefundResult(
                RefundId: root.GetProperty("id").GetString()!,
                Status: root.GetProperty("status").GetString()!,
                Amount: root.GetProperty("amount").GetInt64() / 100m,
                RawJson: json);
        }
        // <<< END NEW --------------------------------------------------------------

        public GatewayWebhookEvent ParseWebhook(string body, string signatureHeader)
        {
            VerifySignature(body, signatureHeader, _configuration["PaymentGateways:Stripe:WebhookSecret"] ?? "");

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var eventType = root.GetProperty("type").GetString();
            var sessionEl = root.GetProperty("data").GetProperty("object");

            // For "charge.refunded" this is actually a Charge object, not a Checkout
            // Session — ToInfo() below won't find session metadata on it, which is
            // exactly why the refund branch matches by payment_intent id instead.
            var info = ToInfo(sessionEl, body);

            return eventType switch
            {
                "checkout.session.completed" or "checkout.session.async_payment_succeeded"
                    => info != null && info.IsPaid
                        ? new GatewayWebhookEvent("Paid", info, null)
                        : new GatewayWebhookEvent("Ignored", info, null),          // async method still pending
                "checkout.session.async_payment_failed" => new GatewayWebhookEvent("Failed", info, "Async payment failed"),
                "checkout.session.expired" => new GatewayWebhookEvent("Expired", info, "Checkout session expired"),
                "charge.refunded" => ToRefundEvent(sessionEl, body),               // >>> NEW
                _ => new GatewayWebhookEvent("Ignored", info, null)
            };
        }

        // >>> NEW ----------------------------------------------------------------
        // Handles refunds issued outside this app (Stripe dashboard, support
        // agent, etc.) so FeeCollectionService can reconcile them.
        private static GatewayWebhookEvent ToRefundEvent(JsonElement charge, string rawJson)
        {
            var paymentIntentId = charge.TryGetProperty("payment_intent", out var pi)
                ? (pi.ValueKind == JsonValueKind.String ? pi.GetString() : pi.TryGetProperty("id", out var piId) ? piId.GetString() : null)
                : null;

            if (string.IsNullOrEmpty(paymentIntentId))
                return new GatewayWebhookEvent("Ignored", null, null);

            var amountRefunded = charge.TryGetProperty("amount_refunded", out var ar) ? ar.GetInt64() / 100m : 0m;
            var fullyRefunded = charge.TryGetProperty("refunded", out var rf) && rf.GetBoolean();

            // Most recent refund id on the charge — used as the idempotency key on the DB side.
            var refundId = charge.TryGetProperty("refunds", out var refunds) &&
                           refunds.TryGetProperty("data", out var refundData) && refundData.GetArrayLength() > 0
                ? refundData[0].GetProperty("id").GetString() ?? ""
                : "";

            return new GatewayWebhookEvent("Refunded", null, null,
                new GatewayRefundWebhookInfo(paymentIntentId, refundId, amountRefunded, fullyRefunded, rawJson));
        }
        // <<< END NEW --------------------------------------------------------------

        // Stripe-Signature header looks like: "t=1614556800,v1=5257a869e...,v0=..."
        private static void VerifySignature(string body, string signatureHeader, string webhookSecret)
        {
            var parts = signatureHeader.Split(',')
                .Select(p => p.Split('=', 2))
                .Where(p => p.Length == 2)
                .ToDictionary(p => p[0], p => p[1]);

            if (!parts.TryGetValue("t", out var timestamp) || !parts.TryGetValue("v1", out var expectedSig))
                throw new InvalidOperationException("Malformed Stripe-Signature header.");

            var signedPayload = $"{timestamp}.{body}";
            var computed = Convert.ToHexString(
                new HMACSHA256(Encoding.UTF8.GetBytes(webhookSecret)).ComputeHash(Encoding.UTF8.GetBytes(signedPayload))).ToLower();

            if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(computed), Encoding.UTF8.GetBytes(expectedSig)))
                throw new InvalidOperationException("Invalid Stripe webhook signature.");

            // Optional: also reject if timestamp is older than a few minutes, to guard against replay.
        }

        private static GatewayPaymentInfo? ToInfo(JsonElement session, string rawJson)
        {
            if (!session.TryGetProperty("metadata", out var metadata) ||
                !metadata.TryGetProperty("OnlinePaymentId", out var idEl) ||
                !int.TryParse(idEl.GetString(), out var id))
                return null;

            var paymentStatus = session.TryGetProperty("payment_status", out var ps) ? ps.GetString() : null;
            var paymentIntentId = session.TryGetProperty("payment_intent", out var pi)
                ? (pi.ValueKind == JsonValueKind.String ? pi.GetString() : pi.TryGetProperty("id", out var piId) ? piId.GetString() : null)
                : null;
            var amountTotal = session.TryGetProperty("amount_total", out var at) ? at.GetInt64() : 0L;
            var method = session.TryGetProperty("payment_method_types", out var pmt) && pmt.GetArrayLength() > 0
                ? pmt[0].GetString()
                : null;

            return new GatewayPaymentInfo(
                id,
                IsPaid: paymentStatus == "paid",
                PaymentId: paymentIntentId,
                Amount: amountTotal / 100m,
                Method: method,
                RawJson: rawJson);
        }
    }
}