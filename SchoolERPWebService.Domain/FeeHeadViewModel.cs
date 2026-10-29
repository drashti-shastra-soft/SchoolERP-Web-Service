using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class FeeHeadViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE


        public int FeeHeadId { get; set; }

        public string FeeHeadName { get; set; }

        public string? DisplayLabel { get; set; }

        // Link with Account Name
        public int AccountId { get; set; }

        // Fee Type
        public int FeeTypeId { get; set; }

        public bool IsRefunded { get; set; }

        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsDelete { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }
    public class FeeHeadRequest
    {
        public int FeeHeadId { get; set; }

        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public string FeeHeadName { get; set; }

        public string? DisplayLabel { get; set; }

        public int AccountId { get; set; }

        public int FeeTypeId { get; set; }

        public bool IsRefunded { get; set; } = false;

        public bool IsActive { get; set; }

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }
}
