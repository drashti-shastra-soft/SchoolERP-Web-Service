using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    // ---------------------------------------------------------------
    // Row shown in the list grid (Index page)
    // ---------------------------------------------------------------
    public class PostalCodeListViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int PostalCodeId { get; set; }
        public string PostalCode { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    // ---------------------------------------------------------------
    // Model bound to PostalCode/Index.cshtml
    // ---------------------------------------------------------------
    public class PostalCodePageViewModel
    {
        public List<PostalCodeListViewModel> PostalCode { get; set; } = new();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();
        public List<CountryModel> Country { get; set; } = new();
        public List<StateModel> State { get; set; } = new();
        public List<CityModel> City { get; set; } = new();

        public string SearchTerm { get; set; }
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }

        public PagePermissions Permissions { get; set; } = new();
    }

    // ---------------------------------------------------------------
    // Model bound to PostalCode/Add.cshtml
    // ---------------------------------------------------------------
    public class PostalCodeAddViewModel
    {
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();
        public List<CountryModel> Country { get; set; } = new();
        public List<StateModel> State { get; set; } = new();
        public List<CityModel> City { get; set; } = new();


        public PostalCodeListViewModel PostalCode { get; set; }
        public PostalCodeListViewModel EditPostalCode { get; set; }

        public PagePermissions Permissions { get; set; } = new();
    }

    // ---------------------------------------------------------------
    // Populated when editing an existing record
    // ---------------------------------------------------------------
    public class PostalCodeEditModel
    {
        public int PostalCodeId { get; set; }
        public string PostalCode { get; set; }
        public string AreaName { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Description { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    // ---------------------------------------------------------------
    // Payload posted from PostalCode.js -> /PostalCode/SavePostalCode
    // ---------------------------------------------------------------
    public class PostalCodeSaveRequest
    {
        public int PostalCodeId { get; set; }
        public string PostalCode { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class ToggleStatusRequest
    {
        public string Ids { get; set; }
        public bool IsActive { get; set; }
    }

    public class SerachPostalCode
    {
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }

    
}
