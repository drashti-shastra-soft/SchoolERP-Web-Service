using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Helpers;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuApiController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IUserManagementService _userManagementService;
        private readonly PermissionHelper _permHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MenuApiController(IMenuService menuService, IUserManagementService userManagementService, PermissionHelper permHelper, IHttpContextAccessor httpContextAccessor)
        {
            _menuService = menuService;
            _userManagementService = userManagementService;
            _permHelper = permHelper;
            _httpContextAccessor = httpContextAccessor;
        }
      
        private int? GetRoleId()
        {
            var staffClaim = User.FindFirst("RoleId")?.Value;
            return int.TryParse(staffClaim, out var roleId) ? roleId : null;
        }

        //                                                                              ^^ capital ID
        /// <summary>
        /// Gets the full list of all registered school companies from the system.
        /// </summary>
        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var data =await _menuService.GetAllMenus();
            return Ok(ApiResponse<List<MenuViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Retrieves user permissions based on the specified user ID.
        /// Optionally filters permissions by menu URL prefix and permission name.
        /// Returns a list of permissions assigned to the user.
        /// </summary>
        /// <returns>
        /// Returns a list of user permissions including menu access and assigned permission details.
        /// </returns>
        [HttpPost("GetUserPermissions")]
        public async Task<IActionResult> GetUserPermissions([FromQuery] int userId, [FromQuery] string? menuUrlPrefix = null, [FromQuery] string? permissionName = null, int? roleID = null)
        {
            if(roleID==null || roleID == 0) 
            {
                roleID =  GetRoleId();
            }
            var data = await _userManagementService.GetUserPermissions(userId, menuUrlPrefix, permissionName, roleID);
            return Ok(ApiResponse<List<UserPermissionViewModel>>.SuccessResponse(data));
        }

    }
}
