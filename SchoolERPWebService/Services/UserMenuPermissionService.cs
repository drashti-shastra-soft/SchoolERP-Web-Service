using SchoolERPWebService.Interfaces;
using System.Security.Claims;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of checking user permissions. It looks at who the user is and what roles they have to decide if they should be allowed to access certain parts of the application.
    /// </summary>
    public class UserMenuPermissionService: IUserMenuPermissionService
    {
        private readonly IUserManagementService _userManagementService;

        public UserMenuPermissionService(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Extracts the user's ID from their login information.
        /// </summary>
        public int GetCurrentUserId(ClaimsPrincipal user)
        {
            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier) ?? user?.FindFirst("UserId");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var id))
                return 0;
            return id;
        }

        /// <summary>
        /// Determines if a user has the right to perform a specific action (like viewing or adding data) on a specific part of the website.
        /// </summary>
        public async Task<bool> Has(ClaimsPrincipal user, string menuUrlPrefix, string permissionName)
        {
            var userId = GetCurrentUserId(user);
            if (userId <= 0) return false;
            if (IsSuperAdmin(user)) return true;
            if (string.IsNullOrWhiteSpace(menuUrlPrefix) || string.IsNullOrWhiteSpace(permissionName))
                return false;

            var key = menuUrlPrefix.Trim().TrimEnd('/');
            if (string.IsNullOrEmpty(key)) return false;

            var perms = await _userManagementService.GetUserPermissions(userId, key, permissionName) ;
            return true;   
        }

        /// <summary>
        /// Checks if the logged-in user is a 'Super Admin' who has permission to do everything in the system.
        /// </summary>
        private static bool IsSuperAdmin(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true) return false;

            return user.Claims.Any(c =>
                (string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "Role", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserTypeName", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase)) &&
                IsSuperAdminValue(c.Value));
        }

        /// <summary>
        /// A small helper tool that checks if a text value means 'Super Admin', ignoring spaces or special characters.
        /// </summary>
        private static bool IsSuperAdminValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(normalized, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
