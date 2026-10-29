using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Filters
{
    /// <summary>
    /// Enforces "View" permission for MVC pages that map to a row in the master menu (MenuURL).
    /// Direct URL access without View permission shows Unauthorized.
    /// </summary>
    public sealed class MenuPermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IMenuService _menuService;
        private readonly IUserManagementService _userManagementService;

        public MenuPermissionAuthorizationFilter(IMenuService menuService, IUserManagementService userManagementService)
        {
            _menuService = menuService;
            _userManagementService = userManagementService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var http = context.HttpContext;
            var path = http.Request.Path.Value ?? "/";

            if (ShouldSkipPath(path))
                return;

            var user = http.User;
            if (user.Identity?.IsAuthenticated != true)
                return;

            if (IsSuperAdmin(user))
                return;

            var userId = GetUserId(user);
            if (userId <= 0)
                return;

            var menus = (await _menuService.GetAllMenus())
                .ToList();

            var match = FindBestMenuMatch(path, menus);
            if (match == null)
                return;

            var permissions = await _userManagementService.GetUserPermissionsAsync(userId,0);

            var canView = permissions.Any(p =>
                p.MenuID == match.MenuID &&
                string.Equals(p.PermissionName, "View", StringComparison.OrdinalIgnoreCase));

            if (!canView)
            {
                context.Result = new ViewResult
                {
                    ViewName = "Unauthorized"
                };
            }
        }
        private static bool ShouldSkipPath(string path)
        {
            var p = path.TrimEnd('/').ToLowerInvariant();
            if (string.IsNullOrEmpty(p)) p = "/";

            if (p.StartsWith("/auth", StringComparison.OrdinalIgnoreCase)) return true;
            if (p.StartsWith("/api", StringComparison.OrdinalIgnoreCase)) return true;
            if (p.StartsWith("/lib", StringComparison.OrdinalIgnoreCase)) return true;
            if (p.StartsWith("/css", StringComparison.OrdinalIgnoreCase)) return true;
            if (p.StartsWith("/js", StringComparison.OrdinalIgnoreCase)) return true;
            if (p.StartsWith("/assets", StringComparison.OrdinalIgnoreCase)) return true;
            if (p.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        /// <summary>
        /// Longest MenuURL prefix match so /User/Index maps to menu with URL /User or /User/Index.
        /// </summary>
        private static MenuViewModel? FindBestMenuMatch(string requestPath, List<MenuViewModel> menus)
        {
            var normalizedPath = NormalizePath(requestPath);
            MenuViewModel? best = null;
            var bestLen = -1;

            foreach (var menu in menus)
            {
                var mu = NormalizePath(menu.MenuURL);
                if (mu.Length == 0) continue;

                if (normalizedPath.Equals(mu, StringComparison.OrdinalIgnoreCase) ||
                    normalizedPath.StartsWith(mu + "/", StringComparison.OrdinalIgnoreCase))
                {
                    if (mu.Length > bestLen)
                    {
                        bestLen = mu.Length;
                        best = menu;
                    }
                }
            }

            return best;
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "/";
            var p = path.Trim();
            if (!p.StartsWith('/')) p = "/" + p;
            return p.TrimEnd('/');
        }

        private static int GetUserId(ClaimsPrincipal user)
        {
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
            if (idClaim == null || !int.TryParse(idClaim.Value, out var id))
                return 0;
            return id;
        }

        private static bool IsSuperAdmin(ClaimsPrincipal user)
        {
            return user.Claims.Any(c =>
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
            return string.Equals(normalized, "SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
