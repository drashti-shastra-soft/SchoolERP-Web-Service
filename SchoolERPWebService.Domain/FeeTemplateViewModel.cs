using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class FeeTemplateViewModel
    {
        public int ROWNO { get; set; }

        public int RESULT { get; set; }

        public int FEETEMPLATEID { get; set; }

        public int CLASSID { get; set; }

        public string? CLASSNAME { get; set; }

        public int SECTIONID { get; set; }

        public string? SECTIONNAME { get; set; }

        public int INSTALLMENTID { get; set; }

        public string? INSTALLMENTNAME { get; set; }

        public string? ADMISSIONTYPE { get; set; }

        public int? FEECATEGORYID { get; set; }

        public string? FEECATEGORYNAME { get; set; }

        public int? CATEGORYID { get; set; }

        public string? StudentCategoryName { get; set; }

        public int NOOFMONTHS { get; set; }

        public DateTime? LATEFEEDATE { get; set; }

        public decimal LATEFEEAMOUNT { get; set; }

        public string? REMARK { get; set; }

        public bool ISACTIVE { get; set; }

        public DateTime? CREATEDON { get; set; }

        public DateTime? MODIFIEDON { get; set; }

        public int TOTALRECORDS { get; set; }
    }

    public class FeeTemplateDetailRequest
    {
        public int DETAILID { get; set; }
        public int SNO { get; set; }
        public int FEEHEADID { get; set; }
        public decimal FEEAMOUNT { get; set; }
        public decimal DISCOUNTPERCENT { get; set; }
        public decimal AMOUNT { get; set; }
        public string ADMISSIONTYPE { get; set; } = string.Empty;
        public bool ISDELETE { get; set; }
    }
    public class FeeTemplateRequest
    {
        public int FEETEMPLATEID { get; set; }
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
        public int CLASSID { get; set; }
        public int SECTIONID { get; set; }
        public int INSTALLMENTID { get; set; }
        public string ADMISSIONTYPE { get; set; } = string.Empty;
        public int? FEECATEGORYID { get; set; }
        public int? CATEGORYID { get; set; }
        public int NOOFMONTHS { get; set; }
        public DateTime? LATEFEEDATE { get; set; }
        public decimal LATEFEEAMOUNT { get; set; }
        public string? REMARK { get; set; }
        public bool ISACTIVE { get; set; }
        public int USERID { get; set; }
        public string? IPADDRESS { get; set; }
    }
    public class FeeTemplateSaveRequest
    {
        // Header
        public int FEETEMPLATEID { get; set; }
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
        public int CLASSID { get; set; }
        public int SECTIONID { get; set; }
        public int INSTALLMENTID { get; set; }
        public string ADMISSIONTYPE { get; set; } = string.Empty;
        public int? FEECATEGORYID { get; set; }
        public int? CATEGORYID { get; set; }
        public int NOOFMONTHS { get; set; }
        public DateTime? LATEFEEDATE { get; set; }
        public decimal LATEFEEAMOUNT { get; set; }
        public string? REMARK { get; set; }
        public bool ISACTIVE { get; set; }
        public int USERID { get; set; }
        public string? IPADDRESS { get; set; }

        // Detail Grid
        public List<FeeTemplateDetailRequest> DETAILS { get; set; }
            = new List<FeeTemplateDetailRequest>();
    }

    public class FeeTemplateResponse
    {
        public int RESULT { get; set; }

        public string? MESSAGE { get; set; }

        public int? FEETEMPLATEID { get; set; }

        public int? TEMPLATEID { get; set; }

        public string? TECHNICALMESSAGE { get; set; }

        public int? ERRORLINE { get; set; }
    }

    public class FeeTemplateListResponse
    {
        public int FEETEMPLATEID { get; set; }
        public int CLASSID { get; set; }
        public string? CLASSNAME { get; set; }

        public int SECTIONID { get; set; }
        public string? SECTIONNAME { get; set; }

        public int INSTALLMENTID { get; set; }
        public string? INSTALLMENTNAME { get; set; }

        public string? ADMISSIONTYPE { get; set; }

        public int? FEECATEGORYID { get; set; }
        public string? FEECATEGORYNAME { get; set; }

        public int? CATEGORYID { get; set; }
        public string? CATEGORYNAME { get; set; }

        public int NOOFMONTHS { get; set; }

        public DateTime? LATEFEEDATE { get; set; }
        public decimal LATEFEEAMOUNT { get; set; }

        public string? REMARK { get; set; }

        public bool ISACTIVE { get; set; }
    }

    public class FeeTemplateHeaderResponse
    {
        public int FEETEMPLATEID { get; set; }

        public int CLASSID { get; set; }
        public string? CLASSNAME { get; set; }

        public int SECTIONID { get; set; }
        public string? SECTIONNAME { get; set; }

        public int INSTALLMENTID { get; set; }
        public string? INSTALLMENTNAME { get; set; }

        public string? ADMISSIONTYPE { get; set; }

        public int? FEECATEGORYID { get; set; }
        public string? FEECATEGORYNAME { get; set; }

        public int? CATEGORYID { get; set; }
        public string? CATEGORYNAME { get; set; }

        public int NOOFMONTHS { get; set; }

        public DateTime? LATEFEEDATE { get; set; }

        public decimal LATEFEEAMOUNT { get; set; }

        public string? REMARK { get; set; }

        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }

        public bool ISACTIVE { get; set; }

        public int? CREATEDBY { get; set; }
        public DateTime? CREATEDON { get; set; }

        public int? MODIFIEDBY { get; set; }
        public DateTime? MODIFIEDON { get; set; }
    }

    public class FeeTemplateDetailResponse
    {
        public int DETAILID { get; set; }

        public int TEMPLATEID { get; set; }

        public int SNO { get; set; }

        public int FEEHEADID { get; set; }

        public string? FEEHEADNAME { get; set; }

        public decimal FEEAMOUNT { get; set; }

        public decimal DISCOUNTPERCENT { get; set; }

        public decimal AMOUNT { get; set; }

        public string? ADMISSIONTYPE { get; set; }

        public bool ISACTIVE { get; set; }
    }
    public class FeeTemplateTotalResponse
    {
        public decimal TOTALFEE { get; set; }

        public decimal TOTALAMOUNT { get; set; }
    }
    public class FeeTemplateGetByIdResponse
    {
        public int RESULT { get; set; }

        public string? MESSAGE { get; set; }

        public FeeTemplateHeaderResponse? HEADER { get; set; }

        public List<FeeTemplateDetailResponse> DETAILS { get; set; }
            = new List<FeeTemplateDetailResponse>();

        public FeeTemplateTotalResponse? TOTAL { get; set; }

        public string? TECHNICALMESSAGE { get; set; }

        public int? ERRORLINE { get; set; }
    }

    public class FeeTemplatePageRequest
    {
        public int COMPANYID { get; set; }

        public int? SESSIONID { get; set; }

        public int? CLASSID { get; set; }

        public int? SECTIONID { get; set; }

        public int? INSTALLMENTID { get; set; }

        public string? SEARCHTEXT { get; set; }

        public bool? ISACTIVE { get; set; }

        public int PAGEINDEX { get; set; } = 1;

        public int PAGESIZE { get; set; } = 10;

        public string SORTCOLUMN { get; set; } = "CLASSNAME";

        public string SORTDIRECTION { get; set; } = "ASC";
    }

    public class FeeTemplatePageApiResponse
    {
        public int RESULT { get; set; }

        public string? MESSAGE { get; set; }

        public List<FeeTemplateViewModel> DATA { get; set; }
            = new List<FeeTemplateViewModel>();

        public int TOTALRECORDS { get; set; }

        public int PAGEINDEX { get; set; }

        public int PAGESIZE { get; set; }

        public string? TECHNICALMESSAGE { get; set; }

        public int? ERRORLINE { get; set; }
    }

    public class FeeTemplateGetByClassSectionResponse
    {
        public int FEETEMPLATEID { get; set; }
        public int CLASSID { get; set; }
        public string? CLASSNAME { get; set; }

        public int SECTIONID { get; set; }
        public string? SECTIONNAME { get; set; }

        public int INSTALLMENTID { get; set; }
        public string? ADMISSIONTYPE { get; set; }

        public int FEECATEGORYID { get; set; }
        public string? FEECATEGORYNAME { get; set; }

        public int? CATEGORYID { get; set; }
        public string? StudentCategoryName { get; set; }

        public decimal? NOOFMONTHS { get; set; }
        public DateTime? LATEFEEDATE { get; set; }
        public decimal? LATEFEEAMOUNT { get; set; }

        public string? REMARK { get; set; }
        public bool ISACTIVE { get; set; }

        // JSON returned by SP
        public string? DETAILSJSON { get; set; }

        public decimal TOTALAMOUNT { get; set; }

        // Deserialized details
        public List<FeeTemplateDetailResponse> Details { get; set; }
            = new List<FeeTemplateDetailResponse>();
    }

    public class FeeTemplateGetByClassSectionRequest
    {
        public int CLASSID { get; set; }
        public int SECTIONID { get; set; }
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }

        public int? CATEGORYID { get; set; }
        public string? ADMISSIONTYPE { get; set; }
        public int? INSTALLMENTID { get; set; }
    }

    public class FeeTemplateDetailsResponse
    {
        public FeeTemplateHeaderResponse Header { get; set; }
        public List<FeeTemplateDetailResponse> Details { get; set; } = new();
    }


    public class StudentFeeTemplateDetailsResponse
    {
        public StudentFeeTemplateHeaderResponse Header { get; set; }
        public List<StudentFeeTemplateDetailResponse> Details { get; set; } = new();
    }

    public class StudentFeeTemplateHeaderResponse
    {
        public int FeeTemplateId { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? SectionId { get; set; }
        public string? SectionName { get; set; }
        public string? AdmissionType { get; set; }
        public int? FeeCategoryId { get; set; }
        public int? CategoryId { get; set; }
        public int? InstallmentId { get; set; }
        public int? NoOfMonths { get; set; }
        public DateTime? LateFeeDate { get; set; }
        public decimal? LateFeeAmount { get; set; }
        public string? Remark { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public string? InstallmentLabel { get; set; }
    }

    public class StudentFeeTemplateDetailResponse
    {
        public int DetailId { get; set; }
        public int FeeHeadId { get; set; }
        public string? FeeHeadName { get; set; }
        public string? AddLess { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Submitted { get; set; }
        public decimal Balance { get; set; }
        public decimal Discount { get; set; }
        public decimal Received { get; set; }
    }

    public class StudentInstallmentSummary 
    {
        public string Instt { get; set; }
        public decimal ActAmt { get; set; }
        public decimal Subm { get; set; }
        public decimal Balance { get; set; }
    }

    public class PaymentModeSummary
    {
        public int RESULT { get; set; }
        public string? MESSAGE { get; set; }
        public string? TECHNICALMESSAGE { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }

    public class StudentFeeCollectionRequest
    {

        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? UserId { get; set; }
    }
}
