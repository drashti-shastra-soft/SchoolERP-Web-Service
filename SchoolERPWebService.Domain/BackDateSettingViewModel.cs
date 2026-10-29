using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class BackDateSettingViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int SettingID { get; set; }
        public int UserID { get; set; }
        public int? RoleID { get; set; }
        public string RoleName { get; set; }   // "All Roles (Default)" when RoleID is null
        public string ModuleType { get; set; }
        public int BackDaysAllowed { get; set; }
        public bool IsUnlimited { get; set; }
        public bool IsActive { get; set; }
        public string StaffName { get; set; }
        public int StaffID { get; set; }
        public string? RoleSettingsJson { get; set; }
        public string? StaffCode { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }

        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class BackDateSettingPageViewModel
    {
        public List<BackDateSettingViewModel> StaffAttendanceSettings { get; set; } = new();
        public List<MstRoleViewModel> Roles { get; set; } = new();
        public List<MstRoleViewModel> Staff { get; set; } = new();
        public PagePermissions Permissions { get; set; }

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalPages => PageSize > 0
                             ? (int)Math.Ceiling((double)TotalRecords / PageSize)
                             : 0;

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }

    public class SaveBackDateSettingRequest
    {
        public int SettingID { get; set; }
        public int? CompanyID { get; set; }
        public int? RoleID { get; set; }
        public int? StaffID { get; set; }
        public string? ModuleType { get; set; }
        public string? RoleSettingsJson { get; set; }
        public int BackDaysAllowed { get; set; }
        public bool IsUnlimited { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    public class BackDateSettingSearchRequest
    {
        public int CompanyID { get; set; }
        public int? RoleId { get; set; }
        public int? StaffId { get; set; }
        public string? SearchKeyword { get; set; }
        public string? ModuleType { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }

    public class BackDateSettingAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public BackDateSettingViewModel HolidayTypes { get; set; } = new BackDateSettingViewModel();
        public BackDateSettingViewModel? EditBackDateSetting { get; set; } = new BackDateSettingViewModel();
        public List<HRStaffViewModel>? StaffList { get; set; } = new List<HRStaffViewModel>();
    }
}
