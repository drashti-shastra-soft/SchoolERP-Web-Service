using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolERPWebService.Domain
{
    public class HomeworkViewModel
    {
        public int RESULT { get; set; }
        public string MESSAGE { get; set; }
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        public int HomeworkID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int SectionID { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int SubjectGroupID { get; set; }
        public string SubjectGroupName { get; set; } = string.Empty;
        public int SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public DateTime HomeworkDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? EvaluationDate { get; set; }
       
        public decimal? MaxMarks { get; set; }
        public string? AttachmentPath { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public int? Status { get; set; }
        public int? SubmissionID { get; set; }
        public string? EvaluationByName { get; set; }

        public string? Note { get; set; }
        public string? MarkSobtained{ get; set; }

        public List<HomeworkAttachmentViewModel> AttechmetDocument { get; set; } = new List<HomeworkAttachmentViewModel>();
    }

    public class HomeworkUpsertRequest
    {
        public int HomeworkID { get; set; }

        [Required(ErrorMessage = "Class is required")]
        public int ClassID { get; set; }

        [Required(ErrorMessage = "Section is required")]
        public int SectionID { get; set; }

        [Required(ErrorMessage = "Subject Group is required")]
        public int SubjectGroupID { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public int SubjectID { get; set; }

        [Required(ErrorMessage = "Homework Date is required")]
        public DateTime HomeworkDate { get; set; }

        [Required(ErrorMessage = "Submission Date is required")]
        public DateTime SubmissionDate { get; set; }

        public decimal? MaxMarks { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Attachment will be handled separately via file upload if needed, 
        // or passed as a path string.
        public string? AttachmentPath { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }


    public class HomeworkAttachmentUpsertRequest 
    {
        public int HomeworkID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string AttachmentsJson { get; set; } = "[]";
    }
    public class HomeworkAttachmentViewModel 
    {
        public int AttachmentID { get; set; }
        public int HomeworkID { get; set; }
        public string AttachmentPath { get; set; } = string.Empty;
        public string AttachmentName { get; set; } = string.Empty;
        public string AttachmentType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class HomeWorkAddDetailsViewModel
    {
        public int? HomeworkID { get; set; }
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public HomeworkViewModel Homeworks { get; set; } = new HomeworkViewModel();
        public List<HomeworkAttachmentViewModel> HomeWorkSubmissionAttechment { get; set; } = new List<HomeworkAttachmentViewModel>();
    }
    public class HomeworkPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<HomeworkViewModel> Homeworks { get; set; } = new List<HomeworkViewModel>();
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
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

    public class HomeworkAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public HomeworkViewModel Homeworks { get; set; } = new HomeworkViewModel();
        public HomeworkViewModel EditHomeworks { get; set; } = new HomeworkViewModel();
        public List<MstClassViewModel> Classes { get; set; } = new List<MstClassViewModel>();
        
    }

    public class HomeworkSubmissionUpsertRequest
    {
        public int SubmissionID { get; set; }
        public int HomeworkID { get; set; }
        public int StudentID { get; set; }

        public int CompanyID { get; set; }
        public int SessionID { get; set; }

        public string? Message { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public decimal? MarksObtained { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public int? EvaluatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserID { get; set; }
        public string? IPAddress { get; set; }
    }

    public class HomeworkSubmissionAttachmentUpsertRequest
    {
        public int SubmissionID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string AttachmentsJson { get; set; } = "[]";
    }

    public class HomeworkEvaluationRow
    {
        public int SubmissionID { get; set; }
        public decimal? MarksObtained { get; set; }
        public string? Remark { get; set; }
    }

    public class HomeworkEvaluateRequest
    {
        public int HomeworkID { get; set; }
        public DateTime EvaluationDate { get; set; }
        public int EvaluatedBy { get; set; }
        public List<HomeworkEvaluationRow> Evaluations { get; set; } = new();

        // Filled server-side, not by the client
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }


    public class HomeworkSubmissionEvaluateUpsertRequest
    {
        public int HomeworkID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public DateTime EvaluationDate { get; set; }
        public int? EvaluatedBy { get; set; }
        public string EvaluationsJson { get; set; } = "[]";
        public int? UserID { get; set; }
        public string? IPAddress { get; set; }
    }
    public class HomeworkSubmission 
    {
        public int SubmissionID { get; set; }
        public int? HomeworkID { get; set; }
        public int? StudentID { get; set; }
        public string? Message { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public int? Status { get; set; }
        public string? Remark { get; set; }
        public decimal? MarksObtained { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public int? EvaluatedBy { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
    public class HomeworkEvaluateViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public int? HomeworkID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public HomeworkViewModel Homeworks { get; set; } = new HomeworkViewModel();

        public List<HomeworkSubmission> Submissions { get; set; }= new List<HomeworkSubmission>();
        public List<HomeworkSubmissionListDto> HomeworkSubmissions { get; set; }= new List<HomeworkSubmissionListDto>();
    }

    public class HomeworkSubmissionListDto
    {
        public int Result { get; set; }
        public int SubmissionID { get; set; }
        public int StudentID { get; set; }
        public string? FullName { get; set; }
        public string? FatherName { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public string? Remark { get; set; }
        public string? Message { get; set; }
        public decimal? MarksObtained { get; set; }

        // Raw JSON string as returned by SQL Server's FOR JSON PATH
        public string? Attachments { get; set; }

        // Pagination
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPage { get; set; }

        // Deserialized attachments — not mapped by Dapper directly,
        // populate this after reading (see helper below)
        //[JsonIgnore]
        public List<HomeworkAttachmentDto> AttachmentList { get; set; } = new();
    }


    public class HomeworkSubmissionDashboardDto
    {
        public int Result { get; set; }

        public int SubmissionId { get; set; }
        public int StudentId { get; set; }
        public int HomeworkId { get; set; }

        public string FullName { get; set; }
        public string AdmissionNo { get; set; }
        public string RollNo { get; set; }

        public string HomeworkTitle { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? SubmittedOn { get; set; }

        public decimal? MarksObtained { get; set; }

        public string Remark { get; set; }
        public int? Status { get; set; }
        public string Data { get; set; }

        public string SubmissionStatus { get; set; }
    }
    public class HomeworkAttachmentDto
    {
        [JsonPropertyName("ATTACHMENTID")]
        public int AttachmentID { get; set; }

        [JsonPropertyName("ATTACHMENTNAME")]
        public string? AttachmentName { get; set; }

        [JsonPropertyName("ATTACHMENTPATH")]
        public string? AttachmentPath { get; set; }

        [JsonPropertyName("ATTACHMENTTYPE")]
        public string? AttachmentType { get; set; }
    }

    // Optional wrapper if you want the paged result exposed as a clean object
    // instead of repeating TotalRecords/CurrentPage/PageSize/TotalPage on every row

    public class HomeworkDashboardDto
    {
        public int HomeworkID { get; set; }
        public string Description { get; set; }
        public DateTime HomeworkDate { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string SubjectName { get; set; }
        public string SubjectGroupName { get; set; }
        public bool IsActive { get; set; }
    }
}
