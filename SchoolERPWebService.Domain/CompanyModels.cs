using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class MstCompanyViewModel
    {
        public int CompanyId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string SchoolCode { get; set; }
        public int? ParentCompanyId { get; set; }
        public string? ParentName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int SessionId { get; set; }
        public string? SessionTitle { get; set; }
        public int CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? SessionStartMonth { get; set; }
        public string? DateFormat { get; set; }
        public string? Timezone { get; set; }
        public string? StartDayOfWeek { get; set; }
        public string? CurrencyFormat { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int Level { get; set; }
        public bool HasChildren { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TotalRecords { get; set; }   // ← TOTALCOUNT
        public int PageNumber { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
        
        public string? PrintLogo { get; set; }   
        public string? AdminLogo { get; set; }   
        public string? AdminSmallLogo { get; set; }   
        public string? WaterMarkLogo { get; set; }   
        public string? BackgroundLogo { get; set; }   
        
        public string? PrintLogoName { get; set; }   
        public string? AdminLogoName { get; set; }   
        public string? AdminSmallLogoName { get; set; }   
        public string? WaterMarkLogoName { get; set; }   
    }

    public class MstCompanyUpsertRequest
    {
        public int CompanyId { get; set; }
        public string SchoolName { get; set; } = string.Empty;
        public string SchoolCode { get; set; }
        public int? ParentCompanyId { get; set; }
        public string? Address { get; set; }
        public string? HeadOfficeAddress { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int SessionId { get; set; }
        public int CurrencyId { get; set; }
        public string? SessionStartMonth { get; set; }
        public string? DateFormat { get; set; }
        public string? Timezone { get; set; }
        public string? StartDayOfWeek { get; set; }
        public string? CurrencyFormat { get; set; }
        public bool IsActive { get; set; }

        public string? AdmissionNo { get; set; }
        public string? BoardName { get; set; }
        public string? SchoolStatus { get; set; }
        public string? LateFinePerDay { get; set; }
        public string? LibraryFinePerDay { get; set; }
        public string? LateFineMaxPerBook { get; set; }

        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
    }

    public class MstCompanyPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstCompanyViewModel> ParentCompanies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();
        public List<MstCurrencyViewModel> Currencies { get; set; } = new();
    }

    public class SetCurrentCompanyRequest
    {
        public int CompanyId { get; set; }
    }

    public class CurrentCompanyResult
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int? CompanyId { get; set; }
    }

    public class CompanyLogoSaveModel
    {
        public int CompanyID { get; set; }
        public int? UserID { get; set; }

        public string? PrintLogo { get; set; }
        public string? PrintLogoType { get; set; }
        public string? PrintLogoName { get; set; }

        public string? AdminLogo { get; set; }
        public string? AdminLogoType { get; set; }
        public string? AdminLogoName { get; set; }

        public string? AdminSmallLogo { get; set; }
        public string? AdminSmallLogoType { get; set; }
        public string? AdminSmallLogoName { get; set; }

        public string? WaterMarkLogo { get; set; }
        public string? WaterMarkLogoType { get; set; }
        public string? WaterMarkLogoName { get; set; }

        public string? BackgroundLogo { get; set; }
        public string? BackgroundLogoType { get; set; }
        public string? BackgroundLogoName { get; set; }
    }

    public class CompanyLogoRequest
    {
        public int CompanyID { get; set; }

        public int? UserID { get; set; }

        public string? PrintLogo { get; set; }
        public string? PrintLogoType { get; set; }
        public string? PrintLogoName { get; set; }

        public string? AdminLogo { get; set; }
        public string? AdminLogoType { get; set; }
        public string? AdminLogoName { get; set; }

        public string? AdminSmallLogo { get; set; }
        public string? AdminSmallLogoType { get; set; }
        public string? AdminSmallLogoName { get; set; }

        public string? WaterMarkLogo { get; set; }
        public string? WaterMarkLogoType { get; set; }
        public string? WaterMarkLogoName { get; set; }

        public string? BackgroundLogo { get; set; }
        public string? BackgroundLogoType { get; set; }
        public string? BackgroundLogoName { get; set; }
    }

    public class CompanyLogoResponse
    {
        public int CompanyLogoID { get; set; }

        public int CompanyID { get; set; }

        public string? PrintLogoUrl { get; set; }

        public string? AdminLogoUrl { get; set; }

        public string? AdminSmallLogoUrl { get; set; }

        public string? WaterMarkUrl { get; set; }

        public string? LoginBackgroundUrl { get; set; }
    }
}
