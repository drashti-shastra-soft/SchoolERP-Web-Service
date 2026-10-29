using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class PublisherModel
    {
        public int PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public string? DisplayLabel { get; set; }
        public string? Description { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
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

    public class PublisherRequest 
    {
        public int PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public string? DisplayLabel { get; set; }
        public string? Description { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    public class PublisherPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<PublisherModel> Publisher { get; set; } = new List<PublisherModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
    }

    public class PublisherddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public PublisherModel Publisher { get; set; } = new PublisherModel();
        public PublisherModel EditPublisher { get; set; } = new PublisherModel();
    }
}
