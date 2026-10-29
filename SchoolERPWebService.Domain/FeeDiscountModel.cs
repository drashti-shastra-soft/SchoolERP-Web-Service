using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    internal class FeeDiscountModel
    {
    }

    public class StudentFeeDiscountRowResponse
    {
        public int MapStudentsFeeId { get; set; }

        public int DetailId { get; set; }

        public int FeeTemplateId { get; set; }

        public string? InstallmentLabel { get; set; }

        public int InstallmentSno { get; set; }

        public int FeeHeadId { get; set; }

        public string? FeeHeadName { get; set; }

        public decimal ActualAmount { get; set; }

        public decimal SavedDiscount { get; set; }

        public bool IsPaid { get; set; }
    }

    public class StudentFeeDiscountRowsRequest
    {
        public int STUDENTID { get; set; }
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
    }

    public class StudentFeeDiscountHeaderResponse
    {
        public int DiscountId { get; set; }

        public int ConcessionId { get; set; }

        public int? FromTemplateId { get; set; }

        public int FeeHeadId { get; set; }

        public string? DiscountType { get; set; }

        public decimal DiscountValue { get; set; }

        public decimal TotalActual { get; set; }

        public decimal TotalDiscount { get; set; }

        public int? DiscountByUserId { get; set; }

        public string? AttachmentPath { get; set; }

        public string? Remark { get; set; }
    }

    public class StudentFeeDiscountHeaderRequest
    {
        public int STUDENTID { get; set; }

        public int COMPANYID { get; set; }

        public int SESSIONID { get; set; }

        public int? FEEHEADID { get; set; }
    }

    public class FeeDiscountLineRequest
    {
        public int DetailId { get; set; }

        public decimal DiscountAmount { get; set; }
    }

    public class StudentFeeDiscountSaveRequest
    {
        public int STUDENTID { get; set; }

        public int COMPANYID { get; set; }

        public int SESSIONID { get; set; }

        public int CONCESSIONID { get; set; }

        public int? FROMTEMPLATEID { get; set; }

        public int FEEHEADID { get; set; }

        public string DISCOUNTTYPE { get; set; } = "A";

        public decimal DISCOUNTVALUE { get; set; }

        public int? DISCOUNTBYUSERID { get; set; }

        public string? ATTACHMENTPATH { get; set; }

        public string REMARK { get; set; } = string.Empty;

        public List<FeeDiscountLineRequest> LINES { get; set; } = new();

        public int? USERID { get; set; }
    }

    public class StudentFeeDiscountSaveResponse
    {
        public int RESULT { get; set; }

        public string? MESSAGE { get; set; }

        public decimal TOTALDISCOUNT { get; set; }
    }

}
