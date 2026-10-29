using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using Common = SchoolERPWebService.Domain.Common;
using System.Security.Claims;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing school companies and branches through the API.
    /// </summary>
    public class CompanyApiController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Settings";

        public CompanyApiController(ICompanyService companyService, IUserMenuPermissionService menuPerm)
        {
            _companyService = companyService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Gets the full list of all registered school companies from the system.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            int userId = GetCurrentUserId();
            if (IsSuperAdmin())
            {
                var Superdata = _companyService.GetAllCompanies(includeDeleted, null);
                return Ok(Common.ApiResponse<List<MstCompanyViewModel>>.SuccessResponse(Superdata));
            }
            var data = _companyService.GetAllCompanies(includeDeleted, userId);
            return Ok(Common.ApiResponse<List<MstCompanyViewModel>>.SuccessResponse(data));
        }

        [HttpPost("GetAllCompanyWithPage")]
        public async Task<IActionResult> GetAllCompanyWithPage([FromBody] SearchRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (IsSuperAdmin())
                {
                    var result = await _companyService.GetAllCompanyWithPage(request);
                    return Ok(ApiResponse<PagedResult<MstCompanyViewModel>>.SuccessResponse(result));
                }
                var data = await _companyService.GetAllCompanyWithPage(request);
                return Ok(ApiResponse<PagedResult<MstCompanyViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Gets the list of companies assigned to the currently logged-in user.
        /// </summary>
        [HttpGet("GetAssignedCompanies")]
        public IActionResult GetAssignedCompanies()
        {
            if (IsSuperAdmin())
            {
                var allData = _companyService.GetAllCompanies(false);
                return Ok(Common.ApiResponse<List<MstCompanyViewModel>>.SuccessResponse(allData));
            }

            int userId = GetCurrentUserId();
            var data = _companyService.GetCompaniesByUserId(userId);
            return Ok(Common.ApiResponse<List<MstCompanyViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Gets the details of one specific school company using its unique ID number.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _companyService.GetCompanyByID(id);
            if (data == null) return NotFound(Common.ApiResponse<MstCompanyViewModel>.ErrorResponse("Company not found"));
            return Ok(Common.ApiResponse<MstCompanyViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Creates a new school company or updates an existing one with the information provided.
        /// </summary>
        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstCompanyUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int userId = GetCurrentUserId();

            var isCreate = request.CompanyId <= 0;
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add companies." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit companies." });

            var (success, message) = _companyService.UpsertCompany(request, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Permanently removes a school company's record from the system.
        /// </summary>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete companies." });

            int userId = GetCurrentUserId();
            var (success, message) = _companyService.DeleteCompany(id, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Turns a school company's active status on or off.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] Common.StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change status." });

                int userId = GetCurrentUserId();
                var (success, message) = _companyService.ToggleStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        /// <summary>
        /// Sets the chosen school company as the currently 'active' one for the user in the database.
        /// </summary>
        [HttpPost("SetCurrent")]
        public IActionResult SetCurrent([FromBody] SetCurrentCompanyRequest request)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _companyService.UpdateUserCurrentCompany(userId, request.CompanyId);
            if (!success) return BadRequest(Common.ApiResponse<dynamic>.ErrorResponse(message));
            return Ok(Common.ApiResponse<dynamic>.SuccessResponse(null, message));
        }

        /// <summary>
        /// Gets the ID of the school company that the user is currently working in.
        /// </summary>
        [HttpGet("GetUserCurrentCompany")]
        public IActionResult GetUserCurrentCompany()
        {
            int userId = GetCurrentUserId();
            var companyId = _companyService.GetUserCurrentCompany(userId);
            return Ok(Common.ApiResponse<int?>.SuccessResponse(companyId));
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }

        private bool IsSuperAdmin()
        {
            if (User?.Identity?.IsAuthenticated != true) return false;

            return User.Claims.Any(c =>
                (string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "Role", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserTypeName", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase)) &&
                IsSuperAdminValue(c.Value));
        }

        private static bool IsSuperAdminValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(normalized, "superadmin", StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// Uploads a company image (logo, watermark, or login background) and saves its
        /// filename reference against the company record.
        /// </summary>
        [HttpPost("UpdateCompanyImage")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<IActionResult> UpdateCompanyImage([FromForm] int companyId, [FromForm] string imageType,[FromForm] IFormFile file)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit companies." });

                if (companyId <= 0)
                    return Ok(new { success = false, message = "Invalid company." });

                if (file == null || file.Length == 0)
                    return Ok(new { success = false, message = "No file received." });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var allowedTypes = new[] { "PrintLogo", "AdminLogo", "AdminSmallLogo", "WaterMark", "LoginBackground" };

                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                    return Ok(new { success = false, message = "Only JPG, PNG, or WEBP files are allowed." });

                if (!allowedTypes.Contains(imageType))
                    return Ok(new { success = false, message = "Unknown image type." });

                int userId = GetCurrentUserId();

                var (success, message, fileName) = await _companyService.UpdateCompanyImageAsync(companyId, imageType, file, userId);

                if (!success)
                    return Ok(new { success = false, message });

                return Ok(Common.ApiResponse<dynamic>.SuccessResponse(new { fileName }, message));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("SaveCompanyLogos")]
        public async Task<IActionResult> SaveCompanyLogos(CompanyLogoRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit companies." });

                int userId = GetCurrentUserId();

                var (success, message) =  _companyService.SaveCompanyLogoAsync(request);

                if (!success)
                    return Ok(new { success = false, message });

                return Ok(new { success = success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("GetAllCompanyLogoById")]
        public async Task<IActionResult> GetAllCompanyLogoById(int? companyID)
        {
            try
            {
                int userId = GetCurrentUserId();
                
                var data =  _companyService.GetAllCompanyLogoById(companyID);
                return Ok(ApiResponse<List<CompanyLogoResponse>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
