using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class PickupPointViewModel
    {
        public int PickupPointID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string PickupPointName { get; set; } = string.Empty;
        public decimal? PickupPointLatitude { get; set; }
        public decimal? PickupPointLongitude { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class PickupPointUpsertRequest
    {
        public int PickupPointID { get; set; }
        public string PickupPointName { get; set; } = string.Empty;
        public decimal? PickupPointLatitude { get; set; }
        public decimal? PickupPointLongitude { get; set; }
        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class PickupPointPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<PickupPointViewModel> Items { get; set; } = new();
    }
}
