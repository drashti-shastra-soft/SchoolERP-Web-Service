namespace SchoolERPWebService.Domain.Common
{
    public class SpResult
    {
        public int Result { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Extra { get; set; } = string.Empty;
        public string TECHNICALMESSAGE { get; set; } = string.Empty;
    }

    public class SpRoomTypeResult
    {
        public int Result { get; set; }        
        public string Message { get; set; } = string.Empty;
        public int RoomTypeID { get; set; }
    }

    public class SpVisitorResult
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? VISITORBOOKID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpPostalDispatch
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? POSTALDISPATCHID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpEvent
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? EventID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpExpense
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? ACCOUNTENTRYID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }

    public class SpAccount
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? AccountID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpPaymentType
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? PAYMENTTYPEID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpSMSType
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? SMSTypeId { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpPostalReceive
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? PostalReceiveID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpItemStockReceive
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? ItemStockId { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpComplaint
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? ComplaintID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
    public class SpLeaveApplicationResult
    {
        public int Result { get; set; }
        public int LeaveAppID { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TECHNICALMESSAGE { get; set; } = string.Empty;
    }

    public class SearchRequest
    {
        public int CompanyID { get; set; }
        public int SessionID { get; set; }
        public string? SearchKeyword { get; set; }
        public string? Mode { get; set; }
        public int? PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? UserId { get; set; }

        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? StudentId { get; set; }
        public int? StaffId { get; set; }
        public int? HomeWorkId { get; set; }
        public int? ContenttypeId { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? SMSTypeId { get; set; }
        public int? VoucherId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SpBooksResult
    {
        public int Result { get; set; }
        public int BookId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TECHNICALMESSAGE { get; set; } = string.Empty;
    }

    public class DropdownModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
    public class DropdownItemCategoryModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Unit { get; set; }
    }
    public class SpStudentTimeline
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? TimelineID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }

    public class SpStaffTimeline
    {
        public int? Result { get; set; }
        public string? Message { get; set; } = string.Empty;
        public int? TimelineID { get; set; }
        public string? TECHNICALMESSAGE { get; set; } = string.Empty;
    }
}
