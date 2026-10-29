using SchoolERPWebService.Domain.Common;
using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class BookViewModel
    {
        // SP status columns
        public int Result { get; set; }
        public string? Message { get; set; }

        // Core book fields (TBL_LIBRARY_BOOKS)
        public int BookID { get; set; }
        public int? CompanyID { get; set; }
        public string BookTitle { get; set; }
        public string SubTitle { get; set; }
        public string? BookNo { get; set; }
        public string? PublisherName { get; set; }
        public string? DocumentName { get; set; }
        public string? AuthorNames { get; set; }
        public string? ISBNNo { get; set; }
        public int? PublisherId { get; set; }

        // CSV of author ids, e.g. "2,4" — aggregated in the SP from TBL_MST_MAP_LibraryAuthor
        public string? AuthorId { get; set; }

        public int? DocumentId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
        public int? LanguageId { get; set; }
        public int? SeriesId { get; set; }
        public string? Edition { get; set; }
        public string? ClassNo { get; set; }
        public string? Volume { get; set; }
        public int? NoOfPages { get; set; }
        public string? KeyWord { get; set; }
        public string? CoverPageImage { get; set; }
        public string? Remarks { get; set; }
        public string? RackNo { get; set; }
        public int? TotalQty { get; set; }
        public int? AvailableQty { get; set; }
        public decimal? BookPrice { get; set; }
        public DateTime? PostDate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        // Accessioning fields (TBL_MST_Library_Accessioning, joined in the SP)
        public int? Quantity { get; set; }
        public int? BudgetId { get; set; }
        public int? SupplierId { get; set; }
        public int? DocumentStatusId { get; set; }
        public string? BillNo { get; set; }
        public DateTime? BillDate { get; set; }
        public string? Location { get; set; }
        public decimal? Price { get; set; }
        public string? AccessionNo { get; set; }

        // Populated only when the SP's CATCH block fires
        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }

        // Temporary shim until FileName exists as a real column — see note above.
        // Remove this once the DB/SP actually returns FileName.
        public string? FileName =>
            string.IsNullOrEmpty(CoverPageImage) ? null : System.IO.Path.GetFileName(CoverPageImage);

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES

        
    }

    public class LibraryMemberViewModel
    {
        public int LibraryMemberID { get; set; }
        public int? StudentID { get; set; }
        public int? StaffID { get; set; }
        public string? LibraryCardNo { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public string? Name { get; set; }
        public string? MemberType { get; set; }
        public string? ClassName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? RenewDate { get; set; }
        public string? FatherName { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? MobileNo { get; set; }
        public DateTime? RegisteredOn { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
        public int? NoOfDaysOverdue { get; set; } 
        public int? MaxIssueDays { get; set; } 
    }

    public class LibraryMemberUpsertRequest
    {
        public string? MemberType { get; set; }
        public int? StudentID { get; set; }
        public int? StaffID { get; set; }
        public int? LibraryMemberID { get; set; }
        public string? LibraryCardNo { get; set; }
    }

    public class MembershipSearchViewModel
    {
        public int ID { get; set; }
        public string? AdmissionNo { get; set; }
        public string? Name { get; set; }
        public string? ExtraInfo { get; set; }
    }

    public class BookUpsertRequest
    {
        public int BookID { get; set; }
        public int? CompanyID { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? SubTitle { get; set; }
        public string? BookNo { get; set; }
        public string? ISBNNo { get; set; }
        public int? PublisherId { get; set; }
        public string? AuthorId { get; set; }
        public int? DocumentId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
        public int? LanguageId { get; set; }
        public int? SeriesId { get; set; }
        public string? Edition { get; set; }
        public string? ClassNo { get; set; }
        public string? Volume { get; set; }
        public int? NoOfPages { get; set; }
        public string? KeyWord { get; set; }
        public string? CoverPageImage { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public string? Remarks { get; set; }
        public string? RackNo { get; set; }
        public int? TotalQty { get; set; }
        public int? AvailableQty { get; set; }
        public decimal? BookPrice { get; set; }
        public DateTime? PostDate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? IPAddress { get; set; }
        public string? Mode { get; set; }

        //Document Accessioning Details 
        public int BudgetId { get; set; }
        public int Quantity { get; set; }
        public int? SupplierId { get; set; }
        public int? DocumentStatusId { get; set; }
        public string? BillNo { get; set; }
        public DateTime? BillDate { get; set; }
        public string? Location { get; set; }
        public decimal? Price { get; set; }
    }


    public class UpsertBookResponse
    {
        public SpBooksResult Result { get; set; }
    }

    public class IssueReturnViewModel
    {
        public int IssueReturnID { get; set; }
        public string? BookTitle { get; set; }
        public string? AccessionNo { get; set; }
        public string? BookNo { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueReturnDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int Status { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class IssueReturnUpsertRequest
    {
        public int? IssueReturnID { get; set; }
        public int LibraryMemberID { get; set; }
        public int BookID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueReturnDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class MemberDetailsViewModel
    {
        public int LibraryMemberID { get; set; }
        public string? LibraryCardNo { get; set; }
        public string? MemberType { get; set; }
        public string? AdmissionNo { get; set; }
        public string? MemberName { get; set; }
        public string? Gender { get; set; }
        public string? MobileNo { get; set; }
        public string? StudentPhoto { get; set; }
        public string? StudentFullName { get; set; }
        public string? StudentEmail { get; set; }

        public string? StaffPhoto { get; set; }
        public string? StaffFullName { get; set; }
        public string? StaffEmail { get; set; }
    }

    public class LibraryMember
    {
        public int LibraryMemberID { get; set; }
        public string MemberType { get; set; }      // "Student" or "Staff"
        public int? StudentID { get; set; }          // nullable - only set when MemberType = Student
        public int? StaffID { get; set; }            // nullable - only set when MemberType = Staff
        public string LibraryCardNo { get; set; }
        public int CompanyID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class LibrarySearchRequest
    {
        public int? CompanyID { get; set; }
        public int? BookId { get; set; }
        public string? SearchText { get; set; }
        public int? PublisherID { get; set; }
        public int? AuthorID { get; set; }
        public int? SubjectID { get; set; }
        public int? CategoryID { get; set; }
        public int? LanguageID { get; set; }
        public int? SeriesID { get; set; }
        /// <summary>Document Type ID</summary>
        public int? DocumentID { get; set; }
        public bool? IsActive { get; set; }
        public string SortColumn { get; set; } = "BookID";
        public string SortDir { get; set; } = "DESC";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class StudentsMembershipSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class StaffLibraryMemberSearchModel
    {
        public int CompanyID { get; set; }
        public int? DepartmentID { get; set; }
        public string? SearchTerm { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class StaffLibraryMember
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        // Must exist — mapped from TOTALRECORDS column in SP
        public int TOTALRECORDS { get; set; }
        public int CURRENTPAGE { get; set; }   // ← CURRENTPAGE
        public int PageSize { get; set; }      // ← PAGESIZE
        public int TotalPages { get; set; }    // ← TOTALPAGES

        public int StaffID { get; set; }
        public string AdmissionNo { get; set; }
        public string StaffName { get; set; }
        public string DepartmentName { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public int LibraryMemberID { get; set; }
        public string LibraryCardNo { get; set; }
        public DateTime? RegisteredOn { get; set; }
        public bool? IsActive { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public DateTime? RenewDate { get; set; }
        public int? NoOfDaysOverdue { get; set; }
        public int? MaxIssueDays { get; set; }
    }

    public class StaffMemberPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<HRDepartmentViewModel> Departments { get; set; } = new();
        public List<StaffLibraryMember> Items { get; set; } = new();
        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public string? SessionYear { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? DepartmentID { get; set; }
    }

    public class IssuePageViewModel 
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MemberDetailsViewModel MemberDetails { get; set; } = new();
        public List<IssueReturnViewModel> IssueReturn { get; set; } = new();
        
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? LibraryMemberID { get; set; }
    }
    public class IssueBookAddViewModel 
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<BookViewModel> BookViewModels { get; set; }
        public List<IssueReturnViewModel> BookReturnViewModels { get; set; }
        public BookViewModel EditIssueBooks { get; set; }
        public int? LibraryMemberID { get; set; }
        public int? IssueReturnID { get; set; }
        public List<DropdownModel> DocumentStatus { get; set; } = new();
        public int IssueNo { get; set; }

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
    }
    public class IssueBookSearchModel
    {
        public int LibraryMemberID { get; set; }
        public int CompanyID { get; set; }
        public string? SearchkeyWord{ get; set; }
        public string? AccessionNo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
    public class MemberSearchModel
    {
        public int CompanyID { get; set; }
        public string? SearchkeyWord { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
    public class ReturnBookIssue
    {
        public int issueId { get; set; }
        public DateTime returnDate { get; set; }
    }

    public class MemberPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<LibraryMemberViewModel> Items { get; set; } = new();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
    }

    public class BooksAttachmentUpsertRequest
    {
        public int BookId { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public bool? RemoveAttachment { get; set; }
    }

    public class AccessionDetailsModel
    {
        // Accessioning fields (TBL_MST_Library_Accessioning, joined in the SP)
        public int? AccessionId { get; set; }
        public int? BookID { get; set; }
        public string? BookTitle { get; set; }
        public string? SubTitle { get; set; }
        public int? Quantity { get; set; }
        public int? BudgetId { get; set; }
        public int? SupplierId { get; set; }
        public int? DocumentStatusId { get; set; }
        public string? StatusName { get; set; }
        public string? BillNo { get; set; }
        public DateTime? BillDate { get; set; }
        public string? Location { get; set; }
        public decimal? Price { get; set; }
        public string? AccessionNo { get; set; }
        public bool? IsAvailable { get; set; }
        public string? DocumentStatus { get; set; }
        public string? Author { get; set; }
        public int? LibraryMemberID { get; set; }
        public int? StaffID { get; set; }
        public int? StudentID { get; set; }
        public string? LibraryCardNo { get; set; }
        public string? MemberType { get; set; }

        // SP status columns
        public int Result { get; set; }
        public string? Message { get; set; }
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class AccessionPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<AccessionDetailsModel> Items { get; set; } = new();
        public List<BookViewModel> ItemsBooks { get; set; } = new();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? BookId { get; set; }
    }
    //BarCodeLabels
    public class BarCodeLabelsPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<AccessionDetailsModel> Items { get; set; } = new();

        public string? Mode { get; set; }
        public int? CompanyId { get; set; }
        public string? FromAccessionNo { get; set; }
        public string? ToAccessionNo { get; set; }
        public string? AccessionNo { get; set; }
    }
    public class AccessionAddViewModel 
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public AccessionDetailsModel Items { get; set; } = new();
        public List<DropdownModel> DocumentStatus { get; set; } = new();
    }

    public class AccessionStatusUpsertRequest
    {
        public int AccessionId { get; set; }
        public int? CompanyID { get; set; }
        public int? DocumentStatusId { get; set; }
    }

    public class AccessionSearchRequest
    {
        public string? Mode { get; set; }
        public int? CompanyId { get; set; }
        public string? FromAccessionNo { get; set; }
        public string? ToAccessionNo { get; set; }
        public string? AccessionNo { get; set; }
    }

    public class IssueNoResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int IssueNo { get; set; }
    }

    public class LibraryIssueSaveRequest
    {
        public int LibraryMemberID { get; set; }
        public DateTime? IssueDate { get; set; }
        public int IssueNo { get; set; }
        public int CompanyID { get; set; }
        public string? IssueListJson { get; set; }
    }

    public class LibraryReturnRequest
    {
        public int CompanyID { get; set; }
        public string ReturnListJson { get; set; }
    }
}
