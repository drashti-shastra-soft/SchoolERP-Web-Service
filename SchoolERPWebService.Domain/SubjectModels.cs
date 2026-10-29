using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    public class MstSubjectViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int SubjectID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public string? SubjectType { get; set; }
        public string? SubjectCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public int? SubjectTypeID { get; set; }
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class MstSubjectUpsertRequest
    {
        public int SubjectID { get; set; }

        [Required(ErrorMessage = "Subject name is required")]
        [StringLength(200, ErrorMessage = "Subject name cannot exceed 200 characters")]
        public string SubjectName { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject type is required")]
        public string? SubjectType { get; set; }
        public string? SubjectCode { get; set; }
        public bool IsActive { get; set; } = true;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? SubjectTypeID { get; set; }
    }

    public class MstSubjectPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstSubjectViewModel> Subjects { get; set; } = new List<MstSubjectViewModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalPages => PageSize > 0
                             ? (int)Math.Ceiling((double)TotalRecords / PageSize)
                             : 0;

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }

    public class MstSubjectAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstSubjectViewModel Subjects { get; set; } = new MstSubjectViewModel();
        public MstSubjectViewModel? EditSubjects { get; set; } = new MstSubjectViewModel();
    }

    public class SubjectSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int? ClassID { get; set; }
        public int? SubjectID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }

    public class DropdowRequest 
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int UserId { get; set; }
    }

    public class Dropdowbinding
    {
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
    }
}
