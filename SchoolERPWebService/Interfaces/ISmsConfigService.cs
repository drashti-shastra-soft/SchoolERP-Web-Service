using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing SMS settings, such as the gateway used to send text messages.
    /// </summary>
    public interface ISmsConfigService
    {

        /// <summary>
        /// Returns the company's currently selected SMS gateway + status
        /// (SmsGateway = e.g. "Twilio SMS Gateway", SmsStatus = "Enable"/"Disable").
        /// </summary>
        MstSmsConfigViewModel? GetSmsConfig(int companyId);

        /// <summary>
        /// Updates or saves new SMS settings, like the API URL and security key.
        /// </summary>
        (bool success, string message) UpsertSmsConfig(MstSmsConfigUpsertRequest request, int userId, int companyId);

        /// <summary>
        /// Returns every configured field VALUE for this company + gateway type,
        /// keyed by FieldKey (e.g. "TWILIO_ACCOUNT_SID" -> "AC8f3c...").
        /// This is what makes the sender fully dynamic: no field name is
        /// hardcoded anywhere in code, it all comes from these two tables.
        /// </summary>
        Dictionary<string, string> GetSmsConfigFields(int companyId, int smsTypeId);
    }
}
