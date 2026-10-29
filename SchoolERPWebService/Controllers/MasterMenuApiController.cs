using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides technical endpoints for setting up and organizing the application's sidebar navigation menus.
    /// </summary>
    public class MasterMenuApiController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MasterMenuApiController(IMenuService menuService)
        {
            _menuService = menuService;
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
        /// Gets the full list of all navigation menus available in the system.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMenus()
        {
            var menus =await _menuService.GetAllMenus();
            return Ok(ApiResponse<List<MenuViewModel>>.SuccessResponse(menus));
        }




        /// <summary>
        /// Gets the details of one specific menu item using its unique ID number.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuById(int id)
        {
            var menu =await _menuService.GetMenuById(id);
            if (menu == null) return NotFound(ApiResponse<MenuViewModel>.ErrorResponse("Menu module block not found"));
            return Ok(ApiResponse<MenuViewModel>.SuccessResponse(menu));
        }

        /// <summary>
        /// Saves a new menu item or updates an existing one with the details you provided.
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] MenuUpsertRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<bool>.ErrorResponse("Menu validation block failed", errors));
            }

            int userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            int mainAccountId = GetClaimInt("MainAccountId", userId);
            int sessionId = GetClaimInt("SessionId", 0);
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var (result, message) =await _menuService.UpsertMenu(request, userId, mainAccountId, sessionId, ipAddress);
            
            if (result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, message));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        /// <summary>
        /// Turns a menu item's visibility on or off.
        /// </summary>
        [HttpPost("toggle-status")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (userId <= 0)
                    return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
                int mainAccountId = GetClaimInt("MainAccountId", userId);
                int sessionId = GetClaimInt("SessionId", 0);
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                request.DoneBy = userId;
                var (result, message) = await _menuService.ToggleMenuStatus(request, mainAccountId, sessionId, ipAddress);

                if (result > 0)
                    return Ok(ApiResponse<bool>.SuccessResponse(true, message));

                return BadRequest(ApiResponse<bool>.ErrorResponse(message));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        /// <summary>
        /// Changes the order in which menu items appear in the navigation sidebar.
        /// </summary>
        [HttpPost("update-order")]
        public IActionResult UpdateOrder([FromBody] List<MenuOrderRequest> orders)
        {
            if (orders == null || !orders.Any()) return BadRequest(ApiResponse<bool>.ErrorResponse("No order data provided."));

            int userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var (result, message) = _menuService.UpdateMenuOrder(orders, userId, ipAddress);
            
            if (result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, message));

            return BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        /// <summary>
        /// Deletes a menu item using its unique ID number.
        /// </summary>
        [HttpPost("delete")]
        public IActionResult Delete(List<int> menuId)
        {
            int userId = GetCurrentUserId();
            if (userId <= 0)
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated."));
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var (result, message) = _menuService.DeleteMenu(menuId, userId, ipAddress);
            
            if (result > 0)
                return Ok(ApiResponse<bool>.SuccessResponse(true, message));

            return BadRequest(ApiResponse<bool>.ErrorResponse(message));
        }

        private int GetCurrentUserId()
        {
            return GetClaimInt(ClaimTypes.NameIdentifier, GetClaimInt("UserId", 0));
        }

        private int GetClaimInt(string claimType, int fallback = 0)
        {
            var claim = User.FindFirst(claimType);
            return (claim != null && int.TryParse(claim.Value, out int value)) ? value : fallback;
        }

        [HttpPost("GetAllMenusWithPage")]
        public async Task<IActionResult> GetAllMenusWithPage([FromBody] SearchRequest request)
        {
            try
            {                
                if (IsSuperAdmin())
                {
                    var result = await _menuService.GetAllMenusWithPage(request);
                    return Ok(ApiResponse<PagedResult<MenuViewModel>>.SuccessResponse(result));
                }
                request.UserId = GetCurrentUserId();
                var data = await _menuService.GetAllMenusWithPage(request);
                return Ok(ApiResponse<PagedResult<MenuViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
