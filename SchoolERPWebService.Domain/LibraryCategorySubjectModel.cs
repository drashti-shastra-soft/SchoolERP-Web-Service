using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class LibraryCategorySubjectModel
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? CategoryName { get;set; }
        public string? Subjectname { get;set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class LibraryCategorySubjectRequest
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    public class LibraryCategorySubjectPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<LibraryCategorySubjectModel> CategorySubject { get; set; } = new List<LibraryCategorySubjectModel>();
        public List<LibraryCategoryModel> Category { get; set; } = new List<LibraryCategoryModel>();
        public List<LibrarySubjectModel> Subject { get; set; } = new List<LibrarySubjectModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
    }

    public class LibraryCategorySubjectAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public LibraryCategorySubjectModel CategorySubject { get; set; } = new LibraryCategorySubjectModel();
        public LibraryCategorySubjectModel EditCategorySubject { get; set; } = new LibraryCategorySubjectModel();
        public List<LibraryCategoryModel> Category { get; set; } = new List<LibraryCategoryModel>();
        public List<LibrarySubjectModel> Subject { get; set; } = new List<LibrarySubjectModel>();
    }

    public class LibraryCategorySubjectSearchRequest
    {
        public int CompanyID { get; set; }
        public int? CategoryId { get; set; }
        public int? SubjectId { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
    }
}
