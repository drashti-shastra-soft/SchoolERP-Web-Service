using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class StudentInformationReportModel
    {
       
        public int StudentId { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }

        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? MobileNo { get; set; }

        public string? StudentPhoto { get; set; }
        public string? StudentPhotoType { get; set; }

        public string? FatherName { get; set; }
        public string? FatherPhone { get; set; }

        public int? CategoryId { get; set; }
        public string? StudentCategoryName { get; set; }

        public int? ClassId { get; set; }
        public string? ClassName { get; set; }

        public int? SectionId { get; set; }
        public string? SectionName { get; set; }

        public string? RTE { get; set; }

        public string? MultiClassIds { get; set; }
        public string? MultiSectionIds { get; set; }
        public string? MultiClassNames { get; set; }
        public string? MultiSectionNames { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class StudentReportRequest
    {
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public int? StudentId { get; set; }
        public int? HostelID { get; set; }
        public int? DepartmentID { get; set; }
        public int? DesignationID { get; set; }
        public int? RoleID { get; set; }
        public string? UserType { get; set; }
        public string? SearchTerm { get; set; }
        public string? Source { get; set; }
        public string? Gender { get; set; }
        public int? CategoryId { get; set; }
        public string? RTE { get; set; }
        public string? SearchType { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // FIX: added so IsActive/status filter can round-trip through the AJAX
        // call and be reflected back on the view model. "both" = no filter.
        public string? Status { get; set; } = "both";

        public int? ItemID { get; set; }
        public int? ItemCategoryID { get; set; }
        public int? ItemStoreID { get; set; }
        public int? ItemSupplierID { get; set; }
        public string? AttendanceType { get; set; }


        public int? UserId { get; set; }
        public string? IpAddress { get; set; }
    }

    public class StudentCountReportResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public int ClassSectionID { get; set; }
        public int ClassID { get; set; }
        public string? ClassName { get; set; }

        public int SectionID { get; set; }
        public string? SectionName { get; set; }

        public int StudentCount { get; set; }

        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }

    public class GuardianReportResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public int StudentId { get; set; }
        public string? AdmissionNo { get; set; }
        public string? FullName { get; set; }
        public string? MobileNo { get; set; }

        public string? GuardianName { get; set; }
        public string? GuardianRelation { get; set; }
        public string? GuardianPhone { get; set; }

        public string? FatherName { get; set; }
        public string? FatherPhone { get; set; }

        public string? MotherName { get; set; }
        public string? MotherPhone { get; set; }

        public int? ClassId { get; set; }
        public string? ClassName { get; set; }

        public int? SectionId { get; set; }
        public string? SectionName { get; set; }

        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }

    public class LoginCredentialReportResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public int StudentId { get; set; }
        public string? AdmissionNo { get; set; }
        public string? FullName { get; set; }

        public string? UserType { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        public int? ClassId { get; set; }
        public string? ClassName { get; set; }

        public int? SectionId { get; set; }
        public string? SectionName { get; set; }

        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }

    public class AdmissionReportResponse
    {
        public int Result { get; set; }
        public string? Message { get; set; }

        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public int StudentId { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }

        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }

        public string? Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string? MobileNo { get; set; }

        public string? StudentPhoto { get; set; }
        public string? StudentPhotoType { get; set; }

        public string? FatherName { get; set; }
        public string? FatherPhone { get; set; }

        public DateTime? AdmissionDate { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? ClassId { get; set; }
        public string? ClassName { get; set; }

        public int? SectionId { get; set; }
        public string? SectionName { get; set; }

        public string? StudentCategoryName { get; set; }

        public string? MultiClassIds { get; set; }
        public string? MultiSectionIds { get; set; }
        public string? MultiClassNames { get; set; }
        public string? MultiSectionNames { get; set; }

        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }

    public class StudentGenderRatioReportResponse
    {
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        public int? TOTALRECORDS { get; set; }
        public int? TOTALPAGES { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? PAGESIZE { get; set; }

        public int? ClassID { get; set; }
        public string? CLASSNAME { get; set; }

        public int? SectionID { get; set; }
        public string? SECTIONNAME { get; set; }

        public int TOTALBOYS { get; set; }
        public int TOTALGIRLS { get; set; }
        public int TOTALSTUDENTS { get; set; }
        
        public int FOOTER_TOTALBOYS { get; set; }
        public int FOOTER_TOTALGIRLS { get; set; }
        public int FOOTER_TOTALSTUDENTS { get; set; }
        public string? FOOTER_BOYSGIRLSRATIO { get; set; }

        public string? BOYSGIRLSRATIO { get; set; }
    }

    public class TimeTableClassSubjectReportResponse
    {
        public int ClassID { get; set; }
        public string? ClassName { get; set; }

        public int SectionID { get; set; }
        public string? SectionName { get; set; }

        public int SubjectID { get; set; }
        public string? SubjectName { get; set; }

        public string? StaffID { get; set; }
        public string? StaffName { get; set; }

        public string? Time { get; set; }
        public string? RoomNo { get; set; }

        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        public int? TOTALRECORDS { get; set; }
        public int? TOTALPAGES { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int? PAGESIZE { get; set; }
    }

    public class StudentTeacherRatioReportResponse
    {
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        // Pagination
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        // Grand Totals
        public int GRANDTOTALSTUDENTS { get; set; }
        public int GRANDTOTALTEACHERS { get; set; }
        public decimal GRANDTOTALRATIO { get; set; }

        // Class / Section Details
        public int ClassID { get; set; }
        public string? ClassName { get; set; }

        public int SectionID { get; set; }
        public string? SectionName { get; set; }

        // Student / Teacher Ratio
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public decimal Ratio { get; set; }

        // Error Details
        public string? TECHNICALMESSAGE { get; set; }
        public int? ERRORLINE { get; set; }
    }

    public class StudentHostelReportResponse
    {
        // Common Response
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        // Pagination
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        // Class / Section
        public int ClassID { get; set; }
        public string? ClassName { get; set; }

        public int SectionID { get; set; }
        public string? SectionName { get; set; }

        // Student Details
        public int StudentID { get; set; }
        public string? AdmissionNo { get; set; }
        public string? StudentName { get; set; }
        public string? MobileNumber { get; set; }
        public string? GuardianPhone { get; set; }

        // Hostel Details
        public int HostelID { get; set; }
        public string? HostelName { get; set; }

        public int RoomID { get; set; }
        public string? RoomNumberName { get; set; }
        public string? RoomType { get; set; }
        public decimal CostPerBed { get; set; }

        // Error Details
        public string? TECHNICALMESSAGE { get; set; }
        public int? ERRORLINE { get; set; }
    }

    public class StaffReportResponse
    {
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        public int STAFFID { get; set; }
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
        public string? STAFFCODE { get; set; }

        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
        public string? EMAIL { get; set; }
        public string? MOBILENO { get; set; }

        public string? GENDER { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOJ { get; set; }

        public bool ISACTIVE { get; set; }
        public bool ISDELETE { get; set; }

        public int? CREATEDBY { get; set; }
        public DateTime? CREATEDON { get; set; }
        public int? MODIFIEDBY { get; set; }
        public DateTime? MODIFIEDON { get; set; }

        public string? DESIGNATIONNAME { get; set; }
        public string? DEPARTMENTNAME { get; set; }

        public string? USERNAME { get; set; }
        public int? USERTYPEID { get; set; }
        public int? USERID { get; set; }

        public string? ROLENAME { get; set; }

        // Pagination
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        // Error Response
        public string? TECHNICALMESSAGE { get; set; }
        public int? ERRORLINE { get; set; }
    }

    public class ItemStockReportResponse
    {
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        // Pagination
        public int TOTALRECORDS { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }
        public int TOTALPAGES { get; set; }

        // Item Details
        public int ITEMID { get; set; }
        public string? NAME { get; set; }

        // Category
        public int ITEMCATEGORYID { get; set; }
        public string? CATEGORY { get; set; }
        public string? CATEGORYDISPLAYLABEL { get; set; }

        // Store
        public int ITEMSTOREID { get; set; }
        public string? STORE { get; set; }

        // Supplier
        public int ITEMSUPPLIERID { get; set; }
        public string? SUPPLIER { get; set; }

        // Stock Details
        public string? UNIT { get; set; }
        public decimal TOTALQUANTITY { get; set; }
        public decimal TOTALISSUED { get; set; }
        public decimal AVAILABLEQUANTITY { get; set; }

        // Last Purchase
        public DateTime? LASTPURCHASEDATE { get; set; }
    }

    public class AddItemResponse
    {
        // Response
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        // Pagination
        public int TOTALRECORDS { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }
        public int TOTALPAGES { get; set; }

        // Item Stock
        public int ITEMSTOCKID { get; set; }
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
        public int ITEMID { get; set; }
        public string? ITEMNAME { get; set; }
        public string? UNIT { get; set; }

        // Category
        public int ITEMCATEGORYID { get; set; }
        public string? ITEMCATEGORY { get; set; }
        public string? CATEGORYDISPLAYLABEL { get; set; }

        // Store
        public int ITEMSTOREID { get; set; }
        public string? ITEMSTORENAME { get; set; }

        // Supplier
        public int ITEMSUPPLIERID { get; set; }
        public string? SUPPLIERNAME { get; set; }

        // Stock / Purchase
        public decimal QUANTITY { get; set; }
        public decimal PURCHASEPRICE { get; set; }
        public DateTime? PURCHASEDATE { get; set; }

        // Attachment
        public string? ATTACHMENT { get; set; }
        public string? FILENAME { get; set; }
        public string? FILETYPE { get; set; }
        public string? DESCRIPTION { get; set; }

        // Status
        public bool ISACTIVE { get; set; }
        public bool ISDELETE { get; set; }

        // Audit
        public int? CREATEDBY { get; set; }
        public DateTime? CREATEDON { get; set; }
        public int? MODIFIEDBY { get; set; }
        public DateTime? MODIFIEDON { get; set; }
    }

    public class ItemIssueResponse
    {
        // Response
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }

        // Pagination
        public int TOTALRECORDS { get; set; }
        public int? CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }
        public int TOTALPAGES { get; set; }

        // Item Issue
        public int ITEMISSUEID { get; set; }
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
        public int ITEMID { get; set; }
        public string? ITEMNAME { get; set; }
        public string? UNIT { get; set; }

        // Category
        public int ITEMCATEGORYID { get; set; }
        public string? ITEMCATEGORY { get; set; }
        public string? CATEGORYDISPLAYLABEL { get; set; }

        // Issue Details
        public string? NOTE { get; set; }
        public decimal QUANTITY { get; set; }
        public string? STATUS { get; set; }

        public DateTime? ISSUEDATE { get; set; }
        public DateTime? RETURNDATE { get; set; }
        public DateTime? EXPECTEDRETURNDATE { get; set; }
        public DateTime? ACTUALRETURNDATE { get; set; }

        public decimal? QUANTITYRETURNED { get; set; }

        // Issued To
        public int? ISSUETO { get; set; }
        public string? ISSUETONAME { get; set; }

        // Issued By
        public int? ISSUEBY { get; set; }
        public string? ISSUEBYNAME { get; set; }

        // Returned By
        public int? RETURNEDBY { get; set; }
        public string? RETURNEDBYNAME { get; set; }

        // Role
        public int? ROLEID { get; set; }

        // Status
        public bool ISACTIVE { get; set; }
        public bool ISDELETE { get; set; }

        // Audit
        public int? CREATEDBY { get; set; }
        public DateTime? CREATEDON { get; set; }
        public int? MODIFIEDBY { get; set; }
        public DateTime? MODIFIEDON { get; set; }
    }
}
