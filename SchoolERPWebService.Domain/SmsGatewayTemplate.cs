using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    //public class SmsGatewayTemplate
    //{
    //    public string SmsType { get; set; } = "";
    //    public string HttpMethod { get; set; } = "POST";
    //    public string RequestUrl { get; set; } = "";
    //    public string ContentType { get; set; } = "application/json";
    //    public Dictionary<string, string> Headers { get; set; } = new();
    //    public Dictionary<string, string> BodyParams { get; set; } = new();
    //    public List<string> BodyArrayParams { get; set; } = new();
    //    public string? SuccessPath { get; set; }
    //    public string? SuccessValue { get; set; }
    //    public string? MessagePath { get; set; }
    //}

    public class SmsGatewayTemplateOptions
    {
        public List<SmsGatewayTemplate> SmsGatewayTemplates { get; set; } = new();
    }
    public class SmsSendResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ProviderMessageId { get; set; }
        public string RawResponse { get; set; }
    }

    /// <summary>
    /// Describes how to call one SMS gateway provider's HTTP API.
    /// Every value can reference {{FIELD_KEY}} placeholders that get resolved
    /// at send-time from that company's configured field values.
    /// </summary>
    public class SmsGatewayTemplate
    {
        public string SmsType { get; set; } = "";
        public string HttpMethod { get; set; } = "POST";
        public string RequestUrl { get; set; } = "";
        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        public Dictionary<string, string> Headers { get; set; } = new();
        public Dictionary<string, string> BodyParams { get; set; } = new();
        public List<string> BodyArrayParams { get; set; } = new();

        public string? SuccessPath { get; set; }
        public string? SuccessValue { get; set; }
        public string? MessagePath { get; set; }

        /// <summary>
        /// Optional. Only set for gateways that authenticate via HTTP Basic Auth
        /// (e.g. Twilio). Names the two field keys to combine into BASIC_AUTH_TOKEN.
        /// Gateways that use a single API key header (Clickatell, MSG91, etc.)
        /// simply leave this null.
        /// </summary>
        public BasicAuthConfig? BasicAuth { get; set; }
    }

    public class BasicAuthConfig
    {
        /// <summary>FieldKey that holds the "username" half (e.g. TWILIO_ACCOUNT_SID).</summary>
        public string UsernameField { get; set; } = "";

        /// <summary>FieldKey that holds the "password" half (e.g. AUTHENTICATION_TOKEN).</summary>
        public string PasswordField { get; set; } = "";
    }
}
