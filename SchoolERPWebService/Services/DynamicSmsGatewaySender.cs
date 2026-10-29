using SchoolERPWebService.Domain;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// Sends an SMS through whichever gateway a company has configured, without
    /// knowing anything specific about that gateway's field names in advance.
    /// Everything (URL, headers, body, auth) is built from {{FIELD_KEY}} placeholders
    /// that get resolved from the "fields" dictionary passed in.
    /// </summary>
    public class DynamicSmsGatewaySender
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DynamicSmsGatewaySender(IHttpClientFactory f) => _httpClientFactory = f;

        public async Task<SmsSendResult> SendAsync(SmsGatewayTemplate t, IDictionary<string, string> fields, string toPhone, string message)
        {
            var ctx = new Dictionary<string, string>(fields, StringComparer.OrdinalIgnoreCase)
            {
                ["ToPhone"] = toPhone,
                ["Message"] = message
            };

            // Dynamic Basic Auth: the template says WHICH two fields to combine.
            // No gateway-specific field names are hardcoded here.
            if (t.BasicAuth != null
                && ctx.TryGetValue(t.BasicAuth.UsernameField, out var authUser)
                && ctx.TryGetValue(t.BasicAuth.PasswordField, out var authPass))
            {
                ctx["BASIC_AUTH_TOKEN"] = Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($"{authUser}:{authPass}"));
            }

            var url = Sub(t.RequestUrl, ctx, true);
            var client = _httpClientFactory.CreateClient("SmsGateway");
            using var req = new HttpRequestMessage(new HttpMethod(t.HttpMethod), url);

            foreach (var h in t.Headers)
                req.Headers.TryAddWithoutValidation(h.Key, Sub(h.Value, ctx, false));

            if (t.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) && t.BodyParams.Count > 0)
            {
                var body = t.ContentType.Contains("form")
                    ? string.Join("&", t.BodyParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(Sub(p.Value, ctx, false))}"))
                    : JsonSerializer.Serialize(t.BodyParams.ToDictionary(
                        p => p.Key,
                        p => t.BodyArrayParams.Contains(p.Key) ? (object)new[] { Sub(p.Value, ctx, false) } : Sub(p.Value, ctx, false)));

                req.Content = new StringContent(body, Encoding.UTF8, t.ContentType);
            }

            string responseBody; bool ok;
            try
            {
                var resp = await client.SendAsync(req);
                responseBody = await resp.Content.ReadAsStringAsync();
                ok = resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                return new SmsSendResult { Success = false, Message = $"Request failed: {ex.Message}" };
            }

            if (string.IsNullOrWhiteSpace(t.SuccessPath))
                return new SmsSendResult { Success = ok, RawResponse = responseBody };

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var actual = SelectPath(doc.RootElement, t.SuccessPath);
                var msg = t.MessagePath == null ? null : SelectPath(doc.RootElement, t.MessagePath);
                return new SmsSendResult
                {
                    Success = string.Equals(actual, t.SuccessValue, StringComparison.OrdinalIgnoreCase),
                    Message = msg,
                    RawResponse = responseBody
                };
            }
            catch
            {
                return new SmsSendResult { Success = ok, RawResponse = responseBody };
            }
        }

        private static string Sub(string tpl, IDictionary<string, string> ctx, bool enc) =>
            Regex.Replace(tpl, @"\{\{(\w+)\}\}", m =>
            {
                var v = ctx.TryGetValue(m.Groups[1].Value, out var val) ? val : "";
                return enc ? Uri.EscapeDataString(v) : v;
            });

        private static string? SelectPath(JsonElement root, string path)
        {
            var cur = root;
            foreach (var seg in path.Split('.'))
            {
                var m = Regex.Match(seg, @"^(\w+)\[(\d+)\]$");
                cur = m.Success ? cur.GetProperty(m.Groups[1].Value)[int.Parse(m.Groups[2].Value)]
                                 : cur.TryGetProperty(seg, out var n) ? n : default;
            }
            return cur.ValueKind == JsonValueKind.String ? cur.GetString() : cur.ToString();
        }
    }
}