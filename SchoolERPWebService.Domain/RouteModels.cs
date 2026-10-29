using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class RouteViewModel
    {
        public int RouteID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class RouteUpsertRequest
    {
        public int RouteID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class RoutePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<RouteViewModel> Items { get; set; } = new();
    }
}
