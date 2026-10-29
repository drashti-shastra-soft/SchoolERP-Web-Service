using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SectionApiController : ControllerBase
    {
        private readonly ISectionService _sectionService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Academics/Section";

        public SectionApiController(ISectionService sectionService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _sectionService = sectionService;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
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
        /// Retrieves all sections for a given company and session.
        /// Use this endpoint to get a list of sections, optionally including deleted ones.
        /// </summary>
        /// <param name="includeDeleted">Whether to include deleted sections.</param>
        /// <param name="sessionId">Session ID to filter sections.</param>
        /// <param name="companyId">Company ID to filter sections.</param>
        /// <returns>List of sections matching the criteria.</returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false, int sessionId=0,int companyId=0)
        {
            try
            {
                int userId = GetCurrentUserId();
                //int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                //int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(new List<MstSectionViewModel>()));
                if (IsSuperAdmin())
                {
                    var datas = _sectionService.GetAllSections(companyId, sessionId, includeDeleted, null);
                    return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(datas));
                }
                var data = _sectionService.GetAllSections(companyId, sessionId, includeDeleted, userId);
                return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all sections assigned to a specific class.
        /// Use this to get sections for a class, optionally filtered by staff.
        /// </summary>
        /// <param name="classId">Class ID to filter sections.</param>
        /// <param name="staffID">Optional staff ID to further filter sections.</param>
        /// <returns>List of sections for the specified class.</returns>
        [HttpGet("GetByClass")]
        public IActionResult GetByClass(int classId, int? staffID = null)
        {
            try
            {
                var data = _sectionService.GetSectionsByClass(classId, staffID);
                return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a section by its unique ID.
        /// Use this to get details of a specific section for viewing or editing.
        /// </summary>
        /// <param name="id">Section ID.</param>
        /// <returns>Section details if found; otherwise, not found response.</returns>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _sectionService.GetSectionByID(id);
                if (data == null) return NotFound(ApiResponse<MstSectionViewModel>.ErrorResponse("Section not found"));
                return Ok(ApiResponse<MstSectionViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }
        
        /// <summary>
        /// Creates a new section or updates an existing section.
        /// Use this endpoint to add a new section or edit an existing one.
        /// Requires appropriate permissions.
        /// </summary>
        /// <param name="request">Section upsert request payload.</param>
        /// <returns>Result indicating success or failure of the operation.</returns>
        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstSectionUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.SectionID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add sections." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit sections." });

                if (request.CompanyId == null || request.CompanyId == 0)
                {
                    request.CompanyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionId == null || request.SessionId == 0)
                {
                    request.SessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (request.CompanyId == 0 || request.SessionId == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var (success, message) = _sectionService.UpsertSection(request, request.CompanyId, request.SessionId, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }
       
        /// <summary>
        /// Deletes one or more sections by their IDs.
        /// Use this to remove sections from the system. Requires delete permission.
        /// </summary>
        /// <param name="ids">List of section IDs to delete.</param>
        /// <returns>Result indicating success or failure of the operation.</returns>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete sections." });

                int userId = GetCurrentUserId();
                var (success, message) = _sectionService.DeleteSection(ids, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }
        
        /// <summary>
        /// Toggles the active status of one or more sections.
        /// Use this to activate or deactivate sections. Requires edit permission.
        /// </summary>
        /// <param name="request">Status update request payload.</param>
        /// <returns>Result indicating success or failure of the operation.</returns>
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change section status." });

                int userId = GetCurrentUserId();
                var (success, message) = _sectionService.ToggleSectionStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message =ex.Message });
            }            
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        /// <summary>
        /// Copies selected sections to a target session.
        /// Use this to duplicate sections from one session to another (e.g., for a new academic year).
        /// Requires add permission.
        /// </summary>
        /// <param name="request">Section copy request payload.</param>
        /// <returns>Result indicating success or failure, with counts of inserted and skipped sections.</returns>
        [HttpPost("CopyToSession")]
        public async Task<IActionResult> CopyToSession([FromBody] SectionCopyRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to copy sections." });

                if (string.IsNullOrWhiteSpace(request.SectionIds))
                    return Ok(new { success = false, message = "Please select at least one section to copy." });

                if (request.TargetSessionId <= 0)
                    return Ok(new { success = false, message = "Please select a target session." });

                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;

                if (companyId == 0)
                    return Ok(new { success = false, message = "Company not found." });

                var (success, message, inserted, skipped) =
                    _sectionService.CopySectionsToSession(
                        request.SectionIds,
                        request.TargetSessionId,
                        companyId,
                        userId);

                return Ok(new { success, message, inserted, skipped });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a paginated list of sections based on search and filter criteria.
        /// Use this for displaying sections in a paged grid or table.
        /// </summary>
        /// <param name="request">Search and pagination parameters.</param>
        /// <returns>Paged result of sections matching the criteria.</returns>
        [HttpPost("GetAllSectionWithPage")]
        public async Task<IActionResult> GetAllSectionWithPage([FromBody] HostelTypeSearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstSectionViewModel>>.SuccessResponse(new List<MstSectionViewModel>()));

                var data = await _sectionService.GetAllSectionWithPage(request);
                return Ok(ApiResponse<PagedResult<MstSectionViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
