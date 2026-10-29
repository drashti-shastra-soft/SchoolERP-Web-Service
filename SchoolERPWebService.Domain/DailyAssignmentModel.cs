using SchoolERPWebService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class DailyAssignmentModel
    {// Result / message / paging (repeated on every row by the SP)
        public int Result { get; set; }
        public string? Message { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }

        // Core assignment fields
        public int? AssignmentID { get; set; }
        public int? StudentID { get; set; }
        public string? StudentName { get; set; }
        public int? CompanyID { get; set; }
        public int? SessionID { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? SubjectGroupID { get; set; }
        public int? SubjectID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string? Remark { get; set; }
        public DateTime? EvaluationDate { get; set; }
        public int? EvaluatedBy { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        // Joined lookup names
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string? SubjectGroupName { get; set; }
        public string? SubjectName { get; set; }
        public string? CreatedByName { get; set; }
        public string? EVALUATEDBYNAME { get; set; }
        public string? CLASSTEACHERNAME { get; set; }

        // Derived status: 1 = Pending, 2 = Evaluated
        public int? Status { get; set; }

        // Raw JSON string from FOR JSON PATH
        public string? AttachmentsJson { get; set; }

        // Populated manually after the query (see helper below)
        [JsonIgnore]
        public List<DailyAssignmentAttachmentDto> AttachmentList { get; set; } = new();

        // Error-path only fields (present when Result = 0 due to an exception)
        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }
    public class DailyAssignmentAttachmentDto
    {
        [JsonPropertyName("ATTACHMENTID")]
        public int AttachmentID { get; set; }

        [JsonPropertyName("ATTACHMENTPATH")]
        public string? AttachmentPath { get; set; }

        [JsonPropertyName("ATTACHMENTNAME")]
        public string? AttachmentName { get; set; }

        [JsonPropertyName("ATTACHMENTTYPE")]
        public string? AttachmentType { get; set; }
    }
    public class AssignmentUpsertRequest
    {
        public int AssignmentID { get; set; }

        public int StudentID { get; set; }

        public int CompanyID { get; set; }

        public int SessionID { get; set; }

        public int ClassID { get; set; }

        public int SectionID { get; set; }

        public int SubjectGroupID { get; set; }

        public int SubjectID { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? AssignmentDate { get; set; }

        public string? Remark { get; set; }

        public DateTime? EvaluationDate { get; set; }

        public int? EvaluatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public int UserID { get; set; }

        public string? IPAddress { get; set; }
    }

    public class AssignmentAttachmentUpsertRequest
    {
        public int AssignmentID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string AttachmentsJson { get; set; } = "[]";
    }

    public class DailyAssignmentSearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }

        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? StudentId { get; set; }
    }

    public class DailyAssignmentPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<DailyAssignmentModel> DailyAssignment { get; set; } = new List<DailyAssignmentModel>();
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
        public int? ClassID { get; set; }
    }

    public class DailyAssignmentAddViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public DailyAssignmentModel DailyAssignment { get; set; } = new DailyAssignmentModel();
        public DailyAssignmentModel EditDailyAssignment { get; set; } = new DailyAssignmentModel();
        public List<DropdownModel> Subject { get; set; } = new List<DropdownModel>();

    }
}
