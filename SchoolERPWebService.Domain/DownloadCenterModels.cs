using System;
using System.Collections.Generic;

namespace SchoolERPWebService.Domain
{
    public class ContentTypeViewModel
    {
        public int ContentTypeID { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? DisplayLabel { get; set; }
        public bool IsActive { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ContentTypeUpsertRequest
    {
        public int ContentTypeID { get; set; }
        public int CompanyID { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? DisplayLabel { get; set; }
    }

    public class VideoTutorialViewModel
    {
        public int VideoID { get; set; }
        public string? Title { get; set; }
        public string? VideoLink { get; set; }
        public string? Description { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? SharedBy { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyID { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class VideoTutorialUpsertRequest
    {
        public int VideoID { get; set; }
        public string? Title { get; set; }
        public string? VideoLink { get; set; }
        public string? Description { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? CompanyID { get; set; }
    }

    public class ContentViewModel
    {
        public int ContentID { get; set; }
        public string? Title { get; set; }
        public string? ContentTypeName { get; set; }
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FileSize { get; set; }
        public string? UploadBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }

        public int? CompanyID { get; set; }

        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class ContentUploadRequest
    {
        public string? Title { get; set; }
        public int ContentTypeID { get; set; }
        public string? VideoLink { get; set; } // For YouTube
        // File is handled via IFormFile in controller
    }

    public class SharedLinkUpsertRequest
    {
        public string? Title { get; set; }
        public DateTime? ShareDate { get; set; }
        public DateTime? ValidUpto { get; set; }
        public List<int>? ContentIDs { get; set; }
        public int? CompanyID { get; set; }
        public string? SendToType { get; set; }
    }

    public class SharedLinkViewModel
    {
        public int SharedLinkID { get; set; }
        public string? Title { get; set; }
        public Guid ShareToken { get; set; }
        public DateTime? ShareDate { get; set; }
        public DateTime? ValidUpto { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? SharedBy { get; set; }

        public int? CompanyID { get; set; }
        public int Result { get; set; }       // maps to RESULT
        public string Message { get; set; }   // maps to MESSAGE
        public int TOTALRECORDS { get; set; }   // ← TOTALCOUNT
        public int CURRENTPAGE { get; set; }   // ← PAGEINDEX
        public int PageSize { get; set; }   // ← PAGESIZE
        public int TotalPages { get; set; }   // ← TOTALPAGES
    }

    public class SpSharedLinkResultRow
    {
        public bool Success { get; set; }
        public int? SharedLinkID { get; set; }
        public Guid? Token { get; set; }
        public string Message { get; set; }
    }
}
