using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public int SessionId { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Guid { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordResult
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public string Guid { get; set; }
        public string Email { get; set; }
        public int UserID { get; set; }
        public int CompanyId { get; set; }
    }

    public class ValidateTokenResult
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public int UserID { get; set; }
        public string Email { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
    }

}
