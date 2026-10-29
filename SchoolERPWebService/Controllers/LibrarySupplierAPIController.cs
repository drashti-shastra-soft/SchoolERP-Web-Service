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
    public class LibrarySupplierAPIController : ControllerBase
    {
        private readonly ILibrarySupplierService _supplieryService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/LibraryCategory/Index";

        public LibrarySupplierAPIController(ILibrarySupplierService supplierService, IUserMenuPermissionService menuPerm)
        {
            _supplieryService = supplierService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] LibrarySupplierRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _supplieryService.UpsertLibrarySupplierAsync(model);
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

                var response = await _supplieryService.GetLibrarySupplierByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllLibrarySupplierWithPage")]
        public async Task<IActionResult> GetAllLibrarySupplierWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<LibrarySupplierModel>>.SuccessResponse(new List<LibrarySupplierModel>()));
                var data = await _supplieryService.GetAllLibrarySupplierWithPage(request);
                return Ok(ApiResponse<PagedResult<LibrarySupplierModel>>.SuccessResponse(data));
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
                var (success, message) = _supplieryService.DeleteLibrarySupplier(ids, userId);
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
                var (success, message) = _supplieryService.ToggleLibrarySupplierStatus(request);
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
                var result = _supplieryService.GetAll(companyId, includdeleted);

                return Ok(new ApiResponse<List<LibrarySupplierModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<LibrarySupplierModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }

        }

        [HttpPost("CopyLibrarySupplier")]
        public async Task<IActionResult> CopyLibrarySupplier([FromBody] CopyForCompanyRequest request)
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
                var result = await _supplieryService.CopyLibrarySupplier(request);

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
