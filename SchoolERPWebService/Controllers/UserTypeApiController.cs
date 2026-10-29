using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing different categories of users (like 'Employee' or 'Student') through the API.
    /// </summary>
    public class UserTypeApiController : ControllerBase
    {
        private readonly IUserManagementService _userMgmtService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Settings";

        public UserTypeApiController(IUserManagementService userMgmtService, IUserMenuPermissionService menuPerm)
        {
            _userMgmtService = userMgmtService;
            _menuPerm = menuPerm;
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
        /// <summary>
        /// Gets the full list of all defined user categories from the system.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _userMgmtService.GetAllUserTypes();
            return Ok(ApiResponse<List<MstUserTypeViewModel>>.SuccessResponse(data));
        }

        [HttpPost("GetAllUserTypesWithPage")]
        public async Task<IActionResult> GetAllUserTypesWithPage([FromBody] SearchRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();               
               
                if (IsSuperAdmin())
                {
                    var datas = await _userMgmtService.GetAllUserTypesWithPage(request);
                    return Ok(ApiResponse<PagedResult<MstUserTypeViewModel>>.SuccessResponse(datas));
                }
                request.UserId = userId;
                var data = await _userMgmtService.GetAllUserTypesWithPage(request);
                return Ok(ApiResponse<PagedResult<MstUserTypeViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the details of one specific user category using its unique ID number.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var data = _userMgmtService.GetUserTypeByID(id);
            if (data == null)
                return NotFound(ApiResponse<MstUserTypeViewModel>.ErrorResponse("User type not found"));
            
            return Ok(ApiResponse<MstUserTypeViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Saves a new user category or updates an existing one with the name you provided.
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] MstUserTypeUpsertRequest request)
        {
            int currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));

            var isCreate = request.UserTypeID == 0;
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add user types." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit user types." });

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            var result = _userMgmtService.UpsertUserType(request, currentUserId, ipAddress);
            if (result.success)
                return Ok(ApiResponse<bool>.SuccessResponse(true, result.message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(result.message));
        }

        /// <summary>
        /// Turns a user category on or off.
        /// </summary>       

        [HttpPost("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change section status." });

                int userId = GetCurrentUserId();
                var (success, message) = _userMgmtService.ToggleUserTypeStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            return (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)) ? userId : 0;
        }

        /// <summary>
        /// Deletes one or more sections by their IDs.
        /// Use this to remove sections from the system. Requires delete permission.
        /// </summary>
        /// <param name="ids">List of section IDs to delete.</param>
        /// <returns>Result indicating success or failure of the operation.</returns>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete sections." });

                int userId = GetCurrentUserId();
                var (success, message) = _userMgmtService.DeleteUserType(ids, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
