using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    // --- User Types ---
    
    /// <summary>
    /// Represents the schema configuration for different logical types of users in the system (e.g. Student, Parent, Admin).
    /// </summary>
    public class MstUserTypeViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int UserTypeID { get; set; }
        public string TypeCode { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    /// <summary>
    /// Incoming payload model for creating or updating a User Type.
    /// </summary>
    public class MstUserTypeUpsertRequest
    {
        public int UserTypeID { get; set; }
        public string TypeCode { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    // --- Roles ---
    
    /// <summary>
    /// ViewModel representing a Master System Role bridging permissions and users.
    /// </summary>
    public class MstRoleViewModel
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? RoleDesc { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }


    public class UserRoleViewModel
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int UserID { get; set; }

        public int RoleID { get; set; }
        public string? RoleName { get; set; }
        public bool? IsDefault { get; set; }
    }

    /// <summary>
    /// Form post payload data model for inserting or updating a Role definition.
    /// </summary>
    public class MstRoleUpsertRequest
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? RoleDesc { get; set; }
        public bool IsActive { get; set; }
    }

    // --- Permissions ---
    
    /// <summary>
    /// ViewModel defining a discreet system action/module that can be mapped to roles.
    /// </summary>
    public class MstPermissionViewModel
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string? DisplayLabel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    /// <summary>
    /// This class represents the data structure and schema for MstPermissionUpsertRequest.
    /// </summary>
    public class MstPermissionUpsertRequest
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string? DisplayLabel { get; set; }
        public bool IsActive { get; set; }
    }

    // --- Page Models ---
    
    /// <summary>
    /// Aggregate wrapper model serving the Settings/UserManagement Index page.
    /// Used to initialize datatables without separate AJAX calls on load.
    /// </summary>
    public class MstUserManagementPageViewModel
    {
        public PagePermissions PermissionsButton { get; set; } = PagePermissions.Denied;
        public List<MstUserTypeViewModel> UserTypes { get; set; } = new();
        public List<MstRoleViewModel> Roles { get; set; } = new();
        public List<MstPermissionViewModel> Permissions { get; set; } = new();
    }

    /// <summary>
    /// Hierarchical tree model structure for mapping Role module access.
    /// Represents one Checkbox on the matrix UI.
    /// </summary>
    public class RoleMenuPermissionViewModel
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string MenuIcon { get; set; } = string.Empty;
        public int? ParentID { get; set; }
        public int PermissionID { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public bool HasAccess { get; set; }
    }

    /// <summary>
    /// Helper DTO structure containing Menu to Permission foreign key combinations.
    /// </summary>
    public class RolePermissionPair
    {
        public int MenuID { get; set; }
        public int PermissionID { get; set; }
    }

    /// <summary>
    /// Incoming AJAX request payload containing the checked Matrix permissions mapped to a specific Role.
    /// </summary>
    public class MstRolePermissionSaveRequest
    {
        public int RoleID { get; set; }
        public List<RolePermissionPair> SelectedPermissions { get; set; } = new();
    }
    public class RoleUpsertResult
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int RoleID { get; set; }
    }

    public class SearchByRoleModel 
    {
        public int ReferenceID { get; set; }
        public int UserID { get; set; }
        public string DisplayName { get; set; }
    }
}
