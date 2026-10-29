using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class RequestHostelTypeModel 
    {

        public int HostelTypeID { get; set; }

        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string HostelTypeName { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int UserID { get; set; }
        public string IPAddress { get; set; }
        // Audit fields (returned by GET)
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class HostelTypeModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int HostelTypeID { get; set; }

        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string HostelTypeName { get; set; }
        public string DisplayLabel { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDelete { get; set; } = false;
        public int UserID { get; set; }
        public string IPAddress { get; set; }
        // Audit fields (returned by GET)
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ApiResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public string TechnicalMessage { get; set; }
    }

    public class HostelTypeSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }

    public class HostelTypePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<HostelTypeModel> HostelType { get; set; } = new List<HostelTypeModel>();
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

    public class HostelTypeAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public HostelTypeModel HostelType { get; set; } = new HostelTypeModel();
        public HostelTypeModel? EditHostelType { get; set; } = new HostelTypeModel();
    }

    public class HostelTypeUpsertRequest
    {
        public int HostelTypeID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }

        public string HostelTypeName { get; set; }
        public string DisplayLabel { get; set; }

        public string? Gender { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }
}
