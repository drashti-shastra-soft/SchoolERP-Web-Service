using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class StudentFeeOpeningBalanceResponse
    {
        public int OpeningBalanceId { get; set; }
        public int MonthNumber { get; set; }
        public string? MonthName { get; set; }
        public int FeeHeadId { get; set; }
        public string? FeeHeadName { get; set; }
        public string? DrCr { get; set; }
        public decimal Amount { get; set; }
    }
    public class StudentFeeOpeningBalanceSaveResponse
    {
        public int RESULT { get; set; }

        public string? MESSAGE { get; set; }

        public string? TECHNICALMESSAGE { get; set; }
    }
    public class StudentFeeOpeningBalanceGetRequest
    {
        public int STUDENTID { get; set; }

        public int COMPANYID { get; set; }

        public int SESSIONID { get; set; }
    }
    public class StudentFeeOpeningBalanceLineRequest
    {
        public int OpeningBalanceId { get; set; }

        public int MonthNumber { get; set; }

        public int FeeHeadId { get; set; }

        public string DrCr { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
    public class StudentFeeOpeningBalanceDeleteRequest
    {
        public int STUDENTID { get; set; }

        public int COMPANYID { get; set; }

        public int SESSIONID { get; set; }

        public int? USERID { get; set; }
    }

    public class StudentFeeOpeningBalanceSaveRequest
    {
        public int STUDENTID { get; set; }

        public int COMPANYID { get; set; }

        public int SESSIONID { get; set; }

        public List<StudentFeeOpeningBalanceLineRequest> LINES { get; set; }
            = new();

        public int? USERID { get; set; }
    }

    public class FeeInstallmentDropdownRequest
    {
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
    }

    public class PositionOfStudentFeeRequest
    {
        public int COMPANYID { get; set; }
        public int SESSIONID { get; set; }
        public string? CLASSIDS { get; set; }
        public string? INSTALLMENTLABELS { get; set; }
        public string? FEEHEADIDS { get; set; }
        public string? TRANSPORTFEEHEADIDS { get; set; }
    }

    public class FeeInstallmentDropdownResponse
    {
        public string? Name { get; set; }
        public int Sno { get; set; }
    }

    public class PositionOfStudentFeeResponse
    {
        public string? RowType { get; set; }
        public int? SrNo { get; set; }

        public int? ClassId { get; set; }
        public string? ClassName { get; set; }

        public int? SectionId { get; set; }
        public string? SectionName { get; set; }

        public int? TS { get; set; }
        public int? TStu { get; set; }
        public int? TNS { get; set; }
        public int? TOS { get; set; }

        public decimal? OpnBal { get; set; }
        public decimal? AtFee { get; set; }
        public decimal? DiscAmt { get; set; }

        public decimal? FeeActual { get; set; }
        public decimal? FeeSubm { get; set; }
        public decimal? FeeBal { get; set; }

        public decimal? TrActual { get; set; }
        public decimal? TrSubm { get; set; }
        public decimal? TrBal { get; set; }

        public decimal? GrandActual { get; set; }
        public decimal? GrandSubm { get; set; }
        public decimal? GrandBal { get; set; }

        public int? SortKey { get; set; }
    }
}
