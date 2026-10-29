using System;
using System.Collections.Generic;

using System.Linq;

namespace SchoolERPWebService.Domain
{
    public class AccountEntryViewModel
    {
        public int AccountEntryID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int AccountHeadID { get; set; }
        public string? HeadName { get; set; }
        public string EntryType { get; set; } = string.Empty; // 'Income' / 'Expense'
        public string Name { get; set; } = string.Empty;
        public string? InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? AttachDoc { get; set; }
        public string? AttachDocType { get; set; }
        public string? AttachDocName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }
    public class AccountEntryAttachmentUpsertRequest
    {
        public int AccountEntryID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
    public class AccountEntryUpsertRequest
    {
        public int AccountEntryID { get; set; }
        public int AccountHeadID { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? InvoiceNo { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? AttachDoc { get; set; }
        public string? AttachDocType { get; set; }
        public string? AttachDocName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class AccountEntryPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<AccountEntryViewModel> Items { get; set; } = new();
        public List<AccountHeadViewModel> Heads { get; set; } = new();
        public string EntryType { get; set; } = string.Empty;
    }

    public class AccountEntryFormModel
    {
        public int AccountEntryID { get; set; }
        public int AccountHeadID { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? InvoiceNo { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        //public IFormFile? Attachment { get; set; }
        public string? Attachment { get; set; }
    }

    public class AccountEntryAddViewModel
    {
        public AccountEntryViewModel Entry { get; set; } = new();
        public List<AccountHeadViewModel> Heads { get; set; } = new();
        public string EntryType { get; set; } = string.Empty;
    }

    public class AccountEntrySearchRequest
    {
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public string EntryType { get; set; }          // e.g. "Expense", "Income"
        public string? SearchType { get; set; }        // optional
        public string? Searchkeyword { get; set; }        // optional
        public DateTime? DateFrom { get; set; }        // optional
        public DateTime? DateTo { get; set; }          // optional
        public int PageIndex { get; set; } = 1;        // 1-based page number
        public int PageSize { get; set; } = 10;
    }

    public class AccountEntrySearchViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<AccountEntryViewModel> Results { get; set; } = new();
        public string EntryType { get; set; } = string.Empty;
        public string SearchType { get; set; } = string.Empty;
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public decimal TotalAmount => Results.Sum(x => x.Amount);
    }
}
