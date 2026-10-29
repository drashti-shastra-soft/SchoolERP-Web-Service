using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionController : Controller
    {
        private readonly IUserMenuPermissionService _permissionService;
        private readonly IUserManagementService _userManagemenService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        public PermissionController(IUserMenuPermissionService permissionService,
            IUserManagementService userManagemenService, ICompanyService companyService,
            ISessionService sessionService)
        {
            _permissionService = permissionService;
            _userManagemenService = userManagemenService;
            _companyService = companyService;
            _sessionService = sessionService;
        }
        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;
        /// <summary>
        /// Retrieves the ID of the currently authenticated user from the user's claims.
        /// </summary>
        /// <returns>
        /// The user ID if found; otherwise 0.
        /// </returns>
        [HttpGet("current-user-id")]
        public IActionResult GetCurrentUserId()
        {
            var userId = _permissionService.GetCurrentUserId(User);
            return Ok(userId);
        }

        /// <summary>
        /// Checks whether the currently authenticated user has the specified permission
        /// for a particular menu or page.
        /// </summary>
        /// <param name="menuUrlPrefix">
        /// The menu URL prefix used to identify the module or page.
        /// </param>
        /// <param name="permissionName">
        /// The permission name to validate, such as View, Add, Edit, or Delete.
        /// </param>
        /// <returns>
        /// True if the user has permission; otherwise false.
        /// </returns>
        [HttpGet("has-permission")]
        public async Task<IActionResult> HasPermission(
            string menuUrlPrefix,
            string permissionName)
        {
            var result = await _permissionService.Has(
                User,
                menuUrlPrefix,
                permissionName);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves all permissions.
        /// </summary>
        /// <returns>List of permissions.</returns>
        [HttpGet]
        [Route("GetAllPermissions")]
        public IActionResult GetAllPermissions()
        {
            try
            {
                var result =_userManagemenService.GetAllPermissions();

                return Ok(new ApiResponse<List<MstPermissionViewModel>>
                {
                    Success = true,
                    Data = result,
                    Message = "Permissions retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<MstPermissionViewModel>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves the details of a specific permission.
        /// </summary>
        /// <param name="permissionID">Permission identifier.</param>
        /// <returns>Permission details.</returns>
        [HttpGet]
        [Route("GetPermissionByID")]
        public IActionResult GetPermissionByID(int permissionID)
        {
            try
            {
                var result = _userManagemenService.GetPermissionByID(permissionID);

                if (result == null)
                {
                    return Ok(new ApiResponse<MstPermissionViewModel>
                    {
                        Success = false,
                        Message = "Permission not found."
                    });
                }

                return Ok(new ApiResponse<MstPermissionViewModel>
                {
                    Success = true,
                    Data = result,
                    Message = "Permission retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<MstPermissionViewModel>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates or updates a permission.
        /// </summary>
        /// <param name="request">Permission details.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("UpsertPermission")]
        public IActionResult UpsertPermission(
            [FromBody] MstPermissionUpsertRequest request)
        {
            try
            {
                int userId = GetUserId();

                string ipAddress =
                    HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

                var result = _userManagemenService.UpsertPermission(
                    request,
                    userId,
                    ipAddress);

                return Ok(new ApiResponse<SpResult>
                {
                    Success = result.success,
                    Message = result.message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates whether a permission is currently usable.
        /// </summary>
        /// <param name="permissionID">Permission identifier.</param>
        /// <param name="isActive">Active status.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("TogglePermissionStatus")]
        public IActionResult TogglePermissionStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                int userId = GetUserId();
                string ipAddress = string.Empty;
                request.DoneBy = userId;
                var result = _userManagemenService.TogglePermissionStatus(request);

                return Ok(new ApiResponse<SpResult>
                {
                    Success = result.success,
                    Message = result.message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<SpResult>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Deletes a permission from the database.
        /// </summary>
        /// <param name="permissionID">Permission identifier.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("DeletePermission")]
        public IActionResult DeletePermission(List<int> permissionID)
        {
            try
            {
                int userId = GetUserId();
                string ipAddress = string.Empty;

                var result = _userManagemenService.DeletePermission(
                    permissionID,
                    userId,
                    ipAddress);

                return Ok(new ApiResponse<object>
                {
                    Success = result.success,
                    Message = result.message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("GetAllPermissionWithPage")]
        public async Task<IActionResult> GetAllPermissionWithPage([FromBody] SearchRequest request)
        {
            try
            {
                var data = await _userManagemenService.GetAllPermissionWithPage(request);
                return Ok(ApiResponse<PagedResult<MstPermissionViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
