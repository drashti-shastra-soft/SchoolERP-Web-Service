namespace SchoolERPWebService.Domain
{
    /// <summary>
    /// Payload structure representing client login credentials.
    /// </summary>
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int IsUserLogin { get; set; }
    }

    /// <summary>
    /// DTO object caching active user data within JWT or session storage.
    /// Includes vital access parameters like UserType and DefaultRole ID to evaluate permissions globally.
    /// </summary>
    public class UserSessionModel
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserTypeName { get; set; } = string.Empty;
        public int DefaultRoleID { get; set; }
        public string DefaultRoleName { get; set; } = string.Empty;
        public int UserTypeID { get; set; }
        public int? DashboardID { get; set; }
        public string Token { get; set; } = string.Empty;
        public string? DashboardURL { get; set; }
        public int? StaffID { get; set; }
        public int? StudentID { get; set; }
        public int? ParentUserID { get; set; }
        public string? StudentPhoto { get; set; }


    }

    /// <summary>
    /// View model for displaying a user in the listing table.
    /// Aligned with TDD 12.7 tbl_mst_users schema.
    /// </summary>
    public class UserViewModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string COMPANIESJSON { get; set; } = string.Empty;

        // Type & Role
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        public int? DefaultRoleID { get; set; }
        public string DefaultRoleName { get; set; } = string.Empty;
        public int? DashboardID { get; set; }

        public string? CompanyIDsCsv { get; set; }

        public List<int> CompanyIDs =>
            string.IsNullOrWhiteSpace(CompanyIDsCsv)
                ? new List<int>()
                : CompanyIDsCsv.Split(',')
                    .Select(int.Parse)
                    .ToList();
        //public List<int> CompanyIDs { get; set; } = new();
        public List<int> RoleIDs { get; set; } = new();

        // Role names from M:M mapping (concatenated)
        public string RoleNames { get; set; } = string.Empty;

        // Access control
        public int BackDaysAllow { get; set; }
        public bool IsOTPEnabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? OTPSecret { get; set; }
        public DateTime? OTPExpiry { get; set; }

        // Status
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime? LastLoginOn { get; set; }

        // Audit
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }


        // Must exist — mapped from TOTALCOUNT column in SP
        public int TotalCount { get; set; }   // ← TOTALCOUNT
        public int PageIndex { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    /// <summary>
    /// Request model for creating/updating a user via the 3-step wizard.
    /// Supports many-to-many role/company mappings and individual permission overrides.
    /// </summary>
    public class UserUpsertRequest
    {
        public int UserID { get; set; }
        public string op { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;    // Plain-text; encrypted in SQL SP
        public string PhoneNo { get; set; } = string.Empty;

        // Type & Defaults
        public int UserTypeID { get; set; }
        public int? DefaultRoleID { get; set; }
        public int? DashboardID { get; set; }

        // M:M Assignments
        public List<int> RoleIDs { get; set; } = new();
        public List<int> CompanyIDs { get; set; } = new();    // NEW: Multi-company support

        // Permission Overrides
        public List<UserPermissionOverrideRequest> PermissionOverrides { get; set; } = new(); // NEW: Individual overrides

        // Access control
        public int BackDaysAllow { get; set; }
        public bool IsOTPEnabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? OTPSecret { get; set; }
        public DateTime? OTPExpiry { get; set; }
    }

    /// <summary>
    /// Explicit override for an individual user's permission node.
    /// </summary>
    public class UserPermissionOverrideRequest
    {
        public int MenuID { get; set; }
        public int PermissionID { get; set; }
        public bool IsAllowed { get; set; }
    }

    /// <summary>
    /// Tree view model for user permission matrix with inheritance information.
    /// </summary>
    public class UserPermissionMatrixViewModel
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuIcon { get; set; } = string.Empty;
        public int? ParentID { get; set; }
        public int PermissionID { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        
        public bool RoleAccess { get; set; }      // From assigned roles
        public bool? UserOverride { get; set; }    // NULL=Inherit, true=Allow, false=Deny
        public bool HasAccess { get; set; }       // Resulting effective access
    }

    /// <summary>
    /// Comprehensive data package required to hydrate the 3-step User Wizard.
    /// </summary>
    public class UserWizardViewModel
    {
        public UserViewModel User { get; set; } = new();
        public List<RoleViewModel> AllRoles { get; set; } = new();
        public List<MstCompanyViewModel> AllCompanies { get; set; } = new();
        public List<UserPermissionMatrixViewModel> PermissionMatrix { get; set; } = new();
    }

    /// <summary>
    /// Represents a discrete permission configuration mapped in the system.
    /// </summary>
    public class PermissionViewModel
    {
        public int PermissionID { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string ModuleGroup { get; set; } = string.Empty;
        public bool HasPermission { get; set; }
    }

    /// <summary>
    /// Simple summary object representing an existing Role.
    /// </summary>
    public class RoleViewModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Payload for creating or updating a simplistic role architecture.
    /// </summary>
    public class RoleUpsertRequest
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request mapping structure binding a generic list of permission IDs to a given Role.
    /// Used during bulk permission saves.
    /// </summary>
    public class RolePermissionSaveRequest
    {
        public int RoleID { get; set; }
        public List<int> PermissionIDs { get; set; } = new();
    }

    /// <summary>
    /// Container model serving the Roles View listing page.
    /// Hydrates UI with Role matrix and general lookup tables.
    /// </summary>
    public class RolesPageViewModel
    {
        public List<RoleViewModel> Roles { get; set; } = new();
        public List<PermissionViewModel> AllPermissions { get; set; } = new();
    }

    /// <summary>
    /// Page view model for User Management listing.
    /// Includes dropdown sources for User Types and Roles.
    /// </summary>
    public class UsersPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<UserViewModel> Users { get; set; } = new();
        public List<RoleViewModel> Roles { get; set; } = new();
        public List<MstUserTypeViewModel> UserTypes { get; set; } = new();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public UserViewModel? EditUser { get; set; }

        public int TotalPages => PageSize > 0
                             ? (int)Math.Ceiling((double)TotalRecords / PageSize)
                             : 0;

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public int? UserTypeId { get; set; }
    }

    /// <summary>
    /// Configuration model for overriding static string localization bindings.
    /// </summary>
    public class TranslationUpdateModel
    {
        public string Language { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public class UserStatusUpdateRequest
    {
        public string UserIds { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int DoneBY { get; set; }
    }

    public class UserSearchRequest
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string? search { get; set; }
        public int? companyId { get; set; }
        public bool? isActive { get; set; }
        public int? userTypeId { get; set; }
    }

    public class StudentRoleContextDto
    {
        public int? StudentID { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int StudentUserID { get; set; }
        public int? ParentUserID { get; set; }
        public string RollNo { get; set; }
        public string AdmissionNo { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? StudentPhoto { get; set; }
    }

    public class StaffRoleContextDto
    {
        public int StaffID { get; set; }
        public int StaffUserID { get; set; }
    }

    public class SwitchRoleRequest
    {
        public int RoleId { get; set; }
    }

    public class UserTypeWiseCountModel
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int UserTypeId { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
    }
}
