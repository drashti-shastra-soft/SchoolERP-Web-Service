using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Services
{
    public class SmsService : ISmsService
    {
        private readonly ISmsConfigService _configRepo;
        private readonly ISmsTemplateProvider _templateProvider;
        private readonly DynamicSmsGatewaySender _sender;

        public SmsService(ISmsConfigService configRepo, ISmsTemplateProvider templateProvider, DynamicSmsGatewaySender sender)
        {
            _configRepo = configRepo;
            _templateProvider = templateProvider;
            _sender = sender;
        }

        public async Task<SmsSendResult> SendAsync(int companyId, string toPhone, string message)
        {
            var getfiled = _configRepo.GetSmsConfig(companyId);

            if (getfiled == null)
                return new SmsSendResult { Success = false, Message = "No SMS gateway is configured for this company." };

            if (string.Equals(getfiled.SmsStatus, "Disable", StringComparison.OrdinalIgnoreCase))
                return new SmsSendResult { Success = false, Message = "SMS sending is currently disabled for this company." };

            // Structural shape of the call (HttpMethod, BodyParams, SuccessPath,
            // BasicAuth field mapping, etc.) - same for every company using this
            // gateway type, defined once in smsGatewayTemplates.json.
            var staticTemplate = _templateProvider.GetBySmsTypeName(getfiled.SmsGateway);
            if (staticTemplate == null)
                return new SmsSendResult { Success = false, Message = $"'{getfiled.SmsGateway}' isn't wired up for sending yet." };

            // Per-company values (credentials, sender number, request URL, etc.)
            // pulled dynamically from the admin-configured fields in the DB —
            // no field name is hardcoded anywhere.
            var fields = _configRepo.GetSmsConfigFields(companyId, getfiled.SMSTypeId);

            if (fields.Count == 0)
                return new SmsSendResult { Success = false, Message = $"'{getfiled.SmsGateway}' has no configured fields for this company." };

            // Clone the template per request instead of mutating the shared
            // instance loaded from JSON/IOptions (which could be reused across
            // concurrent requests for other companies).
            var template = CloneWithOverrides(staticTemplate, fields);

            return await _sender.SendAsync(template, fields, toPhone, message);
        }

        /// <summary>
        /// Applies any per-company overrides on top of the shared static template.
        /// Currently: if the company's fields include a REQUEST_URL, it overrides
        /// the template's default RequestUrl (still resolved with {{...}} placeholders
        /// by the sender afterwards).
        /// </summary>
        private static SmsGatewayTemplate CloneWithOverrides(SmsGatewayTemplate src, IDictionary<string, string> fields)
        {
            var clone = new SmsGatewayTemplate
            {
                SmsType = src.SmsType,
                HttpMethod = src.HttpMethod,
                RequestUrl = fields.TryGetValue("REQUEST_URL", out var url) && !string.IsNullOrWhiteSpace(url)
                    ? url
                    : src.RequestUrl,
                ContentType = src.ContentType,
                Headers = new Dictionary<string, string>(src.Headers, StringComparer.OrdinalIgnoreCase),
                BodyParams = new Dictionary<string, string>(src.BodyParams, StringComparer.OrdinalIgnoreCase),
                BodyArrayParams = new List<string>(src.BodyArrayParams),
                SuccessPath = src.SuccessPath,
                SuccessValue = src.SuccessValue,
                MessagePath = src.MessagePath,
                BasicAuth = src.BasicAuth
            };
            return clone;
        }
    }
}