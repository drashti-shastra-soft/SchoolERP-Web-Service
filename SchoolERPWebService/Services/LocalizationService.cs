using Microsoft.Extensions.Caching.Memory;
using SchoolERPWebService.Interfaces;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of translating the application's text. It reads translations from files and keeps them ready so the application can quickly show the right words in the user's chosen language.
    /// </summary>
    public class LocalizationService: ILocalizationService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CacheKeyPrefix = "Culture_";

        public LocalizationService(IWebHostEnvironment webHostEnvironment, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Takes a 'key' (like 'LoginButton') and returns the actual word in the user's language (like 'Sign In'). If no translation is found, it shows the key itself.
        /// </summary>
        public string GetString(string key)
        {
            var translations = GetAllStrings();
            if (translations != null && translations.TryGetValue(key, out string value))
            {
                return value;
            }
            return key; // Return the key itself if no translation is found
        }

        /// <summary>
        /// Loads all translations for the current language into the application's memory for fast access.
        /// </summary>
        public Dictionary<string, string> GetAllStrings()
        {
            string culture = GetCurrentLanguage();
            string cacheKey = CacheKeyPrefix + culture;

            if (!_cache.TryGetValue(cacheKey, out Dictionary<string, string> translations))
            {
                // Correct path as per user request: wwwroot/language/[Language]/app_files/system_lang.json
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "language", culture, "app_files", "system_lang.json");
                if (System.IO.File.Exists(filePath))
                {
                    string jsonContent = System.IO.File.ReadAllText(filePath);
                    translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                    _cache.Set(cacheKey, translations, TimeSpan.FromHours(1));
                }
                else
                {
                    // Fallback to English if file not found
                    if (culture.ToLower() != "english")
                    {
                        return GetTranslationsForLanguage("English");
                    }
                    translations = new Dictionary<string, string>();
                }
            }

            return translations;
        }

        /// <summary>
        /// Fetches all translated text for a specific language.
        /// </summary>
        public Dictionary<string, string> GetTranslations(string language)
        {
            return GetTranslationsForLanguage(language);
        }

        /// <summary>
        /// Writes translation changes to a file so they are remembered for next time.
        /// </summary>
        public void SaveTranslations(string language, Dictionary<string, string> translations)
        {
            // Update to use the new path if saving is needed
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "language", language, "app_files", "system_lang.json");
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            string jsonContent = JsonSerializer.Serialize(translations, options);
            System.IO.File.WriteAllText(filePath, jsonContent);

            // Update cache
            string cacheKey = CacheKeyPrefix + language;
            _cache.Set(cacheKey, translations, TimeSpan.FromHours(1));
        }

        /// <summary>
        /// A private tool that loads translations from a specific language file into memory.
        /// </summary>
        private Dictionary<string, string> GetTranslationsForLanguage(string language)
        {
            string cacheKey = CacheKeyPrefix + language;
            if (!_cache.TryGetValue(cacheKey, out Dictionary<string, string> translations))
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "language", language, "app_files", "system_lang.json");
                if (System.IO.File.Exists(filePath))
                {
                    string jsonContent = System.IO.File.ReadAllText(filePath);
                    translations = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                    _cache.Set(cacheKey, translations, TimeSpan.FromHours(1));
                }
                else
                {
                    translations = new Dictionary<string, string>();
                }
            }
            return translations;
        }

        /// <summary>
        /// Remembers the user's language choice by saving it in a small file (cookie) on their browser.
        /// </summary>
        public void SetLanguage(string languageCode)
        {
            // Step 1: Create a 'Cookie' which is like a small sticky note for the user's browser.
            // Step 2: Set the note to last for 1 year so the system remembers the language choice every time they visit.
            var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };

            // Step 3: Stick the 'Language' choice onto the browser's cookie jar.
            _httpContextAccessor.HttpContext.Response.Cookies.Append("Language", languageCode, options);
        }

        /// <summary>
        /// Checks the user's browser settings to see which language they prefer.
        /// </summary>
        public string GetCurrentLanguage()
        {
            var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["Language"];
            return string.IsNullOrEmpty(cookieValue) ? "English" : cookieValue;
        }

        /// <summary>
        /// Looks through the system's folders to see which languages have been set up and are ready to be used.
        /// </summary>
        public List<string> GetAvailableLanguages()
        {
            string path = Path.Combine(_webHostEnvironment.WebRootPath, "language");
            if (Directory.Exists(path))
            {
                // List all subdirectories as available languages
                return Directory.GetDirectories(path)
                                .Select(d => Path.GetFileName(d))
                                .Where(name => !name.StartsWith(".")) // Exclude hidden folders if any
                                .ToList();
            }
            return new List<string> { "English" };
        }
    }
}
