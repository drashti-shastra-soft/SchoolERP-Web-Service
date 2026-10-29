namespace SchoolERPWebService.Domain.Common
{
    /// <summary>
    /// Generic base request for any toggle-status operation.
    /// Inherit this and set the Ids property with the correct primary key values.
    /// </summary>
    public class StatusUpdateRequest
    {
        /// <summary>
        /// Comma-separated primary key IDs — e.g. "1,5,7"
        /// Works for any table: UserId, SectionId, ClassId, etc.
        /// </summary>
        public string Ids { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public int DoneBy { get; set; }
        public int? CompanyId { get; set; }

        public string? IpAddress { get; set; }
    }

    public class StatusChangePassRequest
    {
        public int StudentID { get; set; }
        public string Password { get; set; }
        public int? UserID { get; set; }
    }
}
