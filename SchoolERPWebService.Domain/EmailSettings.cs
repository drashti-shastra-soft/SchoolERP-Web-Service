using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    // =====================================================================
    // STEP 3: Settings class (bound from appsettings.json -> "EmailSettings")
    // =====================================================================
    public class EmailSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;     // STARTTLS on 587 (System.Net.Mail does not support 465)
        public string UserName { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "";

        // throttle - keep below your provider's limit
        public int BatchSize { get; set; } = 20;        // rows claimed per cycle
        public int DelayBetweenMs { get; set; } = 500;  // pause between emails
        public int MaxRetries { get; set; } = 3;
    }
}
