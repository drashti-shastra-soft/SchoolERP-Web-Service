using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SchoolERPWebService.Domain
{
    public class VehicleViewModel
    {
        public int VehicleID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string? VehicleModel { get; set; }
        public string? VehicleYearMade { get; set; }
        public string? VehicleRegNo { get; set; }
        public string? VehicleChasisNo { get; set; }
        public int? VehicleMaxCapicity { get; set; }
        public string? VehicleDriverName { get; set; }
        public string? VehicleDriverLicense { get; set; }
        public string? VehicleDriverContact { get; set; }
        public byte[]? VehicleDriverPhotoAttach { get; set; }
        public string? VehicleDriverPhotoName { get; set; }
        public string? VehicleDriverPhotoType { get; set; }
        public string? VehicleNote { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class VehicleUpsertRequest
    {
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string? VehicleModel { get; set; }
        public string? VehicleYearMade { get; set; }
        public string? VehicleRegNo { get; set; }
        public string? VehicleChasisNo { get; set; }
        public int? VehicleMaxCapicity { get; set; }
        public string? VehicleDriverName { get; set; }
        public string? VehicleDriverLicense { get; set; }
        public string? VehicleDriverContact { get; set; }
        public byte[]? VehicleDriverPhotoAttach { get; set; }
        public string? VehicleDriverPhotoName { get; set; }
        public string? VehicleDriverPhotoType { get; set; }
        public string? VehicleNote { get; set; }
        public bool IsActive { get; set; }
    }

    public class VehicleFormModel
    {
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string? VehicleModel { get; set; }
        public string? VehicleYearMade { get; set; }
        public string? VehicleRegNo { get; set; }
        public string? VehicleChasisNo { get; set; }
        public int? VehicleMaxCapicity { get; set; }
        public string? VehicleDriverName { get; set; }
        public string? VehicleDriverLicense { get; set; }
        public string? VehicleDriverContact { get; set; }
        public string? VehicleNote { get; set; }
        public bool IsActive { get; set; } = true;
        public IFormFile? DriverPhoto { get; set; }
        //public string? DriverPhoto { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class VehiclePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<VehicleViewModel> Items { get; set; } = new();
    }
}
