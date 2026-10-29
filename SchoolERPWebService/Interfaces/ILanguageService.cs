using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing the different languages available in the system.
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Gets a list of all languages supported by the system.
        /// </summary>
        List<MstLanguageViewModel> GetAllLanguages(bool includeDeleted = false);

        /// <summary>
        /// Finds the details of a specific language using its unique ID.
        /// </summary>
        MstLanguageViewModel? GetLanguageByID(int languageId);

        /// <summary>
        /// Adds a new language or updates an existing one, including settings like whether it reads from right to left (RTL).
        /// </summary>
        (bool success, string message) UpsertLanguage(MstLanguageUpsertRequest request, int userId);

        /// <summary>
        /// Removes a language from the system.
        /// </summary>
        (bool success, string message) DeleteLanguage(List<int> languageId, int userId);

        /// <summary>
        /// Turns a language's active status on or off.
        /// </summary>
        (bool success, string message) ToggleLanguageStatus(StatusUpdateRequest request);

        Task<PagedResult<MstLanguageViewModel>> GetAllLanguageWithPage(SearchRequest req);
    }
}
