using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class CityModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int CityId { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string CityName { get; set; }
        public string DisplayLabel { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int USERID { get; set; }
        public string? IPADDRESS { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }
    public class CityUpsertRequest
    {
        public int CityId { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string CityName { get; set; }
        public string DisplayLabel { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string? Description { get; set; }
        public bool ISACTIVE { get; set; }
        public int USERID { get; set; }
        public string? IPADDRESS { get; set; }
    }

    public class CityApiResponse
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public string? TECHNICALMESSAGE { get; set; }
        public int? ERRORLINE { get; set; }
    }

    public class CityListResponse
    {
        public int Result { get; set; }
        public string Message { get; set; } = "";
        public List<CityModel> Data { get; set; } = new();

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class CitySearchRequest 
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }

    public class CityAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public CityModel City { get; set; } = new CityModel();
        public List<CountryModel> Country { get; set; } = new List<CountryModel>();
        public List<StateModel> State { get; set; } = new List<StateModel>();
        public CityModel? EditCity { get; set; } = new CityModel();
    }

    public class CityPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<CityModel> City { get; set; } = new List<CityModel>();
        public List<CountryModel> Country { get; set; } = new List<CountryModel>();
        public List<StateModel> State { get; set; } = new List<StateModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? SectionID { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
    }
}
