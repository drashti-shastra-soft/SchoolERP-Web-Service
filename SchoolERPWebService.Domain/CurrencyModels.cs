using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    /// <summary>
    /// This class represents the data structure and schema for MstCurrencyViewModel.
    /// </summary>
    public class MstCurrencyViewModel
    {
        public int CurrencyId { get; set; }
        public string CurrencyTitle { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public decimal CurrencyConvRate { get; set; }
        public bool CurrencyBase { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    /// <summary>
    /// This class represents the data structure and schema for MstCurrencyUpsertRequest.
    /// </summary>
    public class MstCurrencyUpsertRequest
    {
        public int CurrencyId { get; set; }
        public string CurrencyTitle { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public decimal CurrencyConvRate { get; set; }
        public bool CurrencyBase { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// This class represents the data structure and schema for MstCurrencyPageViewModel.
    /// </summary>
    public class MstCurrencyPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstCurrencyViewModel> Currencies { get; set; } = new();
    }

    public class CurrencyDetailsModel
    {
        public int Result { get; set; }
        public string Message { get; set; }

        public int CompanyId { get; set; }
        public string SchoolName { get; set; }

        public int CurrencyId { get; set; }
        public string CurrencyTitle { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }

        public decimal CurrencyConvRate { get; set; }
        public bool CurrencyBase { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
