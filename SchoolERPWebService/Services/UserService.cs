using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Utilities;
using System.ComponentModel.Design;
using System.Data;
using static System.Collections.Specialized.BitVector32;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing user accounts, such as saving profile information, assigning roles and companies, and managing account security in the database.
    /// </summary>
    public class UserService: IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly EncryptionHelper _encryption;
        private readonly ICompanyService _companyService;

        public UserService(IConfiguration configuration, EncryptionHelper encryption, ICompanyService companyService)
        {
            _configuration = configuration;
            _encryption = encryption;
            _companyService = companyService;
        }

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        public async Task<PagedResult<UserViewModel>> GetAllUsers(UserSearchRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));


                var param = new DynamicParameters();
                if(request.pageIndex ==0 && request.pageSize == 0) 
                {
                    request.pageIndex = 1;
                    request.pageSize = 10;
                }

                param.Add("@PAGEINDEX", request.pageIndex);
                param.Add("@PAGESIZE", request.pageSize);
                param.Add("@SEARCHTERM", request.search);
                param.Add("@COMPANYID", request.companyId);
                param.Add("@ISACTIVE", request.isActive);
                param.Add("@USERTYPEID", request.userTypeId);

                var multi = (await conn.QueryAsync<UserViewModel>(
            "sp_Users_GetAll",
            param,
            commandType: CommandType.StoredProcedure)).ToList();


                int totalRecords = multi.FirstOrDefault()?.TotalCount ?? 0;
                int pageIndex = multi.FirstOrDefault()?.PageIndex ?? 0;
                int pageSize = multi.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<UserViewModel>
                {
                    Data = multi,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };
                return userModel;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


        /// <summary>
        /// Get user count grouped by UserType for dashboard cards
        /// </summary>
        public async Task<List<UserTypeWiseCountModel>> GetUserTypeWiseCountAsync(int companyId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = (await conn.QueryAsync<UserTypeWiseCountModel>(
                "SP_USERS_GETUSERTYPEWISE_COUNT",
                new { COMPANYID = companyId },
                commandType: CommandType.StoredProcedure
            )).ToList();

            // Always return Teacher, HR, Admin even if count is 0
            var defaultTypes = new[]
            {
                new { Id = 1, Name = "Teacher" },
                new { Id = 2, Name = "HR"      },
                new { Id = 3, Name = "Admin"   }
            };

            foreach (var type in defaultTypes)
            {
                if (!result.Any(r => r.UserTypeId == type.Id))
                {
                    result.Add(new UserTypeWiseCountModel
                    {
                        UserTypeId = type.Id,
                        RoleName = type.Name,
                        UserCount = 0
                    });
                }
            }

            return result.OrderBy(r => r.UserTypeId).ToList();
        }

        /// <summary>
        /// Looks up the details of a specific user from the database.
        /// </summary>
        public UserViewModel? GetUserById(int userId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var user = conn.QueryFirstOrDefault<UserViewModel>(
                "sp_Users_GetByID",
                new { UserID = userId },
                commandType: CommandType.StoredProcedure);

            if (user == null)
                return null;

            // Ensure company and role IDs are explicitly loaded if not returned by the SP
            if (user.CompanyIDs == null || user.CompanyIDs.Count == 0) 
            {
                List<int> comId = GetUserCompanyIds(userId);
                user.CompanyIDsCsv = string.Join(",", user.CompanyIDs.Concat(comId));
            }
                

            if (user.RoleIDs == null || user.RoleIDs.Count == 0)
                user.RoleIDs = GetUserRoleIds(userId);

            return user;
        }

        /// <summary>
        /// Fetches the list of role IDs assigned to a specific user from the database.
        /// </summary>
        public List<int> GetUserRoleIds(int userId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = conn.Query(
            "sp_UserRoles_GetByUser",
        new { UserID = userId },
        commandType: CommandType.StoredProcedure);

            List<int> roleIds = new List<int>();

            foreach (var row in result)
            {
                roleIds.Add((int)row.ROLEID);
            }

            return roleIds;
        }

        /// <summary>
        /// Fetches the list of company IDs a user is assigned to from the database.
        /// </summary>
        public List<int> GetUserCompanyIds(int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query(
                   "sp_UserCompanies_GetByUser",
                   new { UserID = userId },
                   commandType: CommandType.StoredProcedure);
               
                var companyIds = new List<int>();
                foreach (var row in result)
                {
                    if (row.COMPANYID != null)
                    {
                        companyIds.Add(Convert.ToInt32(row.COMPANYID));
                    }
                }

                return companyIds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        /// <summary>
        /// Retrieves all active roles from the database.
        /// </summary>
        public List<RoleViewModel> GetRoles()
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<RoleViewModel>(
                "sp_Roles_GetAll_ForDropdown",
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Retrieves all active user categories from the database.
        /// </summary>
        public List<MstUserTypeViewModel> GetUserTypes()
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<MstUserTypeViewModel>(
                "sp_UserTypes_GetAll_ForDropdown",
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Creates a new user and assigns selected roles.
        /// Password hashing and salting are handled by the database.
        /// </summary>
        public (int Result, string Message) CreateUser(UserUpsertRequest request, int createdBy)
        {
            string roleIDs = string.Join(",", request.RoleIDs);

            string encryptedOTPSecret = _encryption.Encrypt(request.OTPSecret);

            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = conn.QueryFirstOrDefault<SpResult>(
                "sp_Users_Upsert",
                new
                {
                    UserID = 0,
                    request.FullName,
                    request.Username,
                    Email = NullIfEmpty(request.Email),
                    PasswordPlain = NullIfEmpty(request.Password),
                    PhoneNo = NullIfEmpty(request.PhoneNo),
                    request.UserTypeID,
                    request.DefaultRoleID,
                    request.DashboardID,
                    request.BackDaysAllow,
                    request.IsOTPEnabled,
                    OTPSecret = NullIfEmpty(encryptedOTPSecret),
                    request.OTPExpiry,
                    request.StartDate,
                    request.EndDate,
                    StartTime = request.StartTime?.ToString(@"hh\:mm\:ss"),
                    EndTime = request.EndTime?.ToString(@"hh\:mm\:ss"),
                    RoleIDs = roleIDs,
                    ModifiedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);

            return result != null
                ? (result.Result, result.Message)
                : (-99, "Unknown error");
        }

        /// <summary>
        /// Updates an existing user's information and role assignments.
        /// </summary>
        public (int Result, string Message) UpdateUser(UserUpsertRequest request, int modifiedBy)
        {
            string roleIDs = string.Join(",", request.RoleIDs);

            string? encryptedOTPSecret =
                !string.IsNullOrEmpty(request.OTPSecret)
                    ? _encryption.Encrypt(request.OTPSecret)
                    : null;

            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = conn.QueryFirstOrDefault<SpResult>(
                "sp_Users_Upsert",
                new
                {
                    request.UserID,
                    request.FullName,
                    request.Username,
                    Email = NullIfEmpty(request.Email),
                    PasswordPlain = NullIfEmpty(request.Password),
                    PhoneNo = NullIfEmpty(request.PhoneNo),
                    request.UserTypeID,
                    request.DefaultRoleID,
                    request.DashboardID,
                    request.BackDaysAllow,
                    request.IsOTPEnabled,
                    OTPSecret = NullIfEmpty(encryptedOTPSecret),
                    request.OTPExpiry,
                    request.StartDate,
                    request.EndDate,
                    StartTime = request.StartTime?.ToString(@"hh\:mm\:ss"),
                    EndTime = request.EndTime?.ToString(@"hh\:mm\:ss"),
                    RoleIDs = roleIDs,
                    ModifiedBy = modifiedBy
                },
                commandType: CommandType.StoredProcedure);

            return result != null
                ? (result.Result, result.Message)
                : (-99, "Unknown error");
        }

        /// <summary>
        /// Collects all necessary data (user details, roles, companies) from the database for the user setup wizard.
        /// </summary>
        public UserWizardViewModel GetUserWizardData(int userId, string roleIds = "")
        {
            var wizard = new UserWizardViewModel();

            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            // 1. User Details
            if (userId > 0)
            {
                wizard.User = GetUserById(userId) ?? new UserViewModel();
                wizard.User.CompanyIDs.AddRange(GetUserCompanyIds(userId));
            }
            else
            {
                wizard.User = new UserViewModel();
            }

            // 2. Roles & Companies
            wizard.AllRoles = GetRoles();
            wizard.AllCompanies = _companyService.GetAllCompanies();

            var parsedRoleIds = (roleIds ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => int.TryParse(r.Trim(), out int id) ? id : 0)
                .Where(id => id > 0)
                .ToList();

            var mergedMatrix = new Dictionary<string, UserPermissionMatrixViewModel>();

            foreach (var roleId in parsedRoleIds)
            {
                var permissions = conn.Query<UserPermissionMatrixViewModel>(
                    "sp_Roles_GetPermissionsMatrix",
                    new { RoleID = roleId },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var item in permissions)
                {
                    string key = $"{item.MenuID}:{item.PermissionID}";
                    bool hasAccess = item.HasAccess;

                    if (mergedMatrix.TryGetValue(key, out var existing))
                    {
                        if (hasAccess)
                        {
                            existing.RoleAccess = true;
                            existing.HasAccess = true;
                        }
                    }
                    else
                    {
                        item.RoleAccess = hasAccess;
                        item.UserOverride = null;
                        item.HasAccess = hasAccess;

                        mergedMatrix[key] = item;
                    }
                }
            }

            // 3. User Overrides
            if (userId > 0)
            {
                try
                {
                    var overrides = conn.Query<UserPermissionMatrixViewModel>(
                        "sp_Users_GetPermissionsMatrix",
                        new
                        {
                            UserID = userId,
                            RoleIDs = roleIds ?? ""
                        },
                        commandType: CommandType.StoredProcedure)
                        .ToList();

                    foreach (var item in overrides)
                    {
                        string key = $"{item.MenuID}:{item.PermissionID}";

                        if (mergedMatrix.TryGetValue(key, out var existing))
                        {
                            existing.UserOverride = item.UserOverride;

                            if (item.UserOverride.HasValue)
                                existing.HasAccess = item.UserOverride.Value;
                        }
                        else if (item.UserOverride.HasValue)
                        {
                            item.RoleAccess = false;
                            item.HasAccess = item.UserOverride.Value;

                            mergedMatrix[key] = item;
                        }
                    }
                }
                catch
                {
                    // Optional SP failed
                }
            }

            wizard.PermissionMatrix = mergedMatrix.Values.ToList();

            return wizard;
        }


        /// <summary>
        /// Executes a database command to save all user details, role assignments, and company assignments at once.
        /// </summary>
        public (int Result, string Message) SaveUserWizard(UserUpsertRequest request, int modifiedBy)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Username))
                    return (-1, "Username is required.");

                if (string.IsNullOrWhiteSpace(request.FullName))
                    return (-1, "Full Name is required.");

                if (request.UserTypeID <= 0)
                    return (-1, "User Type is required.");

                if (request.UserID == 0 && string.IsNullOrWhiteSpace(request.Password))
                    return (-1, "Password is required for new users.");

                if (request.IsOTPEnabled && string.IsNullOrWhiteSpace(request.PhoneNo))
                    return (-1, "Phone number is required when Login with OTP is enabled.");

                string roleIDsStr = string.Join(",", request.RoleIDs);
                string companyIDsStr = string.Join(",", request.CompanyIDs);

                string overridesStr = string.Join(",",
                    request.PermissionOverrides.Select(o =>
                        $"{o.MenuID}:{o.PermissionID}:{(o.IsAllowed ? 1 : 0)}"));

                string? passwordPlain = string.IsNullOrEmpty(request.Password)
                    ? null
                    : request.Password;

                string encryptedEmail = _encryption.Encrypt(request.Email ?? "");
                string encryptedPhone = _encryption.Encrypt(request.PhoneNo ?? "");

                string? encryptedOTPSecret =
                    !string.IsNullOrEmpty(request.OTPSecret)
                        ? _encryption.Encrypt(request.OTPSecret)
                        : null;

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Users_Upsert_Wizard",
                    new
                    {
                        request.UserID,
                        request.FullName,
                        request.Username,
                        Email = (object?)NullIfEmpty(request.Email),
                        PasswordPlain = passwordPlain,
                        PhoneNo = (object?)NullIfEmpty(request.PhoneNo),
                        request.UserTypeID,
                        request.DefaultRoleID,
                        request.DashboardID,
                        request.BackDaysAllow,
                        request.IsOTPEnabled,
                        OTPSecret = (object?)NullIfEmpty(encryptedOTPSecret),
                        request.OTPExpiry,
                        request.StartDate,
                        request.EndDate,
                        StartTime = request.StartTime?.ToString(@"hh\:mm\:ss"),
                        EndTime = request.EndTime?.ToString(@"hh\:mm\:ss"),
                        RoleIDs = roleIDsStr,
                        CompanyIDs = companyIDsStr,
                        PermissionOverrides = overridesStr,
                        ModifiedBy = modifiedBy
                    },
                    commandType: CommandType.StoredProcedure);

                return result == default
                    ? (-99, "No result returned from database.")
                    : result;
            }
            catch (Exception ex)
            {
                return (-99, $"Wizard save failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Activates or deactivates a user account.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="isActive">True to activate, false to deactivate.</param>
        /// <param name="doneBy">User performing the action.</param>
        /// <returns>Operation result and message.</returns>
        public (int Result, string Message) ToggleUserStatus(int userId, bool isActive, int doneBy)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            conn.Execute(
                "sp_ToggleUserStatus",
                new
                {
                    UserID = userId,
                    IsActive = isActive
                },
                commandType: CommandType.StoredProcedure);

            return (1, "Status updated successfully");
        }

        /// <summary>
        /// Marks a user as deleted in the database.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="doneBy">User performing the delete action.</param>
        /// <returns>Operation result and message.</returns>
        public (int Result, string Message) DeleteUser(int userId, int doneBy)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                "sp_Users_Delete",
                new
                {
                    UserID = userId,
                    DoneBy = doneBy
                },
                commandType: CommandType.StoredProcedure);

            return result == default
                ? (-99, "Unknown error")
                : result;
        }

        /// <summary>
        /// Clears any login locks on a user's account.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="doneBy">User performing the unlock action.</param>
        public void UnlockUser(int userId, int doneBy)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            conn.Execute(
                "sp_Users_Unlock",
                new
                {
                    UserID = userId,
                    DoneBy = doneBy
                },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// A helper tool that converts raw database data about a user into a format the application can easily use.
        /// </summary>
        
        private object? NullIfEmpty(string? value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// Checks whether the specified username is unique for the given user.
        /// </summary>
        /// <param name="username">Username to validate.</param>
        /// <param name="userId">Current user ID.</param>
        /// <returns>
        /// True if the username is unique; otherwise, false.
        /// </returns>
        public bool IsUsernameUnique(string username, int userId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = conn.QueryFirstOrDefault<dynamic>(
                "sp_Users_CheckUsernameValid",
                new
                {
                    Username = username,
                    UserID = userId
                },
                commandType: CommandType.StoredProcedure);

            return result != null && Convert.ToBoolean(result.ISUNIQUE);
        }

        /// <summary>
        /// Updates the password for the specified user.
        /// </summary>
        /// <param name="userId">User ID whose password is being updated.</param>
        /// <param name="password">New password.</param>
        /// <param name="modifiedBy">User ID performing the update.</param>
        /// <returns>
        /// A tuple containing the result code and message returned by the stored procedure.
        /// </returns>
        public (int Result, string Message) ChangePassword(int userId, string password, int modifiedBy)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result = conn.QueryFirstOrDefault<dynamic>(
                "sp_Users_UpdatePassword",
                new
                {
                    UserID = userId,
                    PasswordPlain = password,
                    ModifiedBy = modifiedBy
                },
                commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                return (
                    Convert.ToInt32(result.Result),
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }

            return (-99, "Unknown error occurred.");
        }


        private static string? FindColumnName(DataTable table, params string[] expectedNames)
        {
            foreach (DataColumn col in table.Columns)
            {
                foreach (string expected in expectedNames)
                {
                    if (string.Equals(col.ColumnName, expected, StringComparison.OrdinalIgnoreCase))
                        return col.ColumnName;
                }
            }
            return null;
        }

        private static bool HasMatrixColumns(DataTable table)
        {
            return !string.IsNullOrWhiteSpace(FindColumnName(table, "MENUID", "MenuID", "MenuId"))
                   && !string.IsNullOrWhiteSpace(FindColumnName(table, "PermissionID", "PERMISSIONID", "PermissionId"));
        }

        /// <summary>
        /// Updates the active/inactive status for multiple users.
        /// </summary>
        public (int Result, string Message) BulkToggleUserStatus(UserStatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                conn.Open();

                conn.Execute(
                    "sp_UserTypes_BulkToggleStatus",
                    new
                    {
                        USERID = request.UserIds,
                        ISACTIVE = request.IsActive,
                        DONEBY = request.DoneBY
                    },
                    commandType: CommandType.StoredProcedure);

                return (1, "Status updated successfully");
            }
            catch (Exception ex)
            {
                return (-99, ex.Message);
            }
        }

        /// <summary>
        /// Marks multiple users as deleted in the database.
        /// </summary>
        public (bool Result, string Message) DeleteBulkUser(string userId, int doneBy)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                conn.Open();

                var parameters = new DynamicParameters();
                parameters.Add("@USERIDS", userId);
                parameters.Add("@DONEBY", doneBy);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_USERS_DELETE_BULK",
                   parameters,
                   commandType: CommandType.StoredProcedure);
                return (
                     result?.Result == 1,
                     result?.Message ?? "Operation completed."
                 );
                
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
