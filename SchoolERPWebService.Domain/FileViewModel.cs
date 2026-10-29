namespace SchoolERPWebService.Domain
{
    public class FileViewModel
    {
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
    }
}
