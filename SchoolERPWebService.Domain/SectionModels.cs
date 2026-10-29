using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    public class MstSectionViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int SectionID { get; set; }
        // Dapper maps "5,6" here
    public string? SectionIds { get; set; }

    // ✅ Parse "5,6" → [5, 6] for Contains check
    public List<int> SectionIdList =>
        string.IsNullOrWhiteSpace(SectionIds)
            ? new List<int>()
            : SectionIds.Split(',')
                        .Where(x => int.TryParse(x.Trim(), out _))
                        .Select(x => int.Parse(x.Trim()))
                        .ToList();
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public MstSectionViewModel? EditSections { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class PaginationMeta 
    {
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class SesctionPageIndexViewModel 
    {
        public List<MstSectionViewModel> Sesction = new List<MstSectionViewModel>();
        public PaginationMeta PageIndex = new PaginationMeta();
    }


    public class MstSectionUpsertRequest
    {
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public int SectionID { get; set; }

        [Required(ErrorMessage = "Section name is required")]
        [StringLength(200, ErrorMessage = "Section name cannot exceed 200 characters")]
        public string SectionName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class MstSectionPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstSectionViewModel> Sections { get; set; } = new List<MstSectionViewModel>();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }

    // ── Result model for copy SP ─────────────────────────────────────────────
    public class CopyResult
    {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Inserted { get; set; }
        public int Skipped { get; set; }
    }

    public class SectionCopyRequest
    {
        public string SectionIds { get; set; } = string.Empty;
        public int TargetSessionId { get; set; }
    }
}
