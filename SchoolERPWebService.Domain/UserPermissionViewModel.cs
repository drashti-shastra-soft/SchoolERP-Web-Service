namespace SchoolERPWebService.Domain
{
    public class UserPermissionViewModel
    {
        public int MenuID { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string MenuURL { get; set; } = string.Empty;
        public string MenuKey { get; set; } = string.Empty;
    }
}
