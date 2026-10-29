using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;
using System.Reflection;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LibraryBudgetAPIController : ControllerBase
    {
        private readonly ILibraryBudgetService _libraryBudgetService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Country/Index";

        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryBudgetAPIController"/> class.
        /// </summary>
        /// <param name="libraryBudgetService">The library budget service.</param>
        /// <param name="menuPerm">The menu permission service.</param>
        public LibraryBudgetAPIController(ILibraryBudgetService libraryBudgetService, IUserMenuPermissionService menuPerm)
        {
            _libraryBudgetService = libraryBudgetService;
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
        /// Creates or updates a library budget.
        /// </summary>
        /// <param name="model">The library budget request data.</param>
        /// <returns>An Action result containing the operation status and result.</returns>
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] LibraryBudgetRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _libraryBudgetService.UpsertLibraryBudgetAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a library budget record by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the budget.</param>
        /// <returns>An Action result containing the budget details if found.</returns>
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _libraryBudgetService.GetLibraryBudgetByIdAsync(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }

        }

        /// <summary>
        /// Retrieves a paginated list of library budgets based on the search criteria.
        /// </summary>
        /// <param name="request">The search filter and pagination request.</param>
        /// <returns>An Action result containing the paged list of library budgets.</returns>
        [HttpPost("GetAllLibraryBudgetWithPage")]
        public async Task<IActionResult> GetAllLibraryBudgetWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<LibraryBudgetModel>>.SuccessResponse(new List<LibraryBudgetModel>()));
                var data = await _libraryBudgetService.GetAllLibraryBudgetWithPage(request);
                return Ok(ApiResponse<PagedResult<LibraryBudgetModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Deletes one or more library budgets by ID.
        /// </summary>
        /// <param name="ids">List of library budget identifiers to delete.</param>
        /// <returns>An Action result indicating success status and operation message.</returns>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete classes." });

                int userId = GetCurrentUserId();
                var (success, message) = _libraryBudgetService.DeleteLibraryBudgety(ids, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Toggles the active/inactive status of a library budget.
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
                var (success, message) = _libraryBudgetService.ToggleLibraryBudgetyStatus(request);
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
                var result = _libraryBudgetService.GetAll(companyId, includdeleted);

                return Ok(new ApiResponse<List<LibraryBudgetModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<BookViewModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }

        }

        [HttpPost("CopyLibraryBudget")]
        public async Task<IActionResult> CopyLibraryBudget([FromBody] CopyForCompanyRequest request)
        {
            try
            {
                
                if (request.UserID == null || request.UserID==0)
                {
                    request.UserID = GetCurrentUserId();
                }
                if (string.IsNullOrWhiteSpace(request.IPAddress))
                    request.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                // ── Execute copy ──────────────────────────────────────────
                var result = await _libraryBudgetService.CopyLibraryBudget(request);

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

        [HttpPost("GetLibraryBudgetChartData")]
        public async Task<IActionResult> GetLibraryBudgetChartData([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<object>.SuccessResponse(null));

                // Reuse your existing service
                var data = await _libraryBudgetService.GetAllLibraryBudgetWithPage(request);

                if (data?.Data == null || !data.Data.Any())
                    return Ok(ApiResponse<object>.SuccessResponse(null));

                // Map to chart format using SP fields: BudgetName, DisplayLabel, Acronym
                var chartData = new
                {
                    labels = data.Data.Select(b => b.BudgetName).ToArray(),
                    display = data.Data.Select(b => b.DisplayLabel).ToArray(),
                    acronym = data.Data.Select(b => b.Acronym).ToArray(),
                    total = data.TotalRecords
                };

                return Ok(ApiResponse<object>.SuccessResponse(chartData));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

    }
}
