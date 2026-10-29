using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace SchoolERPWebService.Utilities
{
    public class LocalizationMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cookieValue = context.Request.Cookies["Language"];
            if (!string.IsNullOrEmpty(cookieValue))
            {
                try
                {
                    // Map common language names to culture codes if needed
                    // For now, assume common names work or we use a mapping
                    string cultureCode = GetCultureCode(cookieValue);
                    var culture = new CultureInfo(cultureCode);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
                catch
                {
                    // Fallback to default
                }
            }

            await _next(context);
        }

        private string GetCultureCode(string language)
        {
            // Basic mapping for major languages, others will use "en-US" fallback or try to match
            return language.ToLower() switch
            {
                "english" => "en-US",
                "hindi" => "hi-IN",
                "arabic" => "ar-SA",
                "french" => "fr-FR",
                "german" => "de-DE",
                "spanish" => "es-ES",
                "urdu" => "ur-PK",
                _ => "en-US" // Default to English
            };
        }
    }
}
