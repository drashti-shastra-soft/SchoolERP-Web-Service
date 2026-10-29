using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class SMSTypeConfigModel
    {
        public int ConfigID { get; set; }

        public int CompanyID { get; set; }

        public int SMSTypeId { get; set; }

        public int? FieldId { get; set; }

        public string? FieldValue { get; set; }

        public bool IsActive { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public int? ModifiedBy { get; set; }
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        // Must exist — mapped from TOTALCOUNT column in SP
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class SMSTypeConfigRequest
    {
        public int CompanyID { get; set; }

        public int SMSTypeId { get; set; }

        public List<PaymentTypeConfigFieldRequest> Fields { get; set; }
            = new List<PaymentTypeConfigFieldRequest>();

        public int UserId { get; set; }

        public string? UserName { get; set; }

        public string? IPAddress { get; set; }
    }

    public class SaveSMSBatchRequest
    {
        public int SMSTypeId { get; set; }
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
        public string? UserName { get; set; }
        public List<ConfigFieldItem> Fields { get; set; } = new();
    }
}
