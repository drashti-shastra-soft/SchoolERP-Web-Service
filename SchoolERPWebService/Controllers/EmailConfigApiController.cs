using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for configuring the email server settings through the API.
    /// </summary>
    public class EmailConfigApiController : ControllerBase
    {
        private readonly IEmailConfigService _emailConfigService;
        private readonly ICompanyService _companySvc;

        public EmailConfigApiController(IEmailConfigService emailConfigService, ICompanyService companySvc)
        {
            _emailConfigService = emailConfigService;
            _companySvc = companySvc;
        }


        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;

        /// <summary>
        /// Gets the current email server settings (like the server address and port) from the system.
        /// </summary>
        [HttpGet("Get")]
        public IActionResult Get(int companyId)
        {
            if (companyId == 0 || companyId == null)
            {
                companyId = CompanyId;
            }
            var data = _emailConfigService.GetEmailConfig(companyId);
            return Ok(ApiResponse<MstEmailConfigViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Saves or updates the email server settings with the information you provided.
        /// </summary>
        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstEmailConfigUpsertRequest request)
        {
            // Abort early if the port constraints or payload strings fail model bounds
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if(request.CompanyId==0 || request.CompanyId == null) 
            {
                request.CompanyId = CompanyId;
            }
            int userId = GetCurrentUserId();
            var (success, message) =await _emailConfigService.UpsertEmailConfig(request, userId, request.CompanyId);
            
            if (success) return Ok(ApiResponse<dynamic>.SuccessResponse(new { success }, message));
            return BadRequest(ApiResponse<dynamic>.ErrorResponse(message));
        }

        /// <summary>
        /// Reads the JWT or encrypted cookie ClaimsPrincipal to identify the administrator acting context.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            // Defaulting to user '1' if token claims are bypassed or missing
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
    }
}
