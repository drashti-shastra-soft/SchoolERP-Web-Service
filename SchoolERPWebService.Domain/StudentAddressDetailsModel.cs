using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class StudentAddressDetailsModel
    {
        public int Result { get; set; }
        public string Message { get; set; }

        public int AddressID { get; set; }
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int StudentID { get; set; }

        public string? AddressType { get; set; }
        public string? AddressDetails { get; set; }

        public bool IsDelete { get; set; }

        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
