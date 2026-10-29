using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class StaffUpsertDTO
    {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StaffID { get; set; }
    }
}
