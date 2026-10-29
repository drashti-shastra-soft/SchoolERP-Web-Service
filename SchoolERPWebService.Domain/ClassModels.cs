using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    public class MstClassViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int ClassID { get; set; }
        public int CompanyID { get; set; }
        
        public int SessionID { get; set; }

        public string ClassName { get; set; } = string.Empty;
        
        public string SectionNames { get; set; } = string.Empty; // From STRING_AGG in SP
        public int SequenceOrder { get; set; } = 0; // From STRING_AGG in SP
        public string DisplayLabel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        //public List<int> SectionIds { get; set; } = new List<int>();
        // ✅ MUST be string — Dapper maps "5,6" as plain string
        public string? SectionIds { get; set; }

        // ✅ Computed — Dapper never touches this
        public List<int> SectionIdList =>
            string.IsNullOrWhiteSpace(SectionIds)
                ? new List<int>()
                : SectionIds.Split(',')
                            .Where(x => int.TryParse(x.Trim(), out _))
                            .Select(x => int.Parse(x.Trim()))
                            .ToList();

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class MstClassUpsertRequest
    {
        public int ClassID { get; set; }

        [Required(ErrorMessage = "Class name is required")]
        [StringLength(200, ErrorMessage = "Class name cannot exceed 200 characters")]
        public string ClassName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public List<int> SectionIds { get; set; } = new List<int>();
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? SequenceOrder { get; set; }
    }

    public class MstClassPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;        
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public List<MstSectionViewModel> AvailableSections { get; set; } = new List<MstSectionViewModel>();
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

    public class MstClassAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstClassViewModel Classes { get; set; } = new MstClassViewModel();
        public MstClassViewModel? EditClasses { get; set; } = new MstClassViewModel();
        public List<MstSectionViewModel> AvailableSections { get; set; } = new List<MstSectionViewModel>();
    }

    public class ClassSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
        public int? SectionID { get; set; }
        public string? ClassID { get; set; }
    }
}
