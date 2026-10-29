using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    /// <summary>
    /// This class represents the data structure and schema for MstPaymentMethodViewModel.
    /// </summary>
    public class MstPaymentMethodViewModel
    {
        public int PaymentId { get; set; }
        public int CompanyId { get; set; }
        public int PaymentTypeId { get; set; }
        public string PaymentKey { get; set; } = string.Empty;
        public string PaymentSecret { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }

    /// <summary>
    /// This class represents the data structure and schema for MstPaymentMethodUpsertRequest.
    /// </summary>
    public class MstPaymentMethodUpsertRequest
    {
        public int PaymentId { get; set; }
        public int CompanyId { get; set; }
        public int PaymentTypeId { get; set; }
        public string? PaymentKey { get; set; } = string.Empty;
        public string? PaymentSecret { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// This class represents the data structure and schema for PaymentMethodsPageViewModel.
    /// </summary>
    public class PaymentMethodsPageViewModel
    {
        public PagePermissions Permissions { get; set; } = PagePermissions.Denied;
        public MstPaymentMethodViewModel? PaymentMethod { get; set; }
    }

    public class PaymentTypeInfo
    {
        public int PaymentTypeId { get; set; }
        public string PaymentType { get; set; } = "";
        public string? PaymentCode { get; set; } = "";   // "Stripe" — used to pick the right IPaymentGateway
        public string? LogoUrl { get; set; }
        public string? WebsiteUrl { get; set; }
    }

    public class ResolvedPaymentGatewayConfig
    {
        public PaymentTypeInfo PaymentType { get; set; } = new();
        public Dictionary<string, string> Settings { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
