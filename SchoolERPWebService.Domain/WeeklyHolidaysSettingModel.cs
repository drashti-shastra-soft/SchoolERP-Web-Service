using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class WeeklyHolidaysSettingModel
    {
            public int Result { get; set; }       // maps to RESULT
            public string Message { get; set; }   // maps to MESSAGE
            public int Id { get; set; }
            public int? CompanyID { get; set; }
            public string? SchoolName { get; set; }
            public string? SchoolCode { get; set; }
            public bool IsSunday { get; set; }
            public string? SundayNthWeek { get; set; }

            public bool IsMonday { get; set; }
            public string? MondayNthWeek { get; set; }

            public bool IsTuesday { get; set; }
            public string? TuesdayNthWeek { get; set; }

            public bool IsWednesday { get; set; }
            public string? WednesdayNthWeek { get; set; }

            public bool IsThursday { get; set; }
            public string? ThursdayNthWeek { get; set; }

            public bool IsFriday { get; set; }
            public string? FridayNthWeek { get; set; }

            public bool IsSaturday { get; set; }
            public string? SaturdayNthWeek { get; set; }


            public DateTime? EffectiveFrom { get; set; }
            public bool IsActive { get; set; }
            public bool IsDelete { get; set; }
            public DateTime CreatedOn { get; set; }
            public int? CreatedBy { get; set; }
            public DateTime? ModifiedOn { get; set; }
            public int? ModifiedBy { get; set; }

            // Must exist — mapped from TOTALCOUNT column in SP
            public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
            public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
            public int PageSize { get; set; }   // ← PAGESIZE
            public int TotalPages { get; set; }   // ← TOTALPAGES

    }

    public class RequestWeeklyHolidaysSetting 
    {
        public int Id { get; set; }
        public int? CompanyID { get; set; }
        public string? DayOfWeek { get; set; }
        public string? NthWeek { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public bool IsActive { get; set; } = true;
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    public class WeeklyHolidayDayDto
    {
        public string DayOfWeek { get; set; }   // SUNDAY, MONDAY, ...
        public string? nthWeek { get; set; }     // only for SATURDAY, e.g. "2,4"
    }

    public class WeeklyHolidayBatchRequest
    {
        public int CompanyID { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public bool IsActive { get; set; }
        public int UserID { get; set; }
        public string? IPAddress { get; set; }

        public bool IsSunday { get; set; }
        public string? SundayNthWeek { get; set; }

        public bool IsMonday { get; set; }
        public string? MondayNthWeek { get; set; }

        public bool IsTuesday { get; set; }
        public string? TuesdayNthWeek { get; set; }

        public bool IsWednesday { get; set; }
        public string? WednesdayNthWeek { get; set; }

        public bool IsThursday { get; set; }
        public string? ThursdayNthWeek { get; set; }

        public bool IsFriday { get; set; }
        public string? FridayNthWeek { get; set; }

        public bool IsSaturday { get; set; }
        public string? SaturdayNthWeek { get; set; }
    }

    public class WeeklyHolidaysSettingSearchRequest
    {
        public int CompanyID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class WeeklyHolidaysSettingAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public WeeklyHolidaysSettingModel WeeklyHolidays { get; set; } = new WeeklyHolidaysSettingModel();
        public WeeklyHolidaysSettingModel? EditWeeklyHolidays { get; set; } = new WeeklyHolidaysSettingModel();
    }

    public class WeeklyHolidaysSettingPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<WeeklyHolidaysSettingModel> WeeklyHolidaysSetting { get; set; } = new List<WeeklyHolidaysSettingModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
    }
}
