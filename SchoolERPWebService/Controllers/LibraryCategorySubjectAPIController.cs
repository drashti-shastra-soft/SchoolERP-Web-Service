using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LibraryCategorySubjectAPIController : ControllerBase
    {
        private readonly ILibraryCategorySubjectService _service;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Publisher/Index";

        public LibraryCategorySubjectAPIController(ILibraryCategorySubjectService service, IUserMenuPermissionService menuPerm)
        {
            _service = service;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] LibraryCategorySubjectRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _service.UpsertLibraryCategorySubjectAsync(model);
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

                var response = await _service.GetLibraryCategorySubjectByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllLibraryCategorySubjectWithPage")]
        public async Task<IActionResult> GetAllLibraryCategorySubjectWithPage([FromBody] LibraryCategorySubjectSearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<LibraryCategorySubjectModel>>.SuccessResponse(new List<LibraryCategorySubjectModel>()));
                var data = await _service.GetAllLibraryCategorySubjectWithPage(request);
                return Ok(ApiResponse<PagedResult<LibraryCategorySubjectModel>>.SuccessResponse(data));
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
                var (success, message) = _service.DeleteLibraryCategorySubject(ids, userId);
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
                var (success, message) = _service.ToggleLibraryCategorySubjectStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetAllLibraryCategory")]
        public async Task<IActionResult> GetAllLibraryCategory([FromQuery] int companyId, [FromQuery] bool? isActive = null)
        {
            try
            {
                if (companyId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _service.GetAllLibraryCategory(companyId,isActive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetAllLibrarySubject")]
        public async Task<IActionResult> GetAllLibrarySubject([FromQuery] int companyId, [FromQuery] bool? isActive=null)
        {
            try
            {
                if (companyId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _service.GetAllLibrarySubject(companyId,isActive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
