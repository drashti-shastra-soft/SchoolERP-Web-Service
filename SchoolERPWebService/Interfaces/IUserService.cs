using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// This interface defines the rules for managing user accounts, including their personal details, roles, and which companies they can access.
    /// </summary>
    public interface IUserService
    {
        /// <summary>Gets a list of all users registered in the system.</summary>
        Task<PagedResult<UserViewModel>> GetAllUsers(UserSearchRequest request);

        Task<List<UserTypeWiseCountModel>> GetUserTypeWiseCountAsync(int companyId);

        /// <summary>Finds the details of a specific user using their unique ID.</summary>
        UserViewModel? GetUserById(int userId);

        /// <summary>Gets a list of all roles assigned to a specific user.</summary>
        List<int> GetUserRoleIds(int userId);

        /// <summary>Gets a list of all active roles available for assigning to users.</summary>
        List<RoleViewModel> GetRoles();

        /// <summary>Gets a list of all active user categories (like 'Admin' or 'Staff').</summary>
        List<MstUserTypeViewModel> GetUserTypes();

        /// <summary>Creates a new user account with their password and assigned roles.</summary>
        (int Result, string Message) CreateUser(UserUpsertRequest request, int createdBy);

        /// <summary>Updates an existing user's information and roles.</summary>
        (int Result, string Message) UpdateUser(UserUpsertRequest request, int modifiedBy);

        /// <summary>Gets a list of all school companies a specific user is allowed to access.</summary>
        List<int> GetUserCompanyIds(int userId);

        /// <summary>
        /// Gets all the information needed for the multi-step user setup form (wizard).
        /// </summary>
        UserWizardViewModel GetUserWizardData(int userId, string roleIds = "");

        /// <summary>
        /// Saves all the information from the user setup form in one go.
        /// </summary>
        (int Result, string Message) SaveUserWizard(UserUpsertRequest request, int modifiedBy);

        /// <summary>Turns a user's account on or off.</summary>
        (int Result, string Message) ToggleUserStatus(int userId, bool isActive, int doneBy);

        /// <summary>Removes a user from the system.</summary>
        (int Result, string Message) DeleteUser(int userId, int doneBy);

        /// <summary>Unlocks a user's account if it was locked (e.g., after too many wrong password attempts).</summary>
        void UnlockUser(int userId, int doneBy);

        /// <summary>Checks if a username is already being used by someone else.</summary>
        bool IsUsernameUnique(string username, int userId);

        /// <summary>Updates a user's password.</summary>
        (int Result, string Message) ChangePassword(int userId, string password, int modifiedBy);
        (int Result, string Message) BulkToggleUserStatus(UserStatusUpdateRequest request);
        (bool Result, string Message) DeleteBulkUser(string userId, int doneBy);
    }
}
