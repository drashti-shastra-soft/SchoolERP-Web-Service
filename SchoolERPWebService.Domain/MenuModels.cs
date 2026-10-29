using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    public class MenuViewModel
    {
        public int MenuID { get; set; }
        public string SubMenusJson { get; set; } = string.Empty;
        public string MenuName { get; set; } = string.Empty;
        public string? DisplayLabel { get; set; }
        public string MenuURL { get; set; } = string.Empty;
        public int? ParentID { get; set; }
        public string? ParentMenuName { get; set; }
        public int DisplayOrder { get; set; }
        public string? MenuIcon { get; set; }
        public string? MenuKey { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public int?MenuLevel{ get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class MenuUpsertRequest
    {
        public int MenuID { get; set; }

        [Required(ErrorMessage = "Menu Name is required")]
        [MaxLength(25, ErrorMessage = "Menu Name cannot exceed 25 characters")]
        public string MenuName { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Display Label cannot exceed 200 characters")]
        public string? DisplayLabel { get; set; }

        [Required(ErrorMessage = "Menu URL is required")]
        [MaxLength(300, ErrorMessage = "Menu URL cannot exceed 300 characters")]
        public string MenuURL { get; set; } = string.Empty;

        public int? ParentID { get; set; }
        public int DisplayOrder { get; set; }
        public string? MenuIcon { get; set; }
        public string? MenuKey { get; set; }
        public bool IsActive { get; set; }
    }

    public class MenuOrderRequest
    {
        public int MenuID { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class MenusPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MenuViewModel> Menus { get; set; } = new();
        public List<MenuViewModel> ParentMenus { get; set; } = new();
    }
}
