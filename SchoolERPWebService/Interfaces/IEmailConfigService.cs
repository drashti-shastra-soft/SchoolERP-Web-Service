using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing email settings, such as the server used to send emails.
    /// </summary>
    public interface IEmailConfigService
    { 
        /// <summary>
      /// Gets the current email settings from the system.
      /// </summary>
        MstEmailConfigViewModel? GetEmailConfig(int companyId);

        /// <summary>
        /// Updates or saves new email settings, like the SMTP server and login details.
        /// </summary>
        Task<(bool success, string message)> UpsertEmailConfig(MstEmailConfigUpsertRequest request, int userId, int companyId);

    }
}
