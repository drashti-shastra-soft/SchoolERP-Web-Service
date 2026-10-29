using System.Security.Claims;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for checking if a user has permission to view a specific page or perform a certain action.
    /// </summary>
    public interface IUserMenuPermissionService
    {
        /// <summary>
        /// Gets the unique ID of the person currently logged in.
        /// </summary>
        int GetCurrentUserId(ClaimsPrincipal user);

        /// <summary>
        /// Checks if the logged-in user is allowed to do something (like 'Edit' or 'Delete') on a specific page.
        /// </summary>
        Task<bool> Has(ClaimsPrincipal user, string menuUrlPrefix, string permissionName);
    }
}
