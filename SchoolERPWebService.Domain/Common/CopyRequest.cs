using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain.Common
{
    public class CopyRequest
    {
        public int FromCompanyId { get; set; }
        public int FromSessionId { get; set; }
        public int ToCompanyId { get; set; }
        public int ToSessionId { get; set; }
        public int UserID { get; set; }

    }

    public class CopyForCompanyRequest
    {
        public int FromCompanyId { get; set; }
        public int ToCompanyId { get; set; }
        public int UserID { get; set; }
        public string? IPAddress { get; set; }

    }
}
