namespace SchoolERP.Domain;

/// <summary>
    /// This class represents the data structure and schema for ErrorViewModel.
    /// </summary>
    public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
