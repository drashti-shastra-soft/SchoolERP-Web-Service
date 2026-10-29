using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing user roles and their permissions through the API.
    /// </summary>
    public class RoleApiController : ControllerBase
    {
        private readonly IUserManagementService _userMgmtService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Settings";

        public RoleApiController(IUserManagementService userMgmtService, IUserMenuPermissionService menuPerm)
        {
            _userMgmtService = userMgmtService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Gets the full list of all defined user roles from the system.
        /// </summary>
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = _userMgmtService.GetAllRoles();
            return Ok(ApiResponse<List<MstRoleViewModel>>.SuccessResponse(roles));
        }

        /// <summary>
        /// Gets the details of one specific role using its unique ID number.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetRoleById(int id)
        {
            var role = _userMgmtService.GetRoleByID(id);
            if (role == null) return NotFound(ApiResponse<MstRoleViewModel>.ErrorResponse("Role not found from specified identifier"));
            return Ok(ApiResponse<MstRoleViewModel>.SuccessResponse(role));
        }

        /// <summary>
        /// Saves a new user role or updates an existing one with the details you provided.
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] MstRoleUpsertRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<int>.ErrorResponse("User is not authenticated."));

            var isCreate = request.RoleID == 0;
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add roles." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit roles." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result =await _userMgmtService.UpsertRole(request, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<int>.SuccessResponse(result.roleId, result.message));
            
            return BadRequest(ApiResponse<int>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Turns a role's active status on or off.
        /// </summary>
        [HttpPost("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change status." });
            
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            request.DoneBy = currentUserId;
            request.IpAddress = ipAddress;
            var result = _userMgmtService.ToggleRoleStatus(request);
            
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Gets the list of allowed actions (like viewing or editing specific menus) for a chosen role.
        /// </summary>
        [HttpGet("{id}/permissions")]
        public IActionResult GetPermissions(int id)
        {
            var matrix = _userMgmtService.GetPermissionsMatrix(id);
            return Ok(ApiResponse<List<RoleMenuPermissionViewModel>>.SuccessResponse(matrix));
        }

        /// <summary>
        /// Saves the chosen list of allowed actions for a role.
        /// </summary>
        [HttpPost("save-permissions")]
        public async Task<IActionResult> SavePermissions([FromBody] MstRolePermissionSaveRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to save permissions." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.SaveRolePermissions(request, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Permanently removes a role from the system's records.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(List<int> id)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete roles." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            var result = _userMgmtService.DeleteRole(id, currentUserId, ipAddress);
            
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) ? userId : 0;
        }

        [HttpGet("GetAllRolesByUser")]
        public IActionResult GetAllRolesByUser(int userId)
        {
            try
            {
                if(userId ==null || userId == 0) 
                {
                    userId= GetCurrentUserId();
                }
                var roles = _userMgmtService.GetAllRolesByUser(userId);
                return Ok(ApiResponse<List<UserRoleViewModel>>.SuccessResponse(roles));
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        [HttpPost("GetAllRoleWithPage")]
        public async Task<IActionResult> GetAllRoleWithPage([FromBody] SearchRequest request)
        {
            try
            {
                request.UserId = GetCurrentUserId();
                var data = await _userMgmtService.GetAllRoleWithPage(request);
                return Ok(ApiResponse<PagedResult<MstRoleViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet("GetAllRolesBySearch")]
        public IActionResult GetAllRolesBySearch(int roleID, string? searchTerm, int companyID, int sessionID)
        {
            try
            {  
                var roles = _userMgmtService.GetAllRolesBySearch(roleID, searchTerm, companyID,sessionID);
                return Ok(ApiResponse<List<SearchByRoleModel>>.SuccessResponse(roles));
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
