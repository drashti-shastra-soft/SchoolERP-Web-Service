using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing school organizations and campuses through the API.
    /// </summary>
    public class OrganisationApiController : ControllerBase
    {
        private readonly IOrganisationService _organisationService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Organisation";

        public OrganisationApiController(IOrganisationService organisationService, IUserMenuPermissionService menuPerm)
        {
            _organisationService = organisationService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Gets the full list of all registered organizations or campuses from the system.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _organisationService.GetAllOrganisations(includeDeleted);
            return Ok(ApiResponse<List<OrganisationViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Gets the details of one specific organization using its unique ID number.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _organisationService.GetOrganisationByID(id);
            if (data == null) return NotFound(ApiResponse<OrganisationViewModel>.ErrorResponse("Organisation slice not found"));
            return Ok(ApiResponse<OrganisationViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Saves a new organization's details or updates an existing one with the information provided.
        /// </summary>
        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] OrganisationUpsertRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var isCreate = request.OrganisationID <= 0;
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add organizations." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit organizations." });

            int userId = GetCurrentUserId();
            var (success, message) = _organisationService.UpsertOrganisation(request, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Permanently removes an organization's record from the system.
        /// </summary>
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete organizations." });

            int userId = GetCurrentUserId();
            var (success, message) = _organisationService.DeleteOrganisation(id, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Turns an organization's active status on or off.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus(int id, bool isActive)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change status." });

            int userId = GetCurrentUserId();
            var (success, message) = _organisationService.ToggleOrganisationStatus(id, isActive, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Identifies the actor context from session streams.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1; // Fallback to 1 for demo/superadmin
        }
    }
}
