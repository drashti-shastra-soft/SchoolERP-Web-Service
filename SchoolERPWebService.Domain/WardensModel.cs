using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class WardensModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int WardenId { get; set; }
        public int? HostelID { get; set; }
        public string? WardenName { get; set; }
        public string? HostelName { get; set; }
        public string? WardenContact { get; set; }
        public string? WardenEmail { get; set; }
        public string? Address { get; set; }
        public string? CountryName { get; set; }
        public string? StateName { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public string? PinCode { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Qualification { get; set; }
        public int? ExperienceYears { get; set; }
        public string? AadharNumber { get; set; }
        public string? Photo { get; set; }
        public string? EmergencyContactNumber { get; set; }
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

    public class WardensRequestModel
    {
        public int WardenId { get; set; }
        public int? HostelID { get; set; }
        public string? WardenName { get; set; }
        public string? WardenContact { get; set; }
        public string? WardenEmail { get; set; }
        public string? Address { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public string? PinCode { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Qualification { get; set; }
        public int? ExperienceYears { get; set; }
        public string? AadharNumber { get; set; }
        public string? Photo { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int UserID { get; set; }
        public string? IPAddress { get; set; }

        // ── transient photo payload — not persisted directly ──
        public string? PhotoBase64 { get; set; }         // e.g. "data:image/png;base64,iVBOR..."
        public string? PhotoFileName { get; set; }        // original filename, used only for extension
        public bool RemovePhoto { get; set; } = false;    // true = clear existing photo
    }

    public class WardensSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int? HostelID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }

    public class WardensPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<WardensModel> WardensModel { get; set; } = new List<WardensModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();
        public List<HostelViewModel> HostelModel { get; set; } = new List<HostelViewModel>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? SectionID { get; set; }
        public int? HostelID { get; set; }
    }

    public class WardensAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public WardensModel WardensModel { get; set; } = new WardensModel();
        public WardensModel? EditWardens { get; set; } = new WardensModel();
        public List<CountryModel> Country { get; set; } = new List<CountryModel>();
        public List<StateModel> State { get; set; } = new List<StateModel>();
        public List<HostelViewModel> HostelModel { get; set; } = new List<HostelViewModel>();
    }

    public class WardenProfileRequest
    {
        public int WardenId { get; set; }
        public string Photo { get; set; }
        public int UserId { get; set; }
    }

    public class WardenSaveResponse 
    {
        public int WardenId { get; set; }
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TechnicalMessage { get; set; } = string.Empty;
    }
}
