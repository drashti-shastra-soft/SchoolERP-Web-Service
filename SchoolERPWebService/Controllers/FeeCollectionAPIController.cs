// ============================================================================
// STEP 5: FeeCollectionAPIController.cs (server-side API) — full file, with
// a Refund endpoint added. Additions vs. your original are marked "// >>> NEW".
// ============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeCollectionAPIController : ControllerBase
    {
        private readonly IFeeCollectionService _feeCollectionService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        public FeeCollectionAPIController(IFeeCollectionService feeCollectionService
            , ICompanyService companyService, ISessionService sessionService)
        {
            _feeCollectionService = feeCollectionService;
            _companyService = companyService;
            _sessionService = sessionService;
        }
        private bool IsSuperAdmin()
        {
            if (User?.Identity?.IsAuthenticated != true) return false;

            return User.Claims.Any(c =>
                (string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "Role", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserTypeName", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase)) &&
                IsSuperAdminValue(c.Value));
        }
        private static bool IsSuperAdminValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(normalized, "superadmin", StringComparison.OrdinalIgnoreCase);
        }
        private int GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
            }
            catch { return 0; }
        }

        [HttpPost("GetStudentFeeList")]
        public async Task<IActionResult> GetStudentFeeList([FromBody] StudentFeeListRequest request)
        {
            try
            {
                if (request == null)
                    return Ok(new StudentFeeTemplateListResponse { Result = 0, Message = "INVALID REQUEST." });

                return Ok(await _feeCollectionService.GetStudentFeeListAsync(request));
            }
            catch (Exception ex)
            {
                return Ok(new StudentFeeTemplateListResponse
                {
                    Result = 0,
                    Message = "UNABLE TO FETCH STUDENT FEE LIST.",
                    TechnicalMessage = ex.Message
                });
            }
        }

        [HttpPost("CollectOffline")]
        public async Task<IActionResult> CollectOffline([FromBody] FeeCollectRequest request)
        {
            try
            {
                if (request == null)
                    return Ok(new FeeCollectResponse { Result = 0, Message = "INVALID REQUEST." });

                request.UserId = GetCurrentUserId();
                return Ok(await _feeCollectionService.CollectOfflineAsync(request));
            }
            catch (Exception ex)
            {
                return Ok(new FeeCollectResponse { Result = 0, Message = "Unable to collect fees.", TechnicalMessage = ex.Message });
            }
        }

        [HttpPost("StartOnline")]
        public async Task<IActionResult> StartOnline([FromBody] OnlineStartRequest request)
        {
            try
            {
                if (request == null)
                    return Ok(new OnlineStartResponse { Result = 0, Message = "INVALID REQUEST." });

                request.UserId = GetCurrentUserId();
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();   // NOTE: this is the client server's IP
                return Ok(await _feeCollectionService.StartOnlineAsync(request));
            }
            catch (Exception ex)
            {
                return Ok(new OnlineStartResponse { Result = 0, Message = "Unable to start online payment.", TechnicalMessage = ex.Message });
            }
        }

        [HttpPost("FinalizeOnline")]
        public async Task<IActionResult> FinalizeOnline([FromBody] OnlineFinalizeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.OrderId))
                    return Ok(new FeeCollectResponse { Result = 0, Message = "INVALID REQUEST." });

                return Ok(await _feeCollectionService.FinalizeOnlineAsync(request));
            }
            catch (Exception ex)
            {
                return Ok(new FeeCollectResponse { Result = 0, Message = "Unable to confirm payment.", TechnicalMessage = ex.Message });
            }
        }

        [HttpPost("CancelOnline")]
        public async Task<IActionResult> CancelOnline([FromBody] OnlineCancelRequest request)
        {
            try
            {
                if (request == null)
                    return Ok(new FeeCollectResponse { Result = 0, Message = "INVALID REQUEST." });

                request.UserId = GetCurrentUserId();
                return Ok(await _feeCollectionService.CancelOnlineAsync(request));
            }
            catch (Exception ex)
            {
                return Ok(new FeeCollectResponse { Result = 0, Message = "Unable to cancel.", TechnicalMessage = ex.Message });
            }
        }

        // >>> NEW ----------------------------------------------------------------
        // Restrict at this layer too (not just the client-facing controller) since
        // this API could in principle be called directly. Adjust roles to match
        // your actual role names.
        [HttpPost("Refund")]
       // [Authorize(Roles = "Admin,Accountant")]
        public async Task<IActionResult> Refund([FromBody] RefundRequest request)
        {
            try
            {
                if (request == null || request.CollectionId <= 0)
                    return Ok(new RefundResponse { Result = 0, Message = "INVALID REQUEST." });

                request.UserId = GetCurrentUserId();
                return Ok(await _feeCollectionService.RefundAsync(request));
            }
            catch (Exception ex)
            {
                return Ok(new RefundResponse { Result = 0, Message = "Unable to process refund.", TechnicalMessage = ex.Message });
            }
        }
        // <<< END NEW --------------------------------------------------------------

        // Stripe calls this directly -> NO login token. Authenticity = Stripe signature check inside the gateway.
        // Public URL to register in Stripe:  https://YOUR-API-DOMAIN/api/FeeCollectionAPI/Webhook/Stripe
        [AllowAnonymous]
        [HttpPost("Webhook/{gateway}")]
        public async Task<IActionResult> Webhook(string gateway)
        {
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();
            var status = await _feeCollectionService.HandleWebhookAsync(gateway, body, signature);
            return StatusCode(status);
        }

        [HttpPost("GetAllStudentFeeWithPage")]
        public async Task<IActionResult> GetAllStudentFeeWithPage([FromBody] SearchRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;

                if (request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (companyId == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<StudentFeeGetAllResponse>>.SuccessResponse(new List<StudentFeeGetAllResponse>()));
                if (IsSuperAdmin())
                {
                    var datas = await _feeCollectionService.GetAllStudentFeeWithPage(request);
                    return Ok(ApiResponse<PagedResult<StudentFeeGetAllResponse>>.SuccessResponse(datas));
                }
                var data = await _feeCollectionService.GetAllStudentFeeWithPage(request);
                return Ok(ApiResponse<PagedResult<StudentFeeGetAllResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}