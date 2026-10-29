using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class CountryModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int CountryId { get; set; }

        public string? CountryName { get; set; }
        public string? Description { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
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
    public class CountryRequestModel
    {
        public int CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? Description { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    public class CountryPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<CountryModel> Country { get; set; } = new List<CountryModel>();
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

    public class CountryAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public CountryModel Country { get; set; } = new CountryModel();
        public CountryModel? EditCountry { get; set; } = new CountryModel();
    }
}
