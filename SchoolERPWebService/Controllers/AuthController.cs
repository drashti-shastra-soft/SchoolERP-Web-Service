using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Utilities;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    /// <summary>
    /// API controller responsible for user authentication operations
    /// such as login and token generation.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthServices _authService;
        private readonly JwtHelper _jwtHelper;
        private readonly IUserManagementService _userMgmtService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public AuthController(IAuthServices service, JwtHelper jwtHelper, IUserManagementService userMgmtService, IUserService userService, IConfiguration configuration)
        {
            _authService = service;
            _jwtHelper = jwtHelper;
            _userMgmtService = userMgmtService;
            _userService = userService;
            _configuration = configuration;
        }
        private int? GetStaffID()
        {
            var staffClaim = User.FindFirst("StaffID")?.Value;
            return int.TryParse(staffClaim, out var staffId) ? staffId : null;
        }
        /// <summary>
        /// Authenticates a user using the provided username and password.
        /// If authentication is successful, user session information and
        /// authentication token are returned.
        /// </summary>
        /// <param name="request">
        /// Login request containing username and password.
        /// </param>
        /// <returns>
        /// Returns user session details on successful authentication;
        /// otherwise returns an error response.
        /// </returns>
        //[AllowAnonymous]
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    if (request == null ||
        //        string.IsNullOrWhiteSpace(request.Username) ||
        //        string.IsNullOrWhiteSpace(request.Password))
        //    {
        //        return BadRequest(
        //            ApiResponse<UserSessionModel>
        //            .ErrorResponse("Invalid request parameters"));
        //    }

        //    var result = await _authService.LoginAsync(
        //        request.Username,
        //        request.Password);

        //    if(result.Data.DashboardID !=null && result.Data.DashboardID != 0) 
        //    {
        //        var data = await _authService.GetDashboardByIdAsync(result.Data.DashboardID.Value);
        //        result.Data.DashboardURL = data.DashboardURL;
        //    }

        //    if (result.Success)
        //        return Ok(result);

        //    return Unauthorized(result);
        //}

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(
                    ApiResponse<UserSessionModel>
                    .ErrorResponse("Invalid request parameters"));
            }

            var result = await _authService.LoginAsync(
                request.Username,
                request.Password);

            if (result.Success) 
            {
                if (result.Data.DashboardID != null && result.Data.DashboardID != 0)
                {
                    var data = await _authService.GetDashboardByIdAsync(result.Data.DashboardID.Value);
                    result.Data.DashboardURL = data.DashboardURL;
                }

                if (result.Success)
                    return Ok(result);
            }
            else 
            {
                return Ok(result);
            }
               

            return Unauthorized(result);
        }

        //[AllowAnonymous]
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    if (request.IsUserLogin == 1) 
        //    {
        //        return await ProcessLogin(request, allowedRoles: new[] { "Student", "Parent" });
        //    }
        //    else
        //    {
        //        return await ProcessLogin(request, allowedRoles: new[] { "Admin", "Teacher", "Staff", "SuperAdmin" });
        //    }
                
        //}


        private async Task<IActionResult> ProcessLogin(LoginRequest request, string[] allowedRoles)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(
                    ApiResponse<UserSessionModel>.ErrorResponse("Invalid request parameters"));
            }

            var result = await _authService.LoginAsync(request.Username, request.Password);

            if (!result.Success)
                return Unauthorized(result);

            // ── Role gate — uses data ALREADY returned by LoginAsync, zero extra DB calls ──
            var userRole = !string.IsNullOrWhiteSpace(result.Data?.DefaultRoleName)
                             ? result.Data.DefaultRoleName
                             : result.Data?.UserTypeName;
            if (!allowedRoles.Any(r => string.Equals(r, userRole, StringComparison.OrdinalIgnoreCase)))
            {
                return Unauthorized(
                    ApiResponse<UserSessionModel>.ErrorResponse(
                        "This account type is not allowed to log in from this portal."));
            }

            if (result.Data.DashboardID != null && result.Data.DashboardID != 0)
            {
                var data = await _authService.GetDashboardByIdAsync(result.Data.DashboardID.Value);
                result.Data.DashboardURL = data.DashboardURL;
            }

            return Ok(result);
        }

        [HttpPost("switchrole")]
        public async Task<IActionResult> SwitchRole([FromBody] SwitchRoleRequest req)
        {
            // Get current user id from the JWT claims already attached to this request
            var idClaim = User.FindFirst("UserId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim.Value, out int userId))
                return Unauthorized(ApiResponse<UserSessionModel>.ErrorResponse("Unable to verify identity."));

            // Confirm the user actually has access to the requested role
            var userRoles = _userMgmtService.GetAllRolesByUser(userId);
            var targetRole = userRoles.FirstOrDefault(r => r.RoleID == req.RoleId);
            if (targetRole == null)
                return Unauthorized(ApiResponse<UserSessionModel>.ErrorResponse("You do not have access to this role."));

            // Fetch full user record to rebuild the token with correct base data
            var user = _userService.GetUserById(userId);
            if (user == null)
                return Unauthorized(ApiResponse<UserSessionModel>.ErrorResponse("User not found."));

            // Re-generate JWT with the NEW role's claims
            var newToken = _jwtHelper.GenerateToken(
                username: user.Username,
                roleName: targetRole.RoleName,
                userId: user.UserID,
                userTypeId: targetRole.RoleID,
                defaultRoleId: targetRole.RoleID,
                userTypeName: targetRole.RoleName,
                staffId: GetStaffID(),
                studentRoleContextDto: null
            );

            var sessionModel = new UserSessionModel
            {
                Token = newToken,
                UserID = user.UserID,
                Username = user.Username,
                UserTypeName = targetRole.RoleName,
                DefaultRoleID = targetRole.RoleID,
            };

            return Ok(ApiResponse<UserSessionModel>.SuccessResponse(sessionModel, "Role switched successfully."));
        }

        
    }
}
