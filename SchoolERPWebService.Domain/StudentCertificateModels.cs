using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class StudentCertificateViewModel
    {
        public int CertificateID { get; set; }
        public string CertificateName { get; set; }
        public string HeaderLeftText { get; set; }
        public string HeaderCenterText { get; set; }
        public string HeaderRightText { get; set; }
        public string BodyText { get; set; }
        public string FooterLeftText { get; set; }
        public string FooterCenterText { get; set; }
        public string FooterRightText { get; set; }
        public decimal? HeaderHeight { get; set; }
        public decimal? FooterHeight { get; set; }
        public decimal? BodyHeight { get; set; }
        public decimal? BodyWidth { get; set; }
        public bool EnableStudentPhoto { get; set; }
        public byte[] BackgroundImage { get; set; }
        public string BackgroundImageType { get; set; }
        public string BackgroundImageName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Helper to display image in UI if needed
        public string BackgroundImageBase64 => BackgroundImage != null ? $"data:{BackgroundImageType};base64,{Convert.ToBase64String(BackgroundImage)}" : null;
    }

    public class StudentCertificateUpsertRequest
    {
        public int CertificateID { get; set; }
        public string CertificateName { get; set; }
        public string? HeaderLeftText { get; set; }
        public string? HeaderCenterText { get; set; }
        public string? HeaderRightText { get; set; }
        public string BodyText { get; set; }
        public string? FooterLeftText { get; set; }
        public string? FooterCenterText { get; set; }
        public string? FooterRightText { get; set; }
        public decimal? HeaderHeight { get; set; }
        public decimal? FooterHeight { get; set; }
        public decimal? BodyHeight { get; set; }
        public decimal? BodyWidth { get; set; }
        public bool EnableStudentPhoto { get; set; }
        public byte[]? BackgroundImage { get; set; }
        public string? BackgroundImageType { get; set; }
        public string? BackgroundImageName { get; set; }
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public bool IsActive { get; set; }
    }

    public class StudentCertificatePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<StudentCertificateViewModel> Certificates { get; set; } = new();
    }
}
