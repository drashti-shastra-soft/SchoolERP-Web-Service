using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERPWebService.Domain
{
    internal class CommunicationViewModel
    {
    }
    public class CommunicationSaveRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public int UserID { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public int? TemplateID { get; set; }
        public string? DLTTemplateID { get; set; }

        public string? Attachment { get; set; }
        public string? AttachmentName { get; set; }
        public string? AttachmentType { get; set; }

        public bool IsEmail { get; set; } = true;
        public bool IsSMS { get; set; }
        public bool IsMobileApp { get; set; }

        // NOW or SCHEDULE
        public string SendMode { get; set; } = "NOW";
        public DateTime? ScheduleDateTime { get; set; }

        // JSON containing ROLE, INDIVIDUAL, CLASS or BIRTHDAY recipients
        public string RecipientsJson { get; set; } = string.Empty;
        public string? IPAddress { get; set; } = string.Empty;
    }


    public class CommunicationSaveResponse
    {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;

        public int CommunicationID { get; set; }
        public string? SendMode { get; set; }
        public bool SendNow { get; set; }

        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }
    }

    public class BirthdayTodayResponse
    {
        public string PersonType { get; set; } = string.Empty;
        public int PersonId { get; set; }
        public int? UserID { get; set; }
        public int CompanyID { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Mobile { get; set; }

        public DateTime? DOB { get; set; }
        public int TurningAge { get; set; }

        public string? ClassName { get; set; }

        // Comma-separated values returned by STRING_AGG
        public string? RoleIds { get; set; }
        public string? RoleNames { get; set; }
    }

    public class BirthdayTodayRequest
    {
        public int? CompanyID { get; set; }
        public int? RoleID { get; set; }

        // ALL, STUDENT, STAFF
        public string PersonType { get; set; } = "ALL";

        // Null means today
        public DateTime? OnDate { get; set; }
    }


    public class CommunicationLogRequest
    {
        public int CompanyID { get; set; }
        public int? SessionID { get; set; }

        // LOG or SCHEDULE
        public string LogType { get; set; } = "LOG";

        public string? SearchKeyword { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // TITLE, DESCRIPTION, DATE, SCHEDULEDATE,
        // EMAIL, SMS, GROUP, INDIVIDUAL, CLASS
        public string? SortColumn { get; set; }
        public string? SortDir { get; set; } = "DESC";

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }


    public class CommunicationLogResponse
    {
        // Result
        public int Result { get; set; }
        public string? Message { get; set; }
        public string? TechnicalMessage { get; set; }
        public int? ErrorLine { get; set; }

        // Pagination
        public int TOTALRECORDS { get; set; }
        public int TOTALPAGES { get; set; }
        public int CURRENTPAGE { get; set; }
        public int PAGESIZE { get; set; }

        // Communication details
        public int CommunicationId { get; set; }
        public int CompanyID { get; set; }
        public int? SessionId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }

        public DateTime? Date { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string? SendMode { get; set; }
        public string? Status { get; set; }
        public DateTime? SentOn { get; set; }

        // Channels
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public bool MobileApp { get; set; }

        // Recipient categories
        public bool Group { get; set; }
        public bool Individual { get; set; }
        public bool Class { get; set; }
        public bool Birthday { get; set; }

        public int? CreatedBy { get; set; }

        // Delivery summary
        public int TotalMessages { get; set; }
        public int SentCount { get; set; }
        public int FailedCount { get; set; }
        public int PendingCount { get; set; }
    }

}
