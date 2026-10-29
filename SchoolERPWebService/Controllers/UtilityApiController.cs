using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Threading.Tasks;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides various helpful technical endpoints, such as changing the system language or getting dashboard summaries.
    /// </summary>
    public class UtilityApiController : ControllerBase
    {
        private readonly ILocalizationService _localizationService;

        public UtilityApiController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        /// <summary>
        /// Changes the language used in the system to your chosen one.
        /// </summary>
        [HttpPost("set-language")]
        public IActionResult SetLanguage([FromQuery] string language)
        {
            _localizationService.SetLanguage(language);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Language set successfully"));
        }

        /// <summary>
        /// Gets a quick summary of important numbers to show on the main dashboard.
        /// </summary>
        [HttpGet("dashboard-summary")]
        public IActionResult GetDashboardSummary()
        {
            // Placeholder for dashboard stats
            return Ok(ApiResponse<object>.SuccessResponse(new { totalUsers = 10, uptime = "99.9%" }));
        }
    }
}
