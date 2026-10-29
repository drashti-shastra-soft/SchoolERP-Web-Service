using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class LibrarySubjectModel
    {
        public int SubjectId { get; set; }
        public string? Subjectname { get; set; }
        public string? DisplayLabel { get; set; }
        public string? ClassNo { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class LibrarySubjectRequest
    {
        public int SubjectId { get; set; }
        public string? Subjectname { get; set; }
        public string? DisplayLabel { get; set; }
        public string? ClassNo { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    public class LibrarySubjectPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<LibrarySubjectModel> Subject { get; set; } = new List<LibrarySubjectModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
    }

    public class LibrarySubjectAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public LibrarySubjectModel Subject { get; set; } = new LibrarySubjectModel();
        public LibrarySubjectModel EditSubject { get; set; } = new LibrarySubjectModel();
    }
}
