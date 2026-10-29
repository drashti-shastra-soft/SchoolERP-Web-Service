using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Helpers
{
    /// <summary>
    /// Helper class responsible for loading and validating
    /// user permissions throughout the application.
    /// Supports permission checks by Menu ID, Menu Key,
    /// and Menu URL, including Super Admin access handling.
    /// </summary>
    public class PermissionHelper
    {
        private readonly IUserManagementService _userMgmtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private List<UserPermissionViewModel>? _userPermissions = null;
        private bool _isSuperAdmin = false;

        public PermissionHelper(IUserManagementService userMgmtService, IHttpContextAccessor httpContextAccessor)
        {
            _userMgmtService = userMgmtService;
            _httpContextAccessor = httpContextAccessor;
        }
        
        /// <summary>
        /// Loads and caches permissions for the specified user.
        /// If the user is identified as a Super Admin,
        /// permission checks are automatically granted.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user.
        /// </param>
        public async Task LoadPermissions(int userId)
        {
            if (_userPermissions == null)
            {
                _isSuperAdmin = IsSuperAdmin(_httpContextAccessor.HttpContext?.User);
                if (_isSuperAdmin)
                {
                    _userPermissions = new List<UserPermissionViewModel>();
                    return;
                }
                _userPermissions =await _userMgmtService.GetUserPermissionsAsync(userId,0);
            }
        }
        /// <summary>
        /// Determines whether the current user has
        /// a specific permission for the specified menu.
        /// </summary>
        /// <param name="menuId">
        /// The menu identifier.
        /// </param>
        /// <param name="permissionName">
        /// The permission to check (View, Add, Edit, Delete, etc.).
        /// </param>
        /// <returns>
        /// True if permission exists; otherwise false.
        /// </returns>
        public bool HasPermission(int menuId, string permissionName)
        {
            if (_isSuperAdmin) return true;
            if (_userPermissions == null) return false;
            return _userPermissions.Any(p => p.MenuID == menuId && p.PermissionName.Equals(permissionName, System.StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// Determines whether the current user has
        /// a specific permission for the specified menu key.
        /// </summary>
        /// <returns>
        /// True if permission exists; otherwise false.
        /// </returns>
        public bool HasPermissionByKey(string menuKey, string permissionName)
        {
            if (_isSuperAdmin) return true;
            if (_userPermissions == null) return false;
            return _userPermissions.Any(p => p.MenuKey.Equals(menuKey, System.StringComparison.OrdinalIgnoreCase) && p.PermissionName.Equals(permissionName, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether the current user has
        /// a specific permission for the specified menu URL.
        /// </summary>
        /// <returns>
        /// True if permission exists; otherwise false.
        /// </returns>
        public bool HasPermissionByUrl(string menuUrl, string permissionName)
        {
            if (_isSuperAdmin) return true;
            if (_userPermissions == null || string.IsNullOrWhiteSpace(menuUrl)) return false;
            var key = menuUrl.Trim().TrimEnd('/');
            return _userPermissions.Any(p =>
                !string.IsNullOrEmpty(p.MenuURL) &&
                p.PermissionName.Equals(permissionName, System.StringComparison.OrdinalIgnoreCase) &&
                (p.MenuURL.Equals(key, System.StringComparison.OrdinalIgnoreCase) ||
                 p.MenuURL.StartsWith(key + "/", System.StringComparison.OrdinalIgnoreCase) ||
                 p.MenuURL.Contains(key, System.StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Returns all loaded permissions for the current user.
        /// </summary>
        /// <returns>
        /// List of user permissions.
        /// </returns>
        public List<UserPermissionViewModel> GetPermissions() => _userPermissions ?? new List<UserPermissionViewModel>();

        /// <summary>
        /// Determines whether the authenticated user
        /// belongs to the Super Admin role.
        /// </summary>
        /// <param name="user">
        /// The authenticated user principal.
        /// </param>
        /// <returns>
        /// True if the user is a Super Admin; otherwise false.
        /// </returns>
        private static bool IsSuperAdmin(ClaimsPrincipal? user)
        {
            if (user?.Identity?.IsAuthenticated != true) return false;

            return user.Claims.Any(c =>
                (string.Equals(c.Type, ClaimTypes.Role, System.StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "Role", System.StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserTypeName", System.StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserType", System.StringComparison.OrdinalIgnoreCase)) &&
                IsSuperAdminValue(c.Value));
        }

        /// <summary>
        /// Validates whether the specified role value
        /// represents a Super Admin role.
        /// </summary>
        /// <param name="value">
        /// The role value extracted from user claims.
        /// </param>
        /// <returns>
        /// True if the value represents Super Admin; otherwise false.
        /// </returns>
        private static bool IsSuperAdminValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(normalized, "SuperAdmin", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
