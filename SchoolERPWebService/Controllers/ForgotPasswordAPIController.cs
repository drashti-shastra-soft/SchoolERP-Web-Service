using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordAPIController : Controller
    {
        private readonly IForgotPasswordService _service;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public ForgotPasswordAPIController(IForgotPasswordService service, ICompanyService companySvc, ISessionService sessionSvc, IHttpContextAccessor httpContext)
        {
            _service = service;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _httpContext = httpContext;
        }


        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.Email))
                    return Ok(new { success = false, message = "Email is required." });

                var baseUrl = $"{_httpContext.HttpContext.Request.Scheme}://{_httpContext.HttpContext.Request.Host}";

                int companyId = 0;
                int sessionId = 0;

                var (success, message) = await _service.GenerateTokenAsync(
                    request.Email, companyId, sessionId, baseUrl);

                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("ValidateToken")]
        [AllowAnonymous]
        public IActionResult ValidateToken(string guid)
        {
            try
            {
                if (string.IsNullOrEmpty(guid))
                    return Ok(new { success = false, message = "Invalid token." });

                var result = _service.ValidateToken(guid);
                return Ok(new
                {
                    success = result?.Result == 1,
                    data = result,
                    message = result?.Message
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request?.NewPassword))
                    return Ok(new { success = false, message = "Password is required." });

                if (request.NewPassword != request.ConfirmPassword)
                    return Ok(new { success = false, message = "Password do not match." });

                if (string.IsNullOrEmpty(request.Guid))
                    return Ok(new { success = false, message = "Invalid reset token." });

                var (success, message) = _service.ResetPassword(request.Guid, request.NewPassword);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
