using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    public class FieldModel
    {
        public int FieldId { get; set; }
        public string BelongsTo { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string FieldValues { get; set; }
        public bool IsSystemField { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public int GridColumn { get; set; }
        public bool OnTable { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class FieldViewModel
    {
        public int FieldId { get; set; }

        [Required(ErrorMessage = "Module selection is required")]
        public string BelongsTo { get; set; }

        [Required(ErrorMessage = "Field label is required")]
        public string FieldName { get; set; }

        [Required(ErrorMessage = "Field type is required")]
        public string FieldType { get; set; }

        public string FieldValues { get; set; }
        public bool IsSystemField { get; set; }
        public bool IsRequired { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
        public int GridColumn { get; set; } = 12;
        public bool OnTable { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }
    public class IDAutoGenSettings
    {
        public int ConfigID { get; set; }
        public string EntityType { get; set; }
        public bool IsEnabled { get; set; }
        public string Prefix { get; set; }
        public int DigitCount { get; set; }
        public int StartNo { get; set; }
        public string FieldsToInclude { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class IDAutoGenRequest
    {
        public string EntityType { get; set; }
        public bool IsEnabled { get; set; }
        public string Prefix { get; set; }
        public int DigitCount { get; set; }
        public int StartNo { get; set; }
        public string[] FieldsToInclude { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

    public class FieldToggleStatusRequest
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }

    public class FieldPageViewModel 
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public List<FieldModel> fieldModels { get; set; }
    }

    public class FieldSearchRequestModel
    {
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
        public bool? IsSystemField { get; set; }
        public string? BelongsTo { get; set; }
        public string? SearchKeyword { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
