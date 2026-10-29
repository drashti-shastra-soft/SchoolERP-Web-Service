using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    /// <summary>
    /// Academic or Financial Session Year View Model (e.g. 2024-2025).
    /// </summary>
    public class MstSessionViewModel
    {
        public int SessionId { get; set; }
        public int CompanyID { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
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
    /// Payload structure for creating or modifying a Session term.
    /// </summary>
    public class MstSessionUpsertRequest
    {
        public int SessionId { get; set; }
        public int CompanyID { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// List wrapper for the initial view load of system Session periods.
    /// </summary>
    public class MstSessionPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstSessionViewModel> Sessions { get; set; } = new();
    }

    public class SetCurrentSessionRequest
    {
        public int SessionId { get; set; }
    }
}
