using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for user registration and login through the API.
    /// </summary>
    public class UsersApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthServices _authService;

        public UsersApiController(IUserService userService, IAuthServices authService)
        {
            _userService = userService;
            _authService = authService;
        }

        /// <summary>
        /// Creates a new user account in the system with the information you provided.
        /// </summary>
        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserUpsertRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(new { Success = false, Message = "User is not authenticated." });

            var (result, message) = _userService.CreateUser(request, currentUserId);
            
            if (result > 0)
                return Ok(new { Success = true, Message = message });
            
            return BadRequest(new { Success = false, Message = message });
        }

        /// <summary>
        /// Verifies a user's login details and provides access if they are correct.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request.Username, request.Password);

            if (response.Success)
                return Ok(new { Success = true, Message = response.Message, Data = response.Data });

            return Unauthorized(new { Success = false, Message = response.Message });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) ? userId : 0;
        }

        [HttpGet("GetUserTypeWiseCount")]
        public async Task<IActionResult> GetUserTypeWiseCount([FromQuery] int companyId)
        {
            try
            {
                if (companyId == 0)
                    return Ok(ApiResponse<List<UserTypeWiseCountModel>>.SuccessResponse(new List<UserTypeWiseCountModel>()));

                var data = await _userService.GetUserTypeWiseCountAsync(companyId);

                return Ok(ApiResponse<List<UserTypeWiseCountModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
