using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class VoucherTypeFieldModel
    {
        public int FieldID { get; set; }
        public int? CompanyID { get; set; }
        public int VoucherId { get; set; }
        public string? FieldLabel { get; set; }
        public string? FieldCode { get; set; }
        public string? FieldType { get; set; }
        public string? Section { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsReadOnly { get; set; }
        public int? MaxLength { get; set; }
        public int? DecimalPlaces { get; set; }
        public string? DefaultValue { get; set; }
        public string? SourceType { get; set; }
        public string? SourceTable { get; set; }
        public string? SourceValueCol { get; set; }
        public string? SourceTextCol { get; set; }
        public string? SourceFilter { get; set; }
        public string? VoucherType { get; set; }
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

    public class VoucherFieldRequest
    {
        public int FieldID { get; set; } = 0;

        public int? CompanyID { get; set; }

        public int VoucherId { get; set; }

        public string FieldLabel { get; set; }

        public string FieldCode { get; set; }

        public string FieldType { get; set; }

        public string Section { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsRequired { get; set; } = false;

        public bool IsReadOnly { get; set; } = false;

        public int? MaxLength { get; set; }

        public int? DecimalPlaces { get; set; }

        public string? DefaultValue { get; set; }

        public string? SourceType { get; set; }

        public string? SourceTable { get; set; }

        public string? SourceValueCol { get; set; }

        public string? SourceTextCol { get; set; }

        public string? SourceFilter { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class VoucherDynamicFieldResponse
    {
        public int FieldID { get; set; }
        public int? CompanyID { get; set; }
        public int VoucherId { get; set; }
        public string? FieldLabel { get; set; }
        public string? FieldCode { get; set; }
        public string? FieldType { get; set; }
        public string? Section { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsRequired { get; set; }
        public bool? IsReadOnly { get; set; }
        public int? MaxLength { get; set; }
        public int? DecimalPlaces { get; set; }
        public string? DefaultValue { get; set; }
        public string? SourceType { get; set; }
        public string? SourceTable { get; set; }
        public string? SourceValueCol { get; set; }
        public string? SourceTextCol { get; set; }
        public string? SourceFilter { get; set; }
        // JSON returned by SQL
        public string? Options { get; set; }
    }
}
