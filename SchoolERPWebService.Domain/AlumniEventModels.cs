using System;

namespace SchoolERPWebService.Domain
{
    public class AlumniEventViewModel
    {
        public int EventID { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Location { get; set; }
        public string? EVENTPHOTO { get; set; }
        public string? EventPhotoName { get; set; }
        public string? EventPhotoType { get; set; }
        public int HasPhoto { get; set; }
        public int EventFor { get; set; }
        public int? SessionID { get; set; }
        public int? ClassID { get; set; }
        public string? SectionIDs { get; set; }
        public string? SessionName { get; set; }
        public string? ClassName { get; set; }
        public string? SectionNames { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }

        public int Result { get; set; }
        public string? Message { get; set; }
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class AlumniEventUpsertRequest
    {
        public int EventID { get; set; }
        public string? EventTitle { get; set; }
        public string? EventDescription { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Location { get; set; }
        public byte[]? EventPhoto { get; set; }
        public string? EventPhotoType { get; set; }
        public string? EventPhotoName { get; set; }
        public int EventFor { get; set; }
        public int? SessionID { get; set; }
        public int? ClassID { get; set; }
        public string? SectionIDs { get; set; }
        public int? CompanyId { get; set; }
    }

    public class AlumniEventAttachmentUpsertRequest
    {
        public int EventID { get; set; }
        public string? Attachment { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
    }
}
