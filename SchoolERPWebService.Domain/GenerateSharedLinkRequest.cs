using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    public class GenerateSharedLinkRequest
    {
        public int? CompanyID { get; set; }
        public string Title { get; set; }
        public DateTime ShareDate { get; set; }
        public DateTime ValidUpto { get; set; }
        public List<int> ContentIDs { get; set; } = new();

        // "GROUP" | "CLASS" | "INDIVIDUAL"
        public string SendToType { get; set; }

        public List<int>? RoleIds { get; set; }
        public List<ClassSectionInput>? ClassSections { get; set; }
        public List<IndividualInput>? Individuals { get; set; }
    }

    public class ClassSectionInput
    {
        public int ClassID { get; set; }
        public List<int> SectionIds { get; set; } = new();
    }

    public class IndividualInput
    {
        public int RoleID { get; set; }
        public int ReferenceID { get; set; }
    }

    public class GenerateSharedLinkResult
    {
        public bool Success { get; set; }
        public int? SharedLinkID { get; set; }
        public Guid? Token { get; set; }
        public string Message { get; set; }
    }
}
