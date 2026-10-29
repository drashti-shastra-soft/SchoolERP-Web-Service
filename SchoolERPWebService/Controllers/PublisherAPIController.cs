using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PublisherAPIController : ControllerBase
    {
        private readonly IPublisherService _publisherService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Publisher/Index";

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherAPIController"/> class.
        /// </summary>
        /// <param name="publisherService">The publisher service.</param>
        /// <param name="menuPerm">The menu permission service.</param>
        public PublisherAPIController(IPublisherService publisherService, IUserMenuPermissionService menuPerm)
        {
            _publisherService = publisherService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Gets the current logged-in user ID from User Claims.
        /// </summary>
        /// <returns>The user ID, or 1 as a default value if not found.</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        /// <summary>
        /// Creates or updates a publisher record.
        /// </summary>
        /// <param name="model">The publisher request data.</param>
        /// <returns>An Action result containing the operation status and result.</returns>
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] PublisherRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _publisherService.UpsertPublisherAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message }); 
            }
        }

        /// <summary>
        /// Retrieves a publisher record by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the publisher.</param>
        /// <returns>An Action result containing the publisher details if found.</returns>
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _publisherService.GetPublisherByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }

        }

        /// <summary>
        /// Retrieves a paginated list of publishers based on the search criteria.
        /// </summary>
        /// <param name="request">The search filter and pagination request.</param>
        /// <returns>An Action result containing the paged list of publishers.</returns>
        [HttpPost("GetAllPublishertWithPage")]
        public async Task<IActionResult> GetAllPublishertWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<PublisherModel>>.SuccessResponse(new List<PublisherModel>()));
                var data = await _publisherService.GetAllPublishertWithPage(request);
                return Ok(ApiResponse<PagedResult<PublisherModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Deletes one or more publisher records by ID.
        /// </summary>
        /// <param name="ids">List of publisher identifiers to delete.</param>
        /// <returns>An Action result indicating success status and operation message.</returns>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete classes." });

                int userId = GetCurrentUserId();
                var (success, message) = _publisherService.DeletePublisher(ids, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Toggles the active/inactive status of a publisher record.
        /// </summary>
        /// <param name="request">Status update request details.</param>
        /// <returns>An Action result indicating success status and operation message.</returns>
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetCurrentUserId();
                var (success, message) = _publisherService.TogglePublisherStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves the list of books for the specified company.
        /// Optionally filters the results using a search term.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="searchTerm">Optional search text to filter books.</param>
        /// <returns>A list of books matching the criteria.</returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll(int companyId, bool? includdeleted)
        {
            try
            {
                var result = _publisherService.GetAll(companyId, includdeleted);

                return Ok(new ApiResponse<List<PublisherModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<PublisherModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }

        }

        [HttpPost("CopyLibraryPublisher")]
        public async Task<IActionResult> CopyLibraryPublisher([FromBody] CopyForCompanyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = GetCurrentUserId();
                }
                if (string.IsNullOrWhiteSpace(request.IPAddress))
                    request.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                // ── Execute copy ──────────────────────────────────────────
                var result = await _publisherService.CopyLibraryPublisher(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }
    }
}
