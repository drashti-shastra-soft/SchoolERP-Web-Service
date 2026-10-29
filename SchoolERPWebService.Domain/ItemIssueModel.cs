using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ItemIssueModel
    {
        public int ItemIssueId { get; set; }

        public int? RoleId { get; set; }
        public int? IssueTo { get; set; }
        public int? IssueBy { get; set; }
        public string? IssueToName { get; set; }
        public string? IssueByName { get; set; }

        public DateTime? IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string? Note { get; set; }

        public int? ItemCategoryId { get; set; }
        public int? ItemId { get; set; }
        public int? Quantity { get; set; }

        public DateTime? ExpectedReturnDate { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        public int? QuantityReturned { get; set; }
        public int? ReturnedBy { get; set; }

        public string? Status { get; set; }
        public string? CategoryName { get; set; }
        public string? ItemName { get; set; }

        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }

        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ItemIssueRequest 
    {
        public int ItemIssueId { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int? RoleId { get; set; }
        public int? IssueTo { get; set; }
        public int? IssueBy { get; set; }

        public DateTime? IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string? Note { get; set; }

        public int? ItemCategoryId { get; set; }
        public int? ItemId { get; set; }
        public int? Quantity { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; }
        public int? UserId { get; set; }
        public string? IPAddress { get; set; }
    }

    public class ItemIssueReturnModel
    {
        public int ItemIssueId { get; set; }
        public int QuantityReturned { get; set; }
        public int ReturnedBy { get; set; }
        public DateTime? ActualReturnDate { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? IpAddress { get; set; }
    }
}
