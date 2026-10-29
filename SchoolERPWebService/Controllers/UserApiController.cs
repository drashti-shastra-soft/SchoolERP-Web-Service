using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/user")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing user accounts (like staff and administrators) through the API.
    /// </summary>
    public class UserApiController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Settings";

        public UserApiController(IUserService userService, IUserMenuPermissionService menuPerm)
        {
            _userService = userService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Gets the full list of all users registered in the system.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GetAllUsers([FromBody] UserSearchRequest request)
        {
            var users =await _userService.GetAllUsers(request);
            return Ok(ApiResponse<PagedResult<UserViewModel>>.SuccessResponse(users));
        }

        /// <summary>
        /// Gets the details of one specific user using their unique ID number.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _userService.GetUserById(id);
            if (user == null) return NotFound(ApiResponse<UserViewModel>.ErrorResponse("User not found"));
            return Ok(ApiResponse<UserViewModel>.SuccessResponse(user));
        }

        /// <summary>
        /// Gets the list of roles assigned to a specific user.
        /// </summary>
        [HttpGet("{id}/roles")]
        public IActionResult GetUserRoleIds(int id)
        {
            var roleIds = _userService.GetUserRoleIds(id);
            return Ok(ApiResponse<List<int>>.SuccessResponse(roleIds));
        }

        /// <summary>
        /// Gets a list of all available roles to show in a selection dropdown.
        /// </summary>
        [HttpGet("roles-dropdown")]
        public IActionResult GetRoles()
        {
            var roles = _userService.GetRoles();
            return Ok(ApiResponse<List<RoleViewModel>>.SuccessResponse(roles));
        }

        /// <summary>
        /// Gets a list of all available user types (like Admin or Staff) to show in a selection dropdown.
        /// </summary>
        [HttpGet("types-dropdown")]
        public IActionResult GetUserTypes()
        {
            var types = _userService.GetUserTypes();
            return Ok(ApiResponse<List<MstUserTypeViewModel>>.SuccessResponse(types));
        }

        /// <summary>
        /// Checks if a username is already taken or if it is available to use.
        /// </summary>
        [HttpGet("check-username")]
        public IActionResult CheckUsername([FromQuery] string username, [FromQuery] int userId = 0)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(ApiResponse<bool>.ErrorResponse("Username is required"));

            bool isUnique = _userService.IsUsernameUnique(username, userId);
            
            // We return the boolean naturally.
            return Ok(ApiResponse<bool>.SuccessResponse(isUnique, isUnique ? "Username is available" : "Username is already taken"));
        }

        /// <summary>
        /// Saves a new user or updates an existing one with the details you provided.
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] UserUpsertRequest request)
        {
            int actingUser = GetCurrentUserId();
            if (actingUser <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            var isCreate = request.UserID == 0;
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add users." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit users." });

            (int result, string message) response;

            // Route dynamically based on Primary Key state
            if (request.UserID == 0)
                response = _userService.CreateUser(request, actingUser);
            else
                response = _userService.UpdateUser(request, actingUser);

            if (response.result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(response.message));
        }

        /// <summary>
        /// Turns a user account on or off.
        /// </summary>
        [HttpPost("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromQuery] int userId, [FromQuery] bool isActive)
        {
            int actingUser = GetCurrentUserId();
            if (actingUser <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change status." });

            var response = _userService.ToggleUserStatus(userId, isActive, actingUser);
            if (response.Result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.Message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(response.Message));
        }

        /// <summary>
        /// Permanently removes a user account from the system.
        /// </summary>
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            int actingUser = GetCurrentUserId();
            if (actingUser <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete users." });

            var response = _userService.DeleteUser(id, actingUser);
            if (response.Result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.Message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(response.Message));
        }

        /// <summary>
        /// Unlocks a user account that was locked due to too many failed login attempts.
        /// </summary>
        [HttpPost("unlock/{id}")]
        public IActionResult Unlock(int id)
        {
            int actingUser = GetCurrentUserId();
            if (actingUser <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            _userService.UnlockUser(id, actingUser);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "User account unlocked"));
        }

        /// <summary>
        /// Gets all the information needed to show the step-by-step user creation assistant.
        /// </summary>
        [HttpGet("wizard-data")]
        public IActionResult GetWizardData([FromQuery] int userId = 0, [FromQuery] string roleIds = "")
        {
            var data = _userService.GetUserWizardData(userId, roleIds);
            return Ok(ApiResponse<UserWizardViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Saves all the information collected during the step-by-step user creation process.
        /// </summary>
        [HttpPost("save-wizard")]
        public async Task<IActionResult> SaveWizard([FromBody] UserUpsertRequest request)
        {
            int actingUser = GetCurrentUserId();
            if (actingUser <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            var isCreate = request.UserID == 0;
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add users." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit users." });

            var response = _userService.SaveUserWizard(request, actingUser);

            if (response.Result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.Message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(response.Message));
        }

        /// <summary>
        /// Turns a user account on or off.
        /// </summary>
        [HttpPost("bulk-toggle-status")]
        public async Task<IActionResult> BulkToggleStatus([FromBody] UserStatusUpdateRequest request)
        {
            int actingUser = GetCurrentUserId();
            if (actingUser <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change status." });
            request.DoneBY = actingUser;
            var response = _userService.BulkToggleUserStatus(request);
            if (response.Result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.Message));

            return BadRequest(ApiResponse<bool>.ErrorResponse(response.Message));
        }

        /// <summary>
        /// Permanently removes a user account from the system.
        /// </summary>
        [HttpPost("delete-bulk")]
        public async Task<IActionResult> DeleteBulkUser([FromQuery] string ids = "")
        {
            int actingUser = GetCurrentUserId();
            if (actingUser <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete users." });

            var response = _userService.DeleteBulkUser(ids, actingUser);
            if (response.Result !=false)
                return Ok(ApiResponse<bool>.SuccessResponse(true, response.Message));

            return BadRequest(ApiResponse<bool>.ErrorResponse(response.Message));
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) ? userId : 0;
        }
    }
}
