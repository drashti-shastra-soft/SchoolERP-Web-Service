using System;
using System.ComponentModel.DataAnnotations;

namespace SchoolERPWebService.Domain
{
    /// <summary>
    /// This class represents the data structure and schema for MstEmailConfigViewModel.
    /// </summary>
    public class MstEmailConfigViewModel
    {
        public int EmailId { get; set; }
        public string? EmailType { get; set; }
        public string? SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string? SMTPUsername { get; set; }
        public string? SMTPPassword { get; set; }
        public string? SslTls { get; set; }
        public string? SMTPAuth { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }

    /// <summary>
    /// This class represents the data structure and schema for MstEmailConfigUpsertRequest.
    /// </summary>
    public class MstEmailConfigUpsertRequest
    {
        public int EmailId { get; set; }

        [Required(ErrorMessage = "Email Engine is required")]
        [StringLength(50)]
        public string? EmailType { get; set; }

        [StringLength(100)]
        public string? SMTPServer { get; set; }

        [StringLength(100)]
        public string? SMTPPort { get; set; }

        [StringLength(100)]
        public string? SMTPUsername { get; set; }

        [StringLength(100)]
        public string? SMTPPassword { get; set; }

        [StringLength(10)]
        public string? SslTls { get; set; }

        [StringLength(10)]
        public string? SMTPAuth { get; set; }

        public bool IsActive { get; set; } = true;
        public int CompanyId { get; set; }
    }

    public class EmailConfigPageViewModel 
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstEmailConfigViewModel mstEmail = new MstEmailConfigViewModel();
    }
}
