using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;
using SchoolERPWebService.Services;
using System.ComponentModel.Design;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing academic sessions (school years) through the API.
    /// </summary>
    public class SessionApiController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;
        private readonly ICompanyService _companySvc;

        private const string MenuPath = "/Settings";

        public SessionApiController(ISessionService sessionService, IUserMenuPermissionService menuPerm, ICompanyService companySvc)
        {
            _sessionService = sessionService;
            _companySvc = companySvc;
            _menuPerm = menuPerm;
        }
        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
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
        /// <summary>
        /// Gets the full list of all academic sessions defined in the system.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false, int? companyId = null)
        {
            var data = _sessionService.GetAllSessions(includeDeleted, companyId);
            return Ok(ApiResponse<List<MstSessionViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Gets the details of one specific academic session using its unique ID number.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _sessionService.GetSessionByID(id);
            if (data == null) return NotFound(ApiResponse<MstSessionViewModel>.ErrorResponse("Time phase interval not found"));
            return Ok(ApiResponse<MstSessionViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Saves a new academic session or updates an existing one with the dates and name you provided.
        /// </summary>
        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstSessionUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();
                var isCreate = request.SessionId <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add sessions." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit sessions." });
                if (request.CompanyID == 0 || request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }

                var (success, message) = _sessionService.UpsertSession(request, userId, request.CompanyID);
                return Ok(new { success, message });
            }

            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Permanently removes an academic session from the system's records.
        /// </summary>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete sessions." });

            int userId = GetCurrentUserId();
            var (success, message) = _sessionService.DeleteSession(id, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Turns an academic session's active status on or off.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change status." });

            int userId = GetCurrentUserId();
            var (success, message) = _sessionService.ToggleSessionStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Sets the chosen academic session as the currently 'active' one for the user in the database.
        /// </summary>
        [HttpPost("SetCurrent")]
        public IActionResult SetCurrent([FromBody] SetCurrentSessionRequest request)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _sessionService.UpdateUserCurrentSession(userId, request.SessionId);
            if (!success) return BadRequest(ApiResponse<dynamic>.ErrorResponse(message));
            return Ok(ApiResponse<dynamic>.SuccessResponse(null, message));
        }

        /// <summary>
        /// Gets the ID of the academic session that the user is currently working in.
        /// </summary>
        [HttpGet("GetUserCurrentSession")]
        public IActionResult GetUserCurrentSession(int? companyId = null)
        {
            int userId = GetCurrentUserId();
            var sessionId = _sessionService.GetUserCurrentSession(userId, companyId);
            return Ok(ApiResponse<int?>.SuccessResponse(sessionId));
        }

        /// <summary>
        /// Resolves cryptographically bound credentials parsing the requesting context.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        [HttpPost("GetAllSessionWithPage")]
        public async Task<IActionResult> GetAllSessionWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (IsSuperAdmin())
                {
                    var result = await _sessionService.GetAllSessionWithPage(request);
                    return Ok(ApiResponse<PagedResult<MstSessionViewModel>>.SuccessResponse(result));
                }
                request.UserId = GetCurrentUserId();
                var data = await _sessionService.GetAllSessionWithPage(request);
                return Ok(ApiResponse<PagedResult<MstSessionViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("CopySession")]
        public async Task<IActionResult> CopySession([FromBody] CopyForCompanyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = GetCurrentUserId();
                }
                if (string.IsNullOrWhiteSpace(request.IPAddress))
                    request.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                // ── Execute copy ──────────────────────────────────────────
                var result = await _sessionService.CopySession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }
    }
}
