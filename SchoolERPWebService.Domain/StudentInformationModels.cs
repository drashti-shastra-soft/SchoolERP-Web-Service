using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class StudentDisableReasonViewModel
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int DisableReasonID { get; set; }
        public int SessionID { get; set; }
        public int CompanyID { get; set; }
        public string DisableReasonTitle { get; set; } = "";
        public string DisplayLabel { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBY { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class StudentDisableReasonUpsertRequest
    {
        public int DisableReasonID { get; set; }
        public string DisableReasonTitle { get; set; } = "";
        public string DisplayLabel { get; set; } = "";
        public int SessionID { get; set; }
        public int CompanyID { get; set; }
    }

    public class StudentDisableReasonPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<StudentDisableReasonViewModel> Items { get; set; } = new();
    }

    public class StudentHouseViewModel
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int StudentHouseID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int CreateBy { get; set; }
        public string StudentHouseName { get; set; } = "";
        public string? StudentHouseDescription { get; set; }
        public string? DisplayLabel { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class StudentHouseUpsertRequest
    {
        public int StudentHouseID { get; set; }
        public string StudentHouseName { get; set; } = "";
        public string? StudentHouseDescription { get; set; }
        public string? DisplayLabel { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class StudentHousePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<StudentHouseViewModel> Items { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalPages => PageSize > 0
                             ? (int)Math.Ceiling((double)TotalRecords / PageSize)
                             : 0;

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
    }

    public class StudentCategoryViewModel
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int StudentCategoryID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int CreatedBY { get; set; }
        public string StudentCategoryName { get; set; } = "";
        public string DisplayLabel { get; set; } = "";
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class StudentCategorySearch
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }

    public class StudentCategoryUpsertRequest
    {
        public int StudentCategoryID { get; set; }
        public string StudentCategoryName { get; set; } = "";
        public string DisplayLabel { get; set; } = "";
        public int SessionID { get; set; }
        public int CompanyID { get; set; }
    }

    public class StudentCategoryPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<StudentCategoryViewModel> Items { get; set; } = new();
    }

    public class StudentAdmissionUpsertRequest
    {
        public int StudentID { get; set; }
        public string RollNo { get; set; } = "";
        public int SessionId { get; set; }
        public int CompanyID { get; set; }
        public int ParentUserID { get; set; }
        public Dictionary<string, string> FieldValues { get; set; } = new();
        public string? FeeTemplateId { get; set; }
    }

    public class StudentDocumentSaveModel
    {
        public int StudentID { get; set; }
        public string DocumentTitle { get; set; } = "";
        public string DocumentContent { get; set; } = "";
        public string DocumentPath { get; set; } = "";
        public string DocumentType { get; set; } = "";
    }

    

    public class StudentDetailsViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public StudentBasicInfoViewModel BasicInfo { get; set; } = new();
        public List<StudentAddressViewModel> Addresses { get; set; } = new();
        public StudentTransportDetailsViewModel? Transport { get; set; }
        public StudentHostelDetailsViewModel? Hostel { get; set; }
        public List<StudentCustomFieldValueViewModel> CustomFields { get; set; } = new();
        public List<StudentDocumentViewModel> Documents { get; set; } = new();
        public List<SiblingViewModel> Siblings { get; set; } = new();
        public List<FieldModel> StudentFields { get; set; } = new ();
        public List<StudentAddressDetailsModel> StudentAddress { get; set; } = new ();
        public List<StudentDocumentModel> StudentDocuments { get; set; } = new ();
        public StudentTransportDetailsModel studentTransportDetailsModel { get; set; } = new StudentTransportDetailsModel();
    }

    public class StudentBasicInfoViewModel
    {
        public int StudentID { get; set; }
        public string? RollNo { get; set; }
        public string? AdmissionNo { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {MiddleName} {LastName}".Trim().Replace("  ", " ");
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? CategoryName { get; set; }
        public string? Religion { get; set; }
        public string? Caste { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? BloodGroup { get; set; }
        public string? HouseName { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public string? ClassName { get; set; }
        public int ClassID { get; set; }
        public string? SectionName { get; set; }
        public int SectionID { get; set; }
        public int StudentCategoryID { get; set; }
        public int CategoryID { get; set; }
        public int? StudentHouseID { get; set; }
        public int? HouseID { get; set; }
        public string? StudentPhoto { get; set; }
        public string? StudentPhotoType { get; set; }
        
        // Parent Details
        public string? FatherName { get; set; }
        public string? FatherPhone { get; set; }
        public string? FatherOccupation { get; set; }
        public string? FatherPhoto { get; set; }
        public string? FatherPhotoType { get; set; }
        public string? MotherName { get; set; }
        public string? MotherPhone { get; set; }
        public string? MotherOccupation { get; set; }
        public string? MotherPhoto { get; set; }
        public string? MotherPhotoType { get; set; }
        public string? IfGuardianIs { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianPhone { get; set; }
        public string? GuardianOccupation { get; set; }
        public string? GuardianRelation { get; set; }
        public string? GuardianEmail { get; set; }
        public string? GuardianPhoto { get; set; }
        public string? GuardianPhotoType { get; set; }

        // Auth
        public string? StudentUsername { get; set; }
        public string? ParentUsername { get; set; }
        public string? StudentPassword { get; set; }
        public string? ParentPassword { get; set; }
        public int? ParentUserID { get; set; }
        public bool IsActive { get; set; } = true;
        public int? DisableReasonID { get; set; }
        public string? DisableReasonName { get; set; }
        public DateTime? DisableDate { get; set; }
        public string? DisableNote { get; set; }
        public int? StudentUserID { get; set; }
        public string? STUDENTHOUSENAME { get; set; }
        public string? STUDENTCATEGORYNAME { get; set; }
    }

    public class SiblingViewModel
    {
        public int StudentID { get; set; }
        public string? FullName { get; set; }
        public string? AdmissionNo { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string? RollNo { get; set; }
    }

    public class StudentAddressViewModel
    {
        public string? AddressType { get; set; }
        public string? AddressDetails { get; set; }
    }

    public class StudentTransportDetailsViewModel
    {
        public string? RouteName { get; set; }
        public int? RouteID { get; set; }
        public int? VehicleID { get; set; }
        public string? PickupPointName { get; set; }
        public int? PickupPointID { get; set; }
        public string? StartMonth { get; set; }
    }

    public class StudentHostelDetailsViewModel
    {
        public string? HostelName { get; set; }
        public int? HostelID { get; set; }
        public string? RoomTitle { get; set; }
        public int? RoomID { get; set; }
    }

    public class StudentCustomFieldValueViewModel
    {
        public int FieldID { get; set; }
        public string? FieldName { get; set; }
        public string? FieldValue { get; set; }
    }

    public class StudentDocumentViewModel
    {
        public int DocID { get; set; }
        public string? DocumentTitle { get; set; }
        public string? DocumentPath { get; set; }
        public byte[]? DocumentContent { get; set; }
    }

    public class StudentDropDwonBindViewModel
    {
        public int StudentID { get; set; }
        public string? RollNo { get; set; }
        public string? FullName { get; set; }
    }
    public class StudentDropDwonBindRequestModel
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SessionID { get; set; }
        public int CompanyID { get; set; }
        public int UserId { get; set; }
    }
    public class StudentListViewModel
    {
        public int StudentID { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public string? FullName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string? FatherName { get; set; }
        public string? GuardianName { get; set; }
        public string? FatherPhone { get; set; }
        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? CategoryName { get; set; }
        public string? MobileNo { get; set; }
        public string? StudentPhoto { get; set; }
        public string? StudentPhotoType { get; set; }
        public bool IsActive { get; set; }
        public string? DisableReasonName { get; set; }
        public DateTime? DisableDate { get; set; }
        public string? DisableNote { get; set; }
        public List<StudentCustomFieldValueViewModel> CustomFieldValues { get; set; } = new();
    }

    public class StudentListPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;


        public List<StudentListViewModel> Students { get; set; } = new();
        public int? SelectedClassId { get; set; }
        public int? SelectedSectionId { get; set; }
        public string? SearchTerm { get; set; }
        public string ViewType { get; set; } = "list";
        public List<FieldModel> SystemFields { get; set; } = new();
        public List<FieldModel> CustomFields { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class StudentTimelineViewModel
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int TimelineID { get; set; }
        public int StudentID { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime TimelineDate { get; set; }
        public string? Description { get; set; }
        public string? DocumentContent { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public bool IsVisibleToStudent { get; set; }
        public bool HasDocument { get; set; }
    }

    public class StudentTimelineUpsertRequest
    {
        public int TimelineID { get; set; }
        public int StudentID { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime TimelineDate { get; set; }
        public string? Description { get; set; }
        public string? DocumentBase64 { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public bool IsVisibleToStudent { get; set; } = true;
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class StudentStatusToggleRequest
    {
        public int StudentID { get; set; }
        public bool IsActive { get; set; }
        public int? DisableReasonID { get; set; }
        public DateTime? DisableDate { get; set; }
        public string? DisableNote { get; set; }
    }

    public class StudentMultiClassViewModel
    {
        public int MultiClassID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public string? ClassName { get; set; }
        public int SectionID { get; set; }
        public string? SectionName { get; set; }
    }

    public class MultiClassStudentCardViewModel
    {
        public int StudentID { get; set; }
        public string? RollNo { get; set; }
        public string? FullName { get; set; }
        public int PrimaryClassID { get; set; }
        public string? PrimaryClassName { get; set; }
        public int PrimarySectionID { get; set; }
        public string? PrimarySectionName { get; set; }
        public List<StudentMultiClassViewModel> AdditionalClasses { get; set; } = new();
    }

    public class StudentMultiClassUpsertRequest
    {
        public int MultiClassID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
    }

    public class StudentAttendanceHistoryViewModel
    {
        public List<StudentAttendanceSummary> Summaries { get; set; } = new();
        public List<StudentAttendanceDayStatus> Days { get; set; } = new();
    }

    public class StudentAttendanceSummary
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Present { get; set; }
        public int Late { get; set; }
        public int Absent { get; set; }
        public int HalfDay { get; set; }
        public int Holiday { get; set; }
        public int Leave { get; set; }
    }

    public class StudentAttendanceDayStatus
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public string Status { get; set; } = string.Empty; // P, L, A, F, H
    }

    public class StudentRollNoRequest
    {
        public Dictionary<string, string>? DynamicValues { get; set; }
        public int SessionId { get; set; }
        public int CompanyID { get; set; }
    }

    public class StudentCategoryAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public StudentCategoryViewModel Items { get; set; } = new();
        public StudentCategoryViewModel? EditStudentCategory { get; set; }
    }

    public class StudentHouseAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public StudentHouseViewModel Items { get; set; } = new();
        public StudentHouseViewModel? EditStudentHouse { get; set; }
    }

    public class StudentDisableReasonAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public StudentDisableReasonViewModel Items { get; set; } = new();
        public StudentDisableReasonViewModel? EditStudentDisableReason { get; set; }
    }
    public class StudentUserModel
    {
        public int StudentID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? PersonType { get; set; }
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public int UserTypeID { get; set; }
        public string? UserTypeCode { get; set; }
        public string? UserType { get; set; }
        public string? Password { get; set; }
        public bool UserIsActive { get; set; }
        public bool? UserIsLocked { get; set; }
    }
    public class MultiClassRowModel
    {
        public int MultiClassID { get; set; }   // 0 = new
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public bool IsDeleted { get; set; }      // true = row was removed client-side
    }

    public class MultiClassBulkSaveRequest
    {
        public int StudentID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public List<MultiClassRowModel> Rows { get; set; } = new();
    }

    public class StudentDisableViewModel
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int StudentID { get; set; }
        public int DisableReasonID { get; set; }
        public DateTime DisableDate { get; set; }
        public string DisableNote { get; set; }
    }

    public class StudentDisableReq
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int StudentID { get; set; }
        public int? DisableReasonID { get; set; }
        public DateTime? DisableDate { get; set; }
        public string? DisableNote { get; set; }
    }

    public class StudentTimelineAttachmentUpsertRequest
    {
        public int TimelineID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }

    public class StudentTimelineDocumentModel
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int TimelineId { get; set; }
        public string DocumentContent { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
    }

    public class StudentFeeTemplateResponse
    {
        public int MapStudentsFeeId { get; set; }
        public int CollectionId { get; set; }
        public int FeeTemplateId { get; set; }
        public int DetailId { get; set; }

        public string? FeesGroup { get; set; }
        public string? FeesCode { get; set; }

        public DateTime? DueDate { get; set; }

        public string? Status { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentId { get; set; }
        public string? PaymentMode { get; set; }
        public DateTime? PaymentDate { get; set; }

        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance { get; set; }
    }

    public class StudentFeeTemplateTotalResponse
    {
        public decimal TotalAmount { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalFine { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalBalance { get; set; }
    }
    public class StudentFeeTemplateListResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public string? TechnicalMessage { get; set; }
        public List<StudentFeeTemplateResponse> Data { get; set; }
            = new();

        public StudentFeeTemplateTotalResponse? Total { get; set; }
    }

    public class FeesSearchStudentResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }

        public string FeeTemplateId { get; set; }

        public int StudentID { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNo { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Gender { get; set; }

        public string MobileNo { get; set; }
        public string Email { get; set; }

        public int? ClassID { get; set; }
        public string Photo { get; set; }
        public string ClassName { get; set; }

        public int? SectionID { get; set; }
        public string SectionName { get; set; }

        // Error response fields
        public string TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }

    public class FeesSearchVoucherResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }


        public int StudentID { get; set; }
        public string VoucherNo { get; set; }
        public int VoucherHeaderID { get; set; }
        // Error response fields
        public string TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }
}
