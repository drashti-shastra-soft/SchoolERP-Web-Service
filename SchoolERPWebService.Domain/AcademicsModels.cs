using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    public class TimeTableViewModel
    {
        public int TimeTableID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public string? ClassName { get; set; }
        public int SectionID { get; set; }
        public string? SectionName { get; set; }
        public int SubjectID { get; set; }
        public int SubjectGroupID { get; set; }
        public string? SubjectName { get; set; }
        public int StaffID { get; set; }
        public string? StaffName { get; set; }
        public string Day { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string? RoomNo { get; set; }
        public bool IsActive { get; set; }

        
        // Holiday fields — existing
        public bool IsHoliday { get; set; }
        public long? HolidayID { get; set; }
        public string HolidayName { get; set; }
        public DateTime? HolidayDate { get; set; }

        // NEW — from TBL_MST_HOLIDAYTYPE + TBL_MST_HOLIDAY
        public string HolidayTypeName { get; set; }
        public string ColourCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? NoOfDays { get; set; }
        public int? LessonPlanId { get; set; }
    }

    public class TimeTableUpsertRequest
    {
        public int TimeTableID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public int SubjectGroupID { get; set; }
        public int SubjectID { get; set; }
        public int StaffID { get; set; }
        public string Day { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string? RoomNo { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }
    public class BulkTimeTableUpsertRequest
    {       
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string Data { get; set; }
    }
    public class AddTimeTablePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<MstSectionViewModel> Sections { get; set; } = new();
        public List<Dropdowbinding> Subjects { get; set; } = new();
        public List<HRStaffViewModel> Staff { get; set; } = new();
        public List<MstSubjectGroupViewModel> SubjectGroups { get; set; } = new();
        public List<TimeTableViewModel> TimeTableSlots { get; set; } = new();
        public int SelectedClassId { get; set; }
        public int SelectedSectionId { get; set; }
        public int SelectedSubjectGroupId { get; set; }
    }

    public class TeacherTimeTablePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<HRStaffViewModel> Staff { get; set; } = new();
        public List<TimeTableViewModel> TimeTableSlots { get; set; } = new();
        public int SelectedStaffId { get; set; }
    }

    public class ClassTeacherViewModel
    {
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int ClassTeacherID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SectionID { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string TEACHERS { get; set; } = string.Empty;
        public int StaffID { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }

        public string STAFFIDS { get; set; } = "";

        public List<int> StaffIDIdsCsv =>
            string.IsNullOrWhiteSpace(STAFFIDS)
                ? new List<int>()
                : STAFFIDS.Split(',')
                               .Select(int.Parse)
                               .ToList();

        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ClassTeacherUpsertRequest
    {
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public List<int> StaffIDs { get; set; } = new();
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class AssignClassTeacherPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<ClassTeacherViewModel> Assignments { get; set; } = new();
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<HRStaffViewModel> Staff { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? SectionID { get; set; }
    }
    public class PromotionSummary
    {
        public int Result { get; set; }
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int? NextClassId { get; set; }
        public string? NextClassName { get; set; }
        public int? NextSessionId { get; set; }
        public string? NextSessionTitle { get; set; }
    }
    public class StudentPromotionViewModel
    {
        public int STUDENTRESULT { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE

        public int StudentID { get; set; }
        public string AdmissionNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public DateTime? DOB { get; set; }
        public string Result { get; set; } = "Pass"; // Pass, Fail
        public string NextStatus { get; set; } = "Continue"; // Continue, Leave
        public string NextClassName { get; set; } = "Continue"; // Continue, Leave
        public int NEXTCLASSID { get; set; } // Continue, Leave
        public int NEXTSESSIONID { get; set; } // Continue, Leave
        public string NEXTSESSIONTITLE { get; set; }// Continue, Leave NEXTSESSIONID	NEXTSESSIONTITLE


        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class PromotionRequest
    {
        public int CurrentClassID { get; set; }
        public int CurrentSectionID { get; set; }
        public int NextSessionID { get; set; }
        public int NextClassID { get; set; }
        public int NextSectionID { get; set; }
        public List<StudentPromotionItem> Students { get; set; } = new();
    }

    public class StudentPromotionItem
    {
        public int StudentID { get; set; }
        public string Result { get; set; } = "Pass";
        public string NextStatus { get; set; } = "Continue";
    }

    public class PromoteStudentsPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<StudentPromotionViewModel> Students { get; set; } = new();
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<MstSessionViewModel> Sessions { get; set; } = new();

        public List<MstCompanyViewModel> Companies { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? SessionId { get; set; }
        public int? SectionID { get; set; }

        // Filter values
        public int? SelectedClassId { get; set; }
        public int? SelectedSectionId { get; set; }
        public int? NextSessionId { get; set; }
        public int? NextClassId { get; set; }
        public int? NextSectionId { get; set; }
    }

    public class AssignClassTeacherAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstSectionViewModel Assignments { get; set; } = new();
        public List<MstClassViewModel> Classes { get; set; } = new();
        public List<HRStaffViewModel> Staff { get; set; } = new();
        public bool EditAssignments { get; set; } = false;
    }


    public class AcademicsSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
        public string? SectionID { get; set; }
        public string? ClassIDs { get; set; }
    }
    public class SearchPromotedStudent 
    {
        public int? CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? UserId { get; set; }
        public int? SectionID { get; set; }
        public int? ClassID { get; set; }
    }

    public class TimeTableSearchRequest 
    {
        public int? CompanyID { get; set; }
        public int SessionID { get; set; }
        public int StudentID { get; set; }
        public int? StaffID { get; set; }
        public int? SectionID { get; set; }
        public int? ClassID { get; set; }
    }


    public class StudentDemotionViewModel
    {
        public int StudentResult { get; set; }
        public string Message { get; set; }

        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        public int StudentID { get; set; }
        public int AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string FatherName { get; set; }

        public DateTime? DOB { get; set; }

        public int PreviousClassID { get; set; }
        public string PreviousClassName { get; set; }

        public int PreviousSessionID { get; set; }
        public string PreviousSessionTitle { get; set; }

        public bool CanDemoteSameSession { get; set; }
        public bool CanDemoteCrossSession { get; set; }
    }

    public class DemotionRequest
    {
        public List<int> StudentIDs { get; set; } = new();
        public int TargetSessionID { get; set; }
        public int CompanyId { get; set; }
        public int TargetClassID { get; set; }
        public int TargetSectionID { get; set; }
        public bool DeactivateCurrent { get; set; } = true;
    }

    public class DemoteSummary
    {
        public int Result { get; set; }
        public string Message { get; set; } = "";
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
    }

    public class DemoteStudentResult
    {
        public int StudentID { get; set; }
        public int Result { get; set; }
        public string Message { get; set; } = "";
        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }

    public class DemoteResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public List<DemoteStudentResult> Details { get; set; } = new();
    }

}
