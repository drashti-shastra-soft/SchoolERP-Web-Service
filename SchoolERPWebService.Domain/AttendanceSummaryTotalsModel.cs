using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class AttendanceSummaryTotalsModel
    {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalPresent { get; set; }
        public int TotalLate { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalHalfDay { get; set; }
        public int TotalHoliday { get; set; }
    }

    public class AttendanceGridRowModel
    {
        public int DayNum { get; set; }
        public int MonthNum { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public DateTime AttendanceDate { get; set; }
        public int AttendanceStatus { get; set; }
        public string AttendanceStatusName { get; set; } = string.Empty;
    }
    
    public class StudentAttendanceSummaryModel
    {
        public AttendanceSummaryTotalsModel Totals { get; set; } = new();
        public List<AttendanceGridRowModel> AttendanceRows { get; set; } = new();
    }

    public class StaffAttendanceSummaryModel
    {
        public AttendanceSummaryTotalsModel Totals { get; set; } = new();
        public List<AttendanceStaffGridRowModel> AttendanceRows { get; set; } = new();
    }
    public class AttendanceStaffGridRowModel
    {
        public int DayNum { get; set; }
        public int MonthNum { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public DateTime AttendanceDate { get; set; }
        public string AttendanceStatus { get; set; }
        public string AttendanceStatusName { get; set; } = string.Empty;
    }
}
