using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class PagePermissions
    {
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public bool CanExport { get; set; }
        public bool CanImport { get; set; }
        public bool CanView { get; set; }

        // All false by default — safe!
        public static PagePermissions Denied => new PagePermissions();
    }
}
