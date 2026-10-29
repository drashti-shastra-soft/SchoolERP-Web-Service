namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for how text is translated and shown in different languages across the system.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Looks up a specific piece of text (like a button label) in the current language.
        /// </summary>
        string GetString(string key);

        /// <summary>
        /// Gets all the translated words and phrases for the current language.
        /// </summary>
        Dictionary<string, string> GetAllStrings();

        /// <summary>
        /// Gets all the translations for a specific language.
        /// </summary>
        Dictionary<string, string> GetTranslations(string language);

        /// <summary>
        /// Saves a new set of translations for a language to the system.
        /// </summary>
        void SaveTranslations(string language, Dictionary<string, string> translations);

        /// <summary>
        /// Changes the system's current language to a new one.
        /// </summary>
        void SetLanguage(string languageCode);

        /// <summary>
        /// Checks which language is currently being used by the user.
        /// </summary>
        string GetCurrentLanguage();

        /// <summary>
        /// Gets a list of all languages that are ready to be used.
        /// </summary>
        List<string> GetAvailableLanguages();
    }
}
