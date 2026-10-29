using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LibrarySeriesAPIController : ControllerBase
    {
        private readonly ILibrarySeriesService _seriesService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/LibraryCategory/Index";

        public LibrarySeriesAPIController(ILibrarySeriesService seriesService, IUserMenuPermissionService menuPerm)
        {
            _seriesService = seriesService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] LibrarySeriesRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _seriesService.UpsertLibrarySeriesAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _seriesService.GetLibrarySeriesByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllLibrarySeriesWithPage")]
        public async Task<IActionResult> GetAllLibraryCategoryWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<LibrarySeriesModel>>.SuccessResponse(new List<LibrarySeriesModel>()));
                var data = await _seriesService.GetAllLibrarySeriesWithPage(request);
                return Ok(ApiResponse<PagedResult<LibrarySeriesModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete classes." });

                int userId = GetCurrentUserId();
                var (success, message) = _seriesService.DeleteLibrarySeries(ids, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetCurrentUserId();
                var (success, message) = _seriesService.ToggleLibrarySeriesStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(int companyId, bool? includdeleted)
        {
            try
            {
                var result = _seriesService.GetAll(companyId, includdeleted);

                return Ok(new ApiResponse<List<LibrarySeriesModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<LibrarySeriesModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }

        }

        [HttpPost("CopyLibrarySeries")]
        public async Task<IActionResult> CopyLibrarySeries([FromBody] CopyForCompanyRequest request)
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
                var result = await _seriesService.CopyLibrarySeries(request);

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
