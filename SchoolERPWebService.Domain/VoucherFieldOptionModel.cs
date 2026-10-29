using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class VoucherFieldOptionModel
    {
        public int OptionID { get; set; }
        public int? CompanyID { get; set; }
        public int FieldID { get; set; }
        public string? OptionLabel { get; set; }
        public string? OptionValue { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class VoucherFieldOptionRequest 
    {
        public int OptionID { get; set; }
        public int? CompanyID { get; set; }
        public int FieldID { get; set; }
        public string? OptionLabel { get; set; }
        public string? OptionValue { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsDefault { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public string? IPAddress { get; set; }
    }

    public class VoucherFieldOptionResponse
    {
        public string? OptionValue { get; set; }

        public string? OptionLabel { get; set; }
    }

    
}
