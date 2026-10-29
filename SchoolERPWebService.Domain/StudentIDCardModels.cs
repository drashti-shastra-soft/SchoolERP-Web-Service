using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class StudentIDCardViewModel
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
        public bool ShowAdmissionNo { get; set; }
        public bool ShowStudentName { get; set; }
        public bool ShowClass { get; set; }
        public bool ShowFatherName { get; set; }
        public bool ShowMotherName { get; set; }
        public bool ShowStudentAddress { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowDOB { get; set; }
        public bool ShowBloodGroup { get; set; }
        public int DesignType { get; set; }
        public bool ShowBarcode { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Helpers
        public string BackgroundImageBase64 => BackgroundImage != null ? $"data:{BackgroundImageType};base64,{Convert.ToBase64String(BackgroundImage)}" : null;
        public string LogoImageBase64 => LogoImage != null ? $"data:{LogoImageType};base64,{Convert.ToBase64String(LogoImage)}" : null;
        public string SignatureImageBase64 => SignatureImage != null ? $"data:{SignatureImageType};base64,{Convert.ToBase64String(SignatureImage)}" : null;
    }

    public class StudentIDCardUpsertRequest
    {
        public int IDCardID { get; set; }
        public string? IDCardTitle { get; set; }
        public string? SchoolName { get; set; }
        public string? HeaderColor { get; set; }
        public string? AddressPhoneEmail { get; set; }

        public byte[]? BackgroundImage { get; set; }
        public string? BackgroundImageType { get; set; }
        public string? BackgroundImageName { get; set; }

        public byte[]? LogoImage { get; set; }
        public string? LogoImageType { get; set; }
        public string? LogoImageName { get; set; }

        public byte[]? SignatureImage { get; set; }
        public string? SignatureImageType { get; set; }
        public string? SignatureImageName { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public IFormFile? BackgroundImageFile { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public IFormFile? LogoImageFile { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public IFormFile? SignatureImageFile { get; set; }

        public bool ShowAdmissionNo { get; set; }
        public bool ShowStudentName { get; set; }
        public bool ShowClass { get; set; }
        public bool ShowFatherName { get; set; }
        public bool ShowMotherName { get; set; }
        public bool ShowStudentAddress { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowDOB { get; set; }
        public bool ShowBloodGroup { get; set; }
        public int DesignType { get; set; }
        public bool ShowBarcode { get; set; }

        public bool IsActive { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class StudentIDCardPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<StudentIDCardViewModel> IDCards { get; set; } = new();
    }
}
