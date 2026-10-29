using SchoolERPWebService.Domain.Common;
using System;

namespace SchoolERPWebService.Domain
{
    public class StudentLeaveViewModel
    {
        public int LeaveAppID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public string? StudentName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Reason { get; set; }
        public int Status { get; set; }
        public DateTime ApplyDate { get; set; }
        public string? AttachmentName { get; set; }
        public string? AttachmentType { get; set; }
        public string? Attachment { get; set; }
        public int HasAttachment { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
                                              // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class LeaveStatusUpdateRequest
    {
        public int LeaveAppID { get; set; }
        public int Status { get; set; }
    }

    public class StudentLeaveUpsertRequest
    {
        public int LeaveAppID { get; set; }
        public int StudentID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime ApplyDate { get; set; }
        public string? Reason { get; set; }
        public string? Attachment { get; set; }
        public string? AttachmentType { get; set; }
        public string? AttachmentName { get; set; }
        public int Status { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class StudentLeaveSearchRequest
    {
        public int CompanyID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }
        public int? SectionID { get; set; }
        public int? ClassID { get; set; }
        public int? Status { get; set; }
    }

    public class StudentLeavePageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public List<StudentLeaveViewModel> StudentLeaveModel { get; set; } = new List<StudentLeaveViewModel>();
        public List<MstCompanyViewModel> Companies { get; set; } = new();

        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public int? CompanyId { get; set; }
        public int? ClassId { get; set; }
        public int? SectionID { get; set; }
        public int? Status { get; set; }
    }

    public class StudentLeaveAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        public StudentLeaveViewModel? EditStudentLeave { get; set; } = new StudentLeaveViewModel();
    }

    public class UpsertLeaveApplicationResponse
    {
        public SpLeaveApplicationResult Result { get; set; }
    }
    public class LeaveApplicationAttachmentUpsertRequest
    {
        public int LeaveAppId { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public int? CompanyId { get; set; }
    }
}
