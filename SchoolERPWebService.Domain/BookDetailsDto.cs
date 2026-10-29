using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class BookDetailsDto
    {
        public int BookID { get; set; }
        public int CompanyID { get; set; }
        public string? BookTitle { get; set; }
        public string? SubTitle { get; set; }
        public string? BookNo { get; set; }
        public string? ISBNNo { get; set; }
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

        public int? PublisherId { get; set; }
        public string? PublisherName { get; set; }

        public int? DocumentId { get; set; }
        public string? DocumentName { get; set; }

        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; }

        public int? SeriesId { get; set; }
        public string? SeriesName { get; set; }

        public string? AuthorNames { get; set; }

        // Raw JSON text as it comes back from FOR JSON PATH — Dapper maps this
        // straight to a string column, nothing special needed on the query side.
        public string? AccessionJson { get; set; }

        public int? IssuedQty { get; set; }
        public int? CurrentlyAvailableQty { get; set; }

        // Not mapped by Dapper directly — populated manually after deserializing
        // AccessionJson. [System.Text.Json.Serialization.JsonIgnore] not required
        // here since Dapper only binds columns that match by name; this property
        // simply has no matching column, so it's left alone.
        public List<AccessionDto> Accessions { get; set; } = new();
    }
    public class AccessionDto
    {
        public int AccessionId { get; set; }
        public string? AccessionNo { get; set; }
        public string? BillNo { get; set; }
        public DateTime? BillDate { get; set; }
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public int? Status { get; set; }        // from tbl_LibraryIssueReturn.Status, NULL = never issued
        public string? SupplierName { get; set; }
    }

    public class IssueHistoryDto
    {
        public int BookID { get; set; }
        public string? AccessionNo { get; set; }
        public int? StudentId { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int? Status { get; set; }
        // add any other tbl_LibraryIssueReturn columns you need here —
        // IR.* means Dapper will map every column that has a matching property.
    }

    public class BookDetailsResult
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public BookDetailsDto? Book { get; set; }
        public List<IssueHistoryDto> IssueHistory { get; set; } = new();
    }


    public class BulkMembershipRow
    {
        public int StudentID { get; set; }
        public int StaffID { get; set; }
        public string? AdmissionNo { get; set; }
        public string? MemberType { get; set; }
        public string? RollNo { get; set; }
        public bool? IsExistingMember { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? RenewDate { get; set; }
        public string? LibraryCardNo { get; set; }
        public int? NoOfDocuments { get; set; }
        public int? MaxDays { get; set; }
        public int? LibraryMemberID { get; set; }
    }

    public class BulkMembershipRequest
    {
        public string SessionYear { get; set; } = string.Empty;
        public int? DefaultNoOfDoc { get; set; }
        public int? DefaultMaxDays { get; set; }
        public List<BulkMembershipRow> Students { get; set; } = new();
    }

    // Matches the SP's first result set: RESULT, MESSAGE, INSERTEDCOUNT, SKIPPEDCOUNT
    public class SpBulkMemberResult
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public int? InsertedCount { get; set; }
        public int? SkippedCount { get; set; }
    }

    // Matches the SP's second result set (only returned when RESULT = 1)
    public class IssuedCard
    {
        public int LibraryMemberID { get; set; }
        public int StudentID { get; set; }
        public string LibraryCardNo { get; set; } = string.Empty;
    }

    public class LibraryMemberSearchResult
    {
        // Common - Library Member fields
        public int LibraryMemberID { get; set; }
        public string MemberType { get; set; }
        public string LibraryCardNo { get; set; }
        public string Photo { get; set; }
        public int? NOOFDOCUMENTS { get; set; }
        public int? MAXDAYS { get; set; }
        public DateTime? EXPIRYDATE { get; set; }
        public DateTime? RENEWDATE { get; set; }
        public bool? IsActive { get; set; }

        // Staff fields
        public int? StaffID { get; set; }
        public string StaffCode { get; set; }
        public string Staff_FirstName { get; set; }
        public string Staff_LastName { get; set; }
        public string Staff_Email { get; set; }
        public string Staff_MobileNo { get; set; }
        public int? DesignationID { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        // Student fields
        public int? StudentID { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNo { get; set; }
        public string Student_FirstName { get; set; }
        public string Student_LastName { get; set; }
        public string Student_Email { get; set; }
        public string Student_MobileNo { get; set; }
        public int? ClassID { get; set; }
        public string ClassName { get; set; }
        public int? SectionID { get; set; }
        public string SectionName { get; set; }

        // Extra fields returned only by the Staff-search / Student-search branches
        // (unaliased FirstName/LastName/Email/MobileNo/Gender/FatherName etc.)
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string Gender { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
    }
}
