using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class AttendanceMonthlyReport
    {
    }

    // ------------------------------------------------------------------
    // REQUEST — mirrors StudentReportRequest's shape/usage pattern
    // ------------------------------------------------------------------
    public class AttendanceMonthlyReportRequest
    {
        public int? ClassId { get; set; }
        public int? SectionId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public int? StudentId { get; set; }
        public string? SearchKeyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
        public int? StaffId { get; set; }
        public int? RoleId { get; set; }

        // stamped server-side, same as StudentReportRequest.UserId/IpAddress
        public int? UserId { get; set; }
        public string? IpAddress { get; set; }
    }

    // ------------------------------------------------------------------
    // RESULT SET 1 — one row per student. D01..D(last day) are NOT fixed
    // properties because the SP only pivots as many day columns as the
    // month actually has (28/29/30/31), so they're captured dynamically
    // into DayCells instead of being hardcoded to 31 possible properties.
    // ------------------------------------------------------------------
    public class AttendanceMonthlyResponse
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int STUDENTID { get; set; }
        public string ROLLNO { get; set; }
        public string ADMISSIONNO { get; set; }
        public string STUDENTNAME { get; set; }

        public int P { get; set; }
        public int L { get; set; }
        public int A { get; set; }
        public int H { get; set; }
        public int ATTENDANCEPERCENT { get; set; }

        // Key = "D01".."D31" (upper-cased), Value = "P"/"L"/"A"/"H"/"" (not marked)
        public Dictionary<string, string> DayCells { get; set; } = new();

        public string GetDay(string dayCol) =>
            DayCells != null && DayCells.TryGetValue(dayCol, out var v) ? v : string.Empty;
    }

    // ------------------------------------------------------------------
    // RESULT SET 2 — one row per calendar day, holiday flag independent
    // of any student's "H" (half-day) mark.
    // ------------------------------------------------------------------
    public class AttendanceHolidayResponse
    {
        public string DAYCOL { get; set; }
        public DateTime CALDATE { get; set; }
        public string WEEKDAYNAME { get; set; }
        public int ISHOLIDAY { get; set; }
        public string HOLIDAYNAME { get; set; }

        // View-only convenience properties — never part of the API wire
        // format. Without [JsonIgnore], IsHoliday collides with ISHOLIDAY
        // under ASP.NET Core's default case-insensitive JSON property
        // matching ("isHoliday" vs "isholiday" are ambiguous to it), which
        // throws "The JSON property name for '...isHoliday' collides with
        // another property" at serialization time.
        [JsonIgnore]
        public bool IsHoliday => ISHOLIDAY == 1;
        [JsonIgnore]
        public int DayNumber => CALDATE.Day;
        [JsonIgnore]
        public string WeekdayShort => string.IsNullOrEmpty(WEEKDAYNAME) ? "" : WEEKDAYNAME.Substring(0, 3);
    }

    // ------------------------------------------------------------------
    // Combined paged result — same shape family as PagedResult<T>, plus
    // the Days calendar (result set 2) needed to render/highlight headers.
    // ------------------------------------------------------------------
    public class AttendanceMonthlyPagedResult<T>
    {
        public List<T> Data { get; set; } = new();
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<AttendanceHolidayResponse> Days { get; set; } = new();
    }

    // ------------------------------------------------------------------
    // PAGE VIEW MODEL — consumed by Index.cshtml / the table partial
    // ------------------------------------------------------------------
    public class AttendanceMonthlyReportPageViewModel
    {
        public List<AttendanceMonthlyResponse> Students { get; set; } = new();
        public List<AttendanceHolidayResponse> Days { get; set; } = new();

        // DAYCOL list ("D01".."D31") driving the dynamic <th>/<td> columns.
        // Built from Days so the header renders correctly even on a
        // zero-student page (Days still reflects the requested month).
        public List<string> DayColumns { get; set; } = new();

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int ClassId { get; set; }
        public int SectionId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int? StudentId { get; set; }
        public string SearchKeyword { get; set; }

        public int CompanyId { get; set; }
        public int SessionId { get; set; }

        // TODO: wire to your existing dropdown-source service if you have one
        // (e.g. GetClassListAsync / GetSectionListAsync), same as other reports.
        public List<(int Id, string Name)> ClassList { get; set; } = new();
        public List<(int Id, string Name)> SectionList { get; set; } = new();

        // Reuse whatever permission type the rest of the app uses
        // (referenced as `Model.Permissions` / `p.CanExport` in the views).
        public dynamic Permissions { get; set; }

        // Convenience lookup used by the partial to shade a day column header.
        public AttendanceHolidayResponse GetDayMeta(string dayCol) =>
            Days?.Find(d => string.Equals(d.DAYCOL, dayCol, StringComparison.OrdinalIgnoreCase));
    }

    public class AttendanceTypeReportResponse
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }

        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int? STUDENTID { get; set; }
        public string ADMISSIONNO { get; set; }
        public string STUDENTNAME { get; set; }
        public string CLASSNAME { get; set; }
        public string FATHERNAME { get; set; }

        public DateTime? DOB { get; set; }
        public DateTime? ADMISSIONDATE { get; set; }

        public string GENDER { get; set; }
        public string MobileNo { get; set; }

        public string ATTENDANCETYPE { get; set; }

        public DateTime? FROMDATE { get; set; }
        public DateTime? TODATE { get; set; }

        public int COUNT { get; set; }

        // Returned only in the error response
        public string TECHNICALMESSAGE { get; set; }
        public int? ERRORLINE { get; set; }
    }

    public class AttendanceDailyReportResponse
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }

        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int? CLASSID { get; set; }
        public int? SECTIONID { get; set; }
        public string CLASSNAME { get; set; }

        public int TOTALSTUDENTS { get; set; }
        public int TOTALPRESENT { get; set; }
        public int TOTALABSENT { get; set; }

        public decimal PRESENTPERCENT { get; set; }
        public decimal ABSENTPERCENT { get; set; }

        public DateTime? REPORTDATE { get; set; }

        // Error response fields
        public string TECHNICALMESSAGE { get; set; }
        public int? ERRORLINE { get; set; }
    }

    public class AttendanceDayWiseReportResponse
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }

        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int SRNO { get; set; }
        public int STUDENTID { get; set; }

        public string ADMISSIONNO { get; set; }
        public string ROLLNUMBER { get; set; }
        public string STUDENTNAME { get; set; }

        public string ATTENDANCE { get; set; }
        public DateTime? ATTENDANCEDATE { get; set; }

        // Error response fields
        public string TECHNICALMESSAGE { get; set; }
        public int? ERRORLINE { get; set; }
    }

    public class AttendanceStaffMonthlyResponse
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int STAFFID { get; set; }
        public string STAFFCODE { get; set; }
        public string STAFFNAME { get; set; }
        public string ROLENAME { get; set; }

        public int P { get; set; }
        public int L { get; set; }
        public int A { get; set; }
        public int H { get; set; }
        public int ATTENDANCEPERCENT { get; set; }

        // Key = "D01".."D31" (upper-cased), Value = "P"/"L"/"A"/"H"/"" (not marked)
        public Dictionary<string, string> DayCells { get; set; } = new();

        public string GetDay(string dayCol) =>
            DayCells != null && DayCells.TryGetValue(dayCol, out var v) ? v : string.Empty;
    }

    public class StaffAttendanceDayWiseReportResponse
    {
        public int Result { get; set; }
        public string Message { get; set; }

        public int? StaffID { get; set; }
        public string StaffCode { get; set; }
        public string StaffName { get; set; }
        public string RoleName { get; set; }

        public int? StaffAttendanceID { get; set; }
        public DateTime? StaffAttendanceDate { get; set; }

        public string StaffAttendance { get; set; }
        public string StaffAttendanceSource { get; set; }

        public string IPAddress { get; set; }
        public string Agent { get; set; }

        public string StaffAttendanceNote { get; set; }
        public DateTime? LastUpdated { get; set; }

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        // Error response fields
        public string TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }
}
