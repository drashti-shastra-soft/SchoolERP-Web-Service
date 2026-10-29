using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class StudentAttendanceViewModel
    {
        public int StudentID { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public string? StudentName { get; set; }
        public int AttendanceStatus { get; set; }
        public string? Note { get; set; }
        public string? Reason { get; set; }
        public int? AttendanceID { get; set; }
        public bool? IsStudentLeaveAdd { get; set; }
        public string? LeaveStatus { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
                                              // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class AttendanceUpsertRequest
    {
        public int CompanyID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public List<AttendanceRecordItem> AttendanceData { get; set; } = new();
    }

    public class AttendanceRecordItem
    {
        public int StudentID { get; set; }
        public int Status { get; set; }
        public string? Note { get; set; }
    }

    public class StudentAttendanceSearchRequest
    {
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public int CompanyID { get; set; }
        public int? StudentID { get; set; }
        public string? SearchKeyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class StudentAttendancePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public List<StudentAttendanceViewModel> StudentAttendanceModel { get; set; } = new List<StudentAttendanceViewModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionID { get; set; }
        public int? StudentId { get; set; }
        public DateTime? AttendanceDate { get; set; }
    }

    public class AttendanceCountViewModel
    {
        public int PresentStudents { get; set; }
        public int TotalStudents { get; set; }
        public string Data { get; set; }
    }

    public class StaffAttendanceCountViewModel
    {
        public int PresentStaff { get; set; }
        public int TotalStaff { get; set; }
    }
}
