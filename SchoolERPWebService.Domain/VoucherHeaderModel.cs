using SchoolERPWebService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class VoucherHeaderModel
    {
        public int VoucherHeaderID { get; set; }
        public int VoucherId { get; set; }
        public string? VoucherNo { get; set; }
        public DateTime? VoucherDate { get; set; }
        public string? VNo { get; set; }
        public int? CostCenterID { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? CompanyID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public string? ForType { get; set; }
        public int? ForRefId { get; set; }
        public string VoucherType { get; set; }
        public int Result { get; set; }       // maps to RESULT
         // maps to MESSAGE
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class VoucherHeaderRequest
    {
        public int VoucherHeaderID { get; set; } = 0;
        public int? CompanyID { get; set; }
        public int VoucherId { get; set; }
        public string VoucherNo { get; set; }
        public DateTime VoucherDate { get; set; }
        public string? VNo { get; set; }
        public int? CostCenterID { get; set; }
        public decimal TotalAmount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public int UserId { get; set; }
        public string? IPAddress { get; set; }
    }

    public class VoucherHeaderSearchRequest
    {
        public int? VoucherId { get; set; }
        public int? CompanyID { get; set; }
        public int? CostCenterID { get; set; }
        public DateTime? FROMDATE { get; set; }
        public DateTime? TODATE { get; set; }
        public bool? ISACTIVE { get; set; }
        public bool INCLUDEDELETED { get; set; } = false;
        public string? SearchKeyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class VoucherHeaderValue
    {
        public int ValueID { get; set; }
        public int VoucherHeaderID { get; set; }
        public int FieldID { get; set; }
        public string? ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBit { get; set; }
        public int? RefID { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class VoucherHeaderValueRequest
    {
        public int ValueID { get; set; } = 0;

        public int? VoucherHeaderID { get; set; }

        public int FieldID { get; set; }

        public string? ValueText { get; set; }

        public decimal? ValueNumber { get; set; }

        public DateTime? ValueDate { get; set; }

        public bool? ValueBit { get; set; }

        public int? RefID { get; set; }

        public int? CompanyID { get; set; }

        public int? SessionID { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class VoucherDetail
    {
        public int DetailID { get; set; }
        public int VoucherHeaderID { get; set; }
        public int? RowNo { get; set; }
        public int? AccID { get; set; }
        public string? Narration { get; set; }
        public decimal? Amount { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class VoucherDetailRequest
    {
        public int DetailID { get; set; } = 0;

        public int VoucherHeaderID { get; set; }

        public int? RowNo { get; set; }

        public int? AccID { get; set; }

        public string? Narration { get; set; }

        public decimal? Amount { get; set; }

        public int? CompanyID { get; set; }

        public int? SessionID { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class VoucherDetailValue
    {
        public int ValueID { get; set; }
        public int DetailID { get; set; }
        public int FieldID { get; set; }
        public string? ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBit { get; set; }
        public int? RefID { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class VoucherDetailValueRequest
    {
        public int ValueID { get; set; } = 0;
        public int DetailID { get; set; }
        public int FieldID { get; set; }
        public string? ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBit { get; set; }
        public int? RefID { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; } = true;
        public int UserId { get; set; }
        public string? IPAddress { get; set; }
    }

    public class VoucherValueSearchRequest
    {
        public int? CompanyID { get; set; }

        public int? SessionID { get; set; }

        public int? VoucherHeaderID { get; set; }

        public int? DetailID { get; set; }

        public int? FieldID { get; set; }

        public bool? ISACTIVE { get; set; }

        public bool INCLUDEDELETED { get; set; } = false;
    }

    public class VoucherSaveFullRequest
    {
        public int VoucherHeaderID { get; set; } = 0;

        public int VoucherId { get; set; }

        public int? CompanyID { get; set; }

        public int? SessionID { get; set; }

        public string? VoucherNo { get; set; }

        public DateTime VoucherDate { get; set; }

        public string? VNo { get; set; }

        public int? CostCenterID { get; set; }
        public string? ForType { get; set; }
        public int? ForRefId { get; set; }
        public List<VoucherHeaderValueJson> HeaderValues { get; set; }
            = new List<VoucherHeaderValueJson>();

        public List<VoucherDetailJson> Details { get; set; }
            = new List<VoucherDetailJson>();

        public int UserId { get; set; }

        public string? IPAddress { get; set; }
    }

    public class VoucherHeaderValueJson
    {
        public int FieldID { get; set; }

        public string? ValueText { get; set; }

        public decimal? ValueNumber { get; set; }

        public DateTime? ValueDate { get; set; }

        public bool? ValueBit { get; set; }

        public int? RefID { get; set; }
    }

    public class VoucherDetailJson
    {
        public int DetailID { get; set; } = 0;

        public int? RowNo { get; set; }

        public int? AccID { get; set; }

        public string? Narration { get; set; }

        public decimal? Amount { get; set; }

        public List<VoucherDetailValueJson> Values { get; set; }
            = new List<VoucherDetailValueJson>();
    }

    public class VoucherDetailValueJson
    {
        public int FieldID { get; set; }

        public string? ValueText { get; set; }

        public decimal? ValueNumber { get; set; }

        public DateTime? ValueDate { get; set; }

        public bool? ValueBit { get; set; }

        public int? RefID { get; set; }
    }

    public class VoucherSaveFullResult : SpResult
    {
        public int? VOUCHERHEADERID { get; set; }
        public string? VOUCHERNO { get; set; }
    }

    public class VoucherHeaderFieldValueDto
    {
        public int ValueID { get; set; }
        public int VoucherHeaderID { get; set; }
        public int FieldID { get; set; }
        public string? FieldCode { get; set; }
        public string? FieldLabel { get; set; }
        public string? FieldType { get; set; }
        public string? ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBit { get; set; }
        public int? RefID { get; set; }
    }

    public class VoucherDetailDto
    {
        public int DetailID { get; set; }
        public int VoucherHeaderID { get; set; }
        public int RowNo { get; set; }
        public int? AccID { get; set; }
        public string? AccountDisplayName { get; set; }
        public string? Narration { get; set; }
        public decimal Amount { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool IsActive { get; set; }
    }

    public class VoucherDetailValueDto
    {
        public int ValueID { get; set; }
        public int DetailID { get; set; }
        public int FieldID { get; set; }
        public string? FieldCode { get; set; }
        public string? FieldLabel { get; set; }
        public string? FieldType { get; set; }
        public string? ValueText { get; set; }
        public decimal? ValueNumber { get; set; }
        public DateTime? ValueDate { get; set; }
        public bool? ValueBit { get; set; }
        public int? RefID { get; set; }
    }

    public class VoucherHeaderFullResult
    {
        public VoucherHeaderModel? Header { get; set; }
        public Dictionary<int, string> DynamicFieldValues { get; set; } = new();
        public List<VoucherDetailDto> Details { get; set; } = new();
        public List<VoucherDetailValueDto> DetailValues { get; set; } = new();
    }

    public class VoucherHeaderAttachmentUpsertRequest
    {
        public int VoucherHeaderID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }

    public class VoucherBankClearingRequest
    {
        public int CompanyID { get; set; }

        /// <summary>
        /// RefID / ValueText saved for the header's BankName field.
        /// </summary>
        public int BankFieldValue { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public bool Cleared { get; set; }
    }

    public class VoucherDetailClearingResponse
    {
        public int VoucherHeaderID { get; set; }

        public int DetailID { get; set; }

        public string VoucherNo { get; set; }

        public int AccID { get; set; }

        public string AccountDisplayName { get; set; }

        public string Narration { get; set; }

        public string TranNo { get; set; }

        public DateTime? CheqDDDate { get; set; }

        public decimal Amount { get; set; }

        public bool IsCleared { get; set; }

        public DateTime? ClearingDate { get; set; }
    }

    public class VoucherBankClearingSaveRequest
    {
        public string DetailIdsJson { get; set; }
        public DateTime? ClearingDate { get; set; }
        public bool Cleared { get; set; }
        public int? CompanyID { get; set; }
        public int UserId { get; set; }
        public string? IPAddress { get; set; }
    }
}
