using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class AccountHeadViewModel
    {
        public int AccountHeadID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string HeadName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public string? HeadDescription { get; set; }
        public string HeadType { get; set; } = string.Empty; // 'Income' or 'Expense'
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class AccountHeadUpsertRequest
    {
        public int AccountHeadID { get; set; }
        public string HeadName { get; set; } = string.Empty;
        public string? DisplayLabel { get; set; } = string.Empty;
        public string? HeadDescription { get; set; }
        public string HeadType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class AccountHeadPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;

        public List<AccountHeadViewModel> Items { get; set; } = new();
        public string HeadType { get; set; } = string.Empty; // To distinguish between Income/Expense in UI
    }

    public class AccountHeadSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public string HeadType { get; set; }
        public int? UserId { get; set; }
    }
}
