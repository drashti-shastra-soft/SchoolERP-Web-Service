using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class StaffIDCardViewModel
    {
        public int IDCardID { get; set; }
        public string IDCardTitle { get; set; }
        public string SchoolName { get; set; }
        public string HeaderColor { get; set; }
        public string AddressPhoneEmail { get; set; }

        // Images
        public byte[] BackgroundImage { get; set; }
        public string BackgroundImageType { get; set; }
        public string BackgroundImageName { get; set; }

        public byte[] LogoImage { get; set; }
        public string LogoImageType { get; set; }
        public string LogoImageName { get; set; }

        public byte[] SignatureImage { get; set; }
        public string SignatureImageType { get; set; }
        public string SignatureImageName { get; set; }

        // Toggles
        public bool ShowStaffName { get; set; }
        public bool ShowDesignation { get; set; }
        public bool ShowStaffID { get; set; }
        public bool ShowDepartment { get; set; }
        public bool ShowDOJ { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowBloodGroup { get; set; }
        public bool ShowStaffAddress { get; set; }
        public int DesignType { get; set; }
        public bool ShowBarcode { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Helpers
        public string BackgroundImageBase64 => BackgroundImage != null ? $"data:{BackgroundImageType};base64,{Convert.ToBase64String(BackgroundImage)}" : null;
        public string LogoImageBase64 => LogoImage != null ? $"data:{LogoImageType};base64,{Convert.ToBase64String(LogoImage)}" : null;
        public string SignatureImageBase64 => SignatureImage != null ? $"data:{SignatureImageType};base64,{Convert.ToBase64String(SignatureImage)}" : null;
    }

    public class StaffIDCardUpsertRequest
    {
        public int IDCardID { get; set; }
        public string IDCardTitle { get; set; }
        public string SchoolName { get; set; }
        public string HeaderColor { get; set; }
        public string AddressPhoneEmail { get; set; }

        public byte[] BackgroundImage { get; set; }
        public string BackgroundImageType { get; set; }
        public string BackgroundImageName { get; set; }

        public byte[] LogoImage { get; set; }
        public string LogoImageType { get; set; }
        public string LogoImageName { get; set; }

        public byte[] SignatureImage { get; set; }
        public string SignatureImageType { get; set; }
        public string SignatureImageName { get; set; }

        public bool ShowStaffName { get; set; }
        public bool ShowDesignation { get; set; }
        public bool ShowStaffID { get; set; }
        public bool ShowDepartment { get; set; }
        public bool ShowDOJ { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowBloodGroup { get; set; }
        public bool ShowStaffAddress { get; set; }
        public int DesignType { get; set; }
        public bool ShowBarcode { get; set; }

        public bool IsActive { get; set; }
    }

    public class StaffIDCardPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<StaffIDCardViewModel> IDCards { get; set; } = new();
    }
}
