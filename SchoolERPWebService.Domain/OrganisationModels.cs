using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    /// <summary>
    /// Represents the full data payload for an Organisation or branch.
    /// Maps directly to the tbl_mst_organisation table containing global institutional settings.
    /// </summary>
    public class OrganisationViewModel
    {
        public int OrganisationID { get; set; }
        public int? ParentOrganisationID { get; set; }
        public string? ParentName { get; set; }
        public string OrganisationName { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;
        public int? FromCode { get; set; }
        public int? ToCode { get; set; }
        public string? FinancialYear { get; set; }
        public int? PreviousFinancialYear { get; set; }
        public string? CollegeCode { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Mobile { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Fax { get; set; }
        public string? AffiliationNo { get; set; }
        public string? BoardName { get; set; }
        public DateTime? SchoolStartDate { get; set; }
        public DateTime? RenewalUptoDate { get; set; }
        public string? SenderID { get; set; }
        public string? SMSApiKey { get; set; }
        public string? SMSLabel { get; set; }
        public string? UploadURL { get; set; }
        public int? SessionID { get; set; }
        public decimal? LateFinePerDay { get; set; }
        public decimal? LibraryFinePerDay { get; set; }
        public decimal? LibraryFineMaxPerBook { get; set; }
        public decimal? FormSalePrice { get; set; }
        public int? PunchBeforeMinute { get; set; }
        public int? PunchAfterMinute { get; set; }
        public bool IsFeePayAllowed { get; set; }
        public bool IsChequeAllowed { get; set; }
        public bool IsFinalApprovalByHOD { get; set; }
        public bool IsReceiptSearchAll { get; set; }
        public bool IsScholarshipEnabled { get; set; }
        public bool IsAttendanceOnline { get; set; }
        public bool IsGroupEnabled { get; set; }
        public bool IsLeaveApplyBackDays { get; set; }
        public bool IsPartialFeeAllowed { get; set; }
        public bool IsCourseSemesterTemplate { get; set; }
        public bool IsLeaveDefaultApproved { get; set; }
        public bool IsFeeTemplateEnabled { get; set; }
        public bool IsMultiReceiptCopy { get; set; }
        public bool IsMultiLanguage { get; set; }
        public bool IsSubstituteLeaveApproval { get; set; }
        public bool IsHostelFeeTemplate { get; set; }
        public bool IsAllowSessionChange { get; set; }
        public bool IsOTPLogin { get; set; }
        public bool IsCopyrightActive { get; set; }
        public string? EnquiryMobileNo { get; set; }
        public string? EnquiryEmail { get; set; }
        public string? EnquiryWebsite { get; set; }
        public string? CopyrightText { get; set; }
        public string? RunBy { get; set; }
        public int? PortalUserTypeID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Form submission payload for creating or amending an Organisation's profile and global toggle switches.
    /// </summary>
    public class OrganisationUpsertRequest
    {
        public int OrganisationID { get; set; }
        public int? ParentOrganisationID { get; set; }

        [Required]
        [StringLength(300)]
        public string OrganisationName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string CompanyCode { get; set; } = string.Empty;

        public int? FromCode { get; set; }
        public int? ToCode { get; set; }
        public string? FinancialYear { get; set; }
        public int? PreviousFinancialYear { get; set; }
        public string? CollegeCode { get; set; }
        public string? Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Mobile { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Fax { get; set; }
        public string? AffiliationNo { get; set; }
        public string? BoardName { get; set; }
        public DateTime? SchoolStartDate { get; set; }
        public DateTime? RenewalUptoDate { get; set; }
        public string? SenderID { get; set; }
        public string? SMSApiKey { get; set; }
        public string? SMSLabel { get; set; }
        public string? UploadURL { get; set; }
        public int? SessionID { get; set; }
        public decimal? LateFinePerDay { get; set; }
        public decimal? LibraryFinePerDay { get; set; }
        public decimal? LibraryFineMaxPerBook { get; set; }
        public decimal? FormSalePrice { get; set; }
        public int? PunchBeforeMinute { get; set; }
        public int? PunchAfterMinute { get; set; }
        public bool IsFeePayAllowed { get; set; }
        public bool IsChequeAllowed { get; set; }
        public bool IsFinalApprovalByHOD { get; set; }
        public bool IsReceiptSearchAll { get; set; }
        public bool IsScholarshipEnabled { get; set; }
        public bool IsAttendanceOnline { get; set; }
        public bool IsGroupEnabled { get; set; }
        public bool IsLeaveApplyBackDays { get; set; }
        public bool IsPartialFeeAllowed { get; set; }
        public bool IsCourseSemesterTemplate { get; set; }
        public bool IsLeaveDefaultApproved { get; set; }
        public bool IsFeeTemplateEnabled { get; set; }
        public bool IsMultiReceiptCopy { get; set; }
        public bool IsMultiLanguage { get; set; }
        public bool IsSubstituteLeaveApproval { get; set; }
        public bool IsHostelFeeTemplate { get; set; }
        public bool IsAllowSessionChange { get; set; }
        public bool IsOTPLogin { get; set; }
        public bool IsCopyrightActive { get; set; }
        public string? EnquiryMobileNo { get; set; }
        public string? EnquiryEmail { get; set; }
        public string? EnquiryWebsite { get; set; }
        public string? CopyrightText { get; set; }
        public string? RunBy { get; set; }
        public int? PortalUserTypeID { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Page wrapper model providing the list of active/inactive organisations for the master view.
    /// </summary>
    public class OrganisationPageViewModel
    {
        public List<OrganisationViewModel> Organisations { get; set; } = new List<OrganisationViewModel>();
    }
}
