using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class StudentDocumentModel
    {
        public int Result { get; set; }
        public string? Message { get; set; }
        public int DocID { get; set; }
        public int StudentID { get; set; }
        public string DocumentTitle { get; set; } = string.Empty;
        public string DocumentContent { get; set; } = string.Empty; // original filename
        public string DocumentPath { get; set; } = string.Empty;    // saved file path/URL
        public string DocumentType { get; set; } = string.Empty;    // filename or extension
        public bool IsDelete { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
