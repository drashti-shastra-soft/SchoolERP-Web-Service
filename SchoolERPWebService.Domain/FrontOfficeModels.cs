using SchoolERPWebService.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    // ─── PURPOSE ────────────────────────────────────────────────
    public class MstFOPurposeViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int PurposeID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class MstFOPurposeUpsertRequest
    {
        
        public int PurposeID { get; set; }

        [Required(ErrorMessage = "Purpose name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        public string? DisplayLabel { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    // ─── COMPLAINT TYPE ─────────────────────────────────────────
    public class MstFOComplaintTypeViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int ComplaintTypeID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class MstFOComplaintTypeUpsertRequest
    {
        public int ComplaintTypeID { get; set; }

        [Required(ErrorMessage = "Complaint Type name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        public string? DisplayLabel { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    // ─── SOURCE ─────────────────────────────────────────────────
    public class MstFOSourceViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int SourceID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class MstFOSourceUpsertRequest
    {
        public int SourceID { get; set; }

        [Required(ErrorMessage = "Source name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    // ─── REFERENCE ──────────────────────────────────────────────
    public class MstFOReferenceViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int ReferenceID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class MstFOReferenceUpsertRequest
    {
        public int ReferenceID { get; set; }

        [Required(ErrorMessage = "Reference name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        public string DisplayLabel { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    // ─── COMPLAINT ──────────────────────────────────────────────
    public class FOComplaintViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int ComplaintID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ComplaintTypeID { get; set; }
        public int SourceID { get; set; }
        
        public string ComplaintTypeName { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
        
        public string ComplaintBy { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime ComplaintDate { get; set; }
        
        public string? Description { get; set; }
        public string? ActionTaken { get; set; }
        public string? AssignedTo { get; set; }
        public string? Note { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ComplaintSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
        public int? ComplaintTypeID { get; set; }
        public int? SourceID { get; set; }
    }

    public class FOComplaintUpsertRequest
    {
        public int ComplaintID { get; set; }

        [Required(ErrorMessage = "Complaint Type is required")]
        public int ComplaintTypeID { get; set; }

        [Required(ErrorMessage = "Source is required")]
        public int SourceID { get; set; }

        [Required(ErrorMessage = "Complaint By is required")]
        [StringLength(150)]
        public string ComplaintBy { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Complaint Date is required")]
        public DateTime ComplaintDate { get; set; }

        public string? Description { get; set; }
        public string? ActionTaken { get; set; }
        
        [StringLength(100)]
        public string? AssignedTo { get; set; }
        
        public string? Note { get; set; }
        
        [StringLength(500)]
        public string? Attachment { get; set; }
        
        public bool IsActive { get; set; } = true;
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    // ─── POSTAL RECEIVE ────────────────────────────────────────
    public class FOPostalReceiveViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int PostalReceiveID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string FromTitle { get; set; } = string.Empty;
        public string? ToTitle { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class FOPostalReceiveUpsertRequest
    {
        public int PostalReceiveID { get; set; }

        [Required(ErrorMessage = "From Title is required")]
        [StringLength(200)]
        public string FromTitle { get; set; } = string.Empty;

        [StringLength(200)]
        public string? ToTitle { get; set; }

        [StringLength(100)]
        public string? ReferenceNo { get; set; }

        public string? Address { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public byte[]? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; } = true;

        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
    }

    public class UpsertPostalReceiveResponse
    {
        public SpPostalReceive Result { get; set; }
    }
    public class FOPostalReceiveAttachmentUpsertRequest
    {
        public int PostalReceiveID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
    // ─── POSTAL DISPATCH ───────────────────────────────────────
    public class FOPostalDispatchViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int PostalDispatchID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string ToTitle { get; set; } = string.Empty;
        public string? FromTitle { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class FOPostalDispatchUpsertRequest
    {
        public int PostalDispatchID { get; set; }

        [Required(ErrorMessage = "To Title is required")]
        [StringLength(200)]
        public string ToTitle { get; set; } = string.Empty;

        [StringLength(200)]
        public string? FromTitle { get; set; }

        [StringLength(100)]
        public string? ReferenceNo { get; set; }

        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        public string? Note { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        public byte[]? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; } = true;
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class FOPostalDispatchSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }


    public class FOPostalDispatchAttachmentUpsertRequest
    {
        public int PostalDispatchID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
    public class UpsertPostalDispatchResponse
    {
        public SpPostalDispatch Result { get; set; }
    }
    // ─── PHONE CALL LOG ───────────────────────────────────────
    public class FOPhoneCallLogViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int PhoneCallLogID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string? CallDuration { get; set; }
        public string? Note { get; set; }
        public string? CallType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class FOPhoneCallLogUpsertRequest
    {
        public int PhoneCallLogID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        public string? Description { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public string? CallDuration { get; set; }
        public string? Note { get; set; }
        public string? CallType { get; set; }
        public bool IsActive { get; set; } = true;
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class FOPhoneCallLogSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
        public string? CallType { get; set; }
    }

    // ─── VISITOR BOOK ──────────────────────────────────────────
    public class FOVisitorBookViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int VisitorBookID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int PurposeID { get; set; }
        public string PurposeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? IDCard { get; set; }
        public int NoOfPersons { get; set; }
        public DateTime Date { get; set; }
        public string? InTime { get; set; }
        public string? OutTime { get; set; }
        public string? Note { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public string? MeetingWith { get; set; }
        public int? StudentID { get; set; }
        public int? StaffID { get; set; }
        public string? StudentName { get; set; }
        public string? StaffName { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }


    public class FOVisitorBookSerchRequest 
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int? StudentID { get; set; }
        public int? StaffID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? Purposes { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class FOVisitorBookUpsertRequest
    {
        public int VisitorBookID { get; set; }

        [Required(ErrorMessage = "Purpose is required")]
        public int PurposeID { get; set; }

        [Required(ErrorMessage = "Visitor name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? IDCard { get; set; }

        public int NoOfPersons { get; set; } = 1;

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [StringLength(20)]
        public string? InTime { get; set; }

        [StringLength(20)]
        public string? OutTime { get; set; }

        public string? Note { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool? IsActive { get; set; }

        public string? MeetingWith { get; set; }
        public int? StudentID { get; set; }
        public int? StaffID { get; set; }

        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }


    public class FOVisitorBookAttachmentUpsertRequest
    {
        public int VisitorBookID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
    public class UpsertVisitorResponse
    {
        public SpVisitorResult Result { get; set; }
    }

    // ─── ADMISSION INQUIRY ──────────────────────────────────────
    public class FOAdmissionInquiryViewModel
    {
        public int InquiryID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }
        public DateTime Date { get; set; }
        public DateTime? NextFollowUpDate { get; set; }
        public int? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        public int? ReferenceID { get; set; }
        public string? ReferenceName { get; set; }
        public int? SourceID { get; set; }
        public string? SourceName { get; set; }
        public int? ClassID { get; set; }
        public string? ClassName { get; set; }
        public int NoOfChild { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastFollowUpDate { get; set; }

        public List<FOInquiryFollowUpViewModel> FollowUps { get; set; } = new();

        // Must exist — mapped from TOTALCOUNT column in SP
        public int Result { get; set; }   // ← TOTALCOUNT
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class FOAdmissionInquiryUpsertRequest
    {
        public int InquiryID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }

        public string? Description { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        public DateTime? NextFollowUpDate { get; set; }
        public int? AssignedTo { get; set; }
        public int? ReferenceID { get; set; }
        public int? SourceID { get; set; }
        public int? ClassID { get; set; }
        public int NoOfChild { get; set; } = 1;

        [StringLength(20)]
        public string Status { get; set; } = "Active";
        public bool IsActive { get; set; } = true;

        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class FOInquiryFollowUpViewModel
    {
        public int FollowUpID { get; set; }
        public int InquiryID { get; set; }
        public DateTime FollowUpDate { get; set; }
        public DateTime NextFollowUpDate { get; set; }
        public string Response { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class FOInquiryFollowUpSaveRequest
    {
        public int InquiryID { get; set; }

        [Required(ErrorMessage = "Follow Up Date is required")]
        public DateTime FollowUpDate { get; set; }

        [Required(ErrorMessage = "Next Follow Up Date is required")]
        public DateTime NextFollowUpDate { get; set; }

        [Required(ErrorMessage = "Response is required")]
        public string Response { get; set; } = string.Empty;

        public string? Note { get; set; }
    }

    // ─── PAGE VIEW MODEL ────────────────────────────────────────
    public class FrontOfficeSetupPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstFOPurposeViewModel> Purposes { get; set; } = new();
        public List<MstFOComplaintTypeViewModel> ComplaintTypes { get; set; } = new();
        public List<MstFOSourceViewModel> Sources { get; set; } = new();
        public List<MstFOReferenceViewModel> References { get; set; } = new();
    }

    // ─── PAGE VIEW MODEL ────────────────────────────────────────
    public class PurposesPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstFOPurposeViewModel> Purposes { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }
    public class PurposesAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstFOPurposeViewModel Purposes { get; set; } = new();
        public MstFOPurposeViewModel EditPurposes { get; set; } = new();
    }

    public class FOComplaintPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<FOComplaintViewModel> Complaints { get; set; } = new();
        public List<MstFOComplaintTypeViewModel> ComplaintTypes { get; set; } = new();
        public List<MstFOSourceViewModel> Sources { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? ComplaintTypeID { get; set; }
        public int? SourceID { get; set; }
    }

    public class FOComplaintAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public FOComplaintViewModel Complaints { get; set; } = new();
        public FOComplaintViewModel EditComplaints { get; set; } = new();
        public List<MstFOComplaintTypeViewModel> ComplaintTypes { get; set; } = new();
        public List<MstFOSourceViewModel> Sources { get; set; } = new();
    }

    public class FOPostalReceivePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<FOPostalReceiveViewModel> Items { get; set; } = new();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }


    public class FOPostalReceiveAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public FOPostalReceiveViewModel Items { get; set; } = new();
        public FOPostalReceiveViewModel EditItems { get; set; } = new();
    }

    public class FOPostalDispatchPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<FOPostalDispatchViewModel> Items { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }

    public class FOPostalDispatchAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public FOPostalDispatchViewModel Items { get; set; } = new();
        public FOPostalDispatchViewModel EditItems { get; set; } = new();
    }

    public class FOPhoneCallLogPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<FOPhoneCallLogViewModel> Items { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public string? CallType { get; set; }
    }

    public class FOPhoneCallLogAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public FOPhoneCallLogViewModel Items { get; set; } = new();
        public FOPhoneCallLogViewModel EditItems { get; set; } = new();
    }

    public class FOVisitorBookPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<FOVisitorBookViewModel> Visitors { get; set; } = new();
        public List<MstFOPurposeViewModel> Purposes { get; set; } = new();
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<HRStaffViewModel> Staff { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? StaffId { get; set; }
        public int? StudentId { get; set; }
    }

    public class FOAdmissionInquiryPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<FOAdmissionInquiryViewModel> Inquiries { get; set; } = new();
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<MstFOSourceViewModel> Sources { get; set; } = new();
        public List<MstFOReferenceViewModel> References { get; set; } = new();
        public List<HRStaffViewModel> Staff { get; set; } = new();
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

    public class AddAdmissionInquiryPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public FOAdmissionInquiryViewModel Inquiries { get; set; } = new();
        public FOAdmissionInquiryViewModel EditInquiries { get; set; } = new();
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<MstFOSourceViewModel> Sources { get; set; } = new();
        public List<MstFOReferenceViewModel> References { get; set; } = new();
        public List<HRStaffViewModel> Staff { get; set; } = new();
    }

    public class EnquirySearchRequest
    {
        public int? CompanyID { get; set; }

        public int? SessionID { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? SourceID { get; set; } = 0;

        public int? ClassID { get; set; } = 0;

        public string? Status { get; set; }

        public string? SearchKeyword { get; set; }

        public int? PageNumber { get; set; } = 1;

        public int? PageSize { get; set; } = 10;
    }

    public class AddVisitorBookPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public FOVisitorBookViewModel Visitors { get; set; } = new();
        public FOVisitorBookViewModel EditVisitors { get; set; } = new();
        public List<MstFOPurposeViewModel> Purposes { get; set; } = new();
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<HRStaffViewModel> Staff { get; set; } = new();
    }


    public class ComplaintTypePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstFOComplaintTypeViewModel> ComplaintType { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }
    public class ComplaintTypeAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstFOComplaintTypeViewModel ComplaintType { get; set; } = new();
        public MstFOComplaintTypeViewModel EditComplaintType { get; set; } = new();
    }

    public class SourcePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstFOSourceViewModel> Source { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }
    public class SourceAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstFOSourceViewModel Source { get; set; } = new();
        public MstFOSourceViewModel EditSource { get; set; } = new();
    }

    public class ReferencePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstFOReferenceViewModel> Reference { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }
    public class ReferenceAddPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstFOReferenceViewModel Reference { get; set; } = new();
        public MstFOReferenceViewModel EditReference { get; set; } = new();
    }

    public class FOComplaintAttachmentUpsertRequest
    {
        public int ComplaintID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }

    public class FOAdmissionInquiryStatusCountViewModel
    {
        public int Result { get; set; }
        public string STATUS { get; set; }
        public int STATUSCOUNT { get; set; }
        public string Message { get; set; }
    }

    public class InquiryStatusSummaryViewModel
    {
        public int NewCount { get; set; }
        public int FollowUpCount { get; set; }
        public int ConvertedCount { get; set; }
        public int LostCount { get; set; }
        public int TotalCount { get; set; }
    }
    public class InquiryStatusCountRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }


    public class PhoneCallLogChartModel
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int MonthNumber { get; set; }
        public string MonthName { get; set; }
        public int Completed { get; set; }
        public int Ongoing { get; set; }
        public int Rescheduled { get; set; }
    }

    public class PhoneCallLogChartResponse
    {
        public string[] Months { get; set; }
        public int[] Completed { get; set; }
        public int[] Ongoing { get; set; }
        public int[] Rescheduled { get; set; }
    }

    public class VisitorDashboardDto
    {
        public int VisitorBookID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string PurposeName { get; set; }
        public DateTime Date { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string MeetingWith { get; set; }
        public string StudentName { get; set; }
        public string StaffName { get; set; }
        public int NoOfPersons { get; set; }
        public string Note { get; set; }
    }
}
