using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ExamMarksheetDownloadPermissionViewModel
    {
        public int Result { get; set; }
        public int PermissionId { get; set; }

        public int CompanyID { get; set; }

        public int SessionId { get; set; }

        public int? ClassId { get; set; }

        public string? ClassName { get; set; }

        public int? ExamGroupId { get; set; }

        public string? ExamGroupName { get; set; }

        public int? DueMonthNo { get; set; }

        public string? DueMonthUpTo { get; set; }

        public decimal? MaxBalance { get; set; }

        public bool AllowMarksheet { get; set; }

        public bool AllowStripsheet { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int TotalRecords { get; set; }
    }

    public class ExamGroupGetAllRequest
    {
        public int CompanyID { get; set; }
        public int SessionId { get; set; }
        public int? ClassId { get; set; }
        public int? ExamGroupId { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    public class PermissionRequest
    {
        public int PermissionId { get; set; } = 0;

        public int CompanyID { get; set; }

        public int SessionId { get; set; }

        public int ClassId { get; set; }

        public int ExamGroupId { get; set; }

        public byte DueMonthNo { get; set; }

        public decimal MaxBalance { get; set; } = 0;

        /// <summary>
        /// NULL on insert = 0
        /// NULL on update = Keep Current
        /// </summary>
        public bool? AllowMarksheet { get; set; }

        /// <summary>
        /// NULL on insert = 0
        /// NULL on update = Keep Current
        /// </summary>
        public bool? AllowStripsheet { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }
    }

    public class ExamMarksheetDownloadPermissionSetAccessRequest
    {
        /// <summary>
        /// Comma-separated Permission IDs
        /// Example: "1,2,5"
        /// </summary>
        public string Ids { get; set; } = string.Empty;

        /// <summary>
        /// M = MarkSheet
        /// S = StripSheet
        /// </summary>
        public string Target { get; set; } = string.Empty;

        public bool Allow { get; set; }

        public int CompanyID { get; set; }

        public int UserId { get; set; }

        public string? UserName { get; set; }
    }

    public class ExamMarksheetDownloadPermissionSetAccessResponse
    {
        public bool Success { get; set; }

        public string? Message { get; set; }

        public int Id { get; set; }
    }

    public class DeleteRequest
    {
        public List<int> Ids { get; set; } = new();

        public int CompanyId { get; set; }
    }
}
