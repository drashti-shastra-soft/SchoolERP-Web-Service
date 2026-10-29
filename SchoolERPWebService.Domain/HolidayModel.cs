using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class HolidayModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int HolidayID { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int? HolidayTypeID { get; set; }
        public string? HolidayName { get; set; }
        public string? HolidayTypeName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? NoOfDays { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class HolidayRequestModel 
    {
        public int HolidayID { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int? HolidayTypeID { get; set; }
        public string? HolidayName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int UserID { get; set; }
        public string? IPAddress { get; set; }

    }

    public class HolidayPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<HolidayModel> Holiday { get; set; } = new List<HolidayModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();
        

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? SectionID { get; set; }
    }

    public class HolidayAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public HolidayModel Holiday{ get; set; } = new HolidayModel();
        public HolidayModel? EditHoliday { get; set; } = new HolidayModel();
        public List<HolidayType> HolidayTypeModel { get; set; } = new();
    }
}
