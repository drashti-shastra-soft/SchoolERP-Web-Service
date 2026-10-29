using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SchoolERPWebService.Helpers;
using System.Security.Claims;

namespace SchoolERPWebService.TagHelpers
{
    [HtmlTargetElement("button", Attributes = "asp-permission")]
    [HtmlTargetElement("a", Attributes = "asp-permission")]
    public class PermissionTagHelper : TagHelper
    {
        private readonly PermissionHelper _permHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionTagHelper(PermissionHelper permHelper, IHttpContextAccessor httpContextAccessor)
        {
            _permHelper = permHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("asp-permission")]
        public string Permission { get; set; }

        [HtmlAttributeName("asp-permission-url")]
        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return;

            int currentUserId = 0;
            var user = httpContext.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var idClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst("UserId");
                if (idClaim != null) int.TryParse(idClaim.Value, out currentUserId);
            }

            if (currentUserId <= 0)
            {
                output.SuppressOutput();
                return;
            }

            _permHelper.LoadPermissions(currentUserId);

            var url = Url ?? httpContext.Request.Path.Value;
            
            // Handle cases where path might be empty or root
            if (string.IsNullOrEmpty(url) || url == "/")
            {
                // If it's the home page, maybe we don't suppress or we have a default menu key
                // For now, let's assume if no URL is provided and it's root, we allow it or check a specific "Home" permission
            }

            if (!_permHelper.HasPermissionByUrl(url, Permission))
            {
                output.SuppressOutput();
            }
            
            // Remove the attribute from the final HTML
            output.Attributes.RemoveAll("asp-permission");
            output.Attributes.RemoveAll("asp-permission-url");
        }
    }
}
