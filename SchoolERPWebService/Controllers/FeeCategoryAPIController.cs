using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeCategoryAPIController : ControllerBase
    {
        private readonly IFeeCategoryService _feeCategoryService;
        private readonly IUserMenuPermissionService _menuPerm;
        public FeeCategoryAPIController(IFeeCategoryService feeCategoryService, IUserMenuPermissionService menuPerm)
        {
            _feeCategoryService = feeCategoryService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] FeeCategoryRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserId = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _feeCategoryService.UpsertFeeCategoryAsync(model);
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

                var response = await _feeCategoryService.GetFeeCategoryByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllFeeCategoryWithPage")]
        public async Task<IActionResult> GetAllFeeCategoryWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FeeCategoryViewModel>>.SuccessResponse(new List<FeeCategoryViewModel>()));

                var data = await _feeCategoryService.GetAllFeeCategoryWithPage(request);
                return Ok(ApiResponse<PagedResult<FeeCategoryViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int companyId, [FromQuery] int sessionId)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _feeCategoryService.GetAllFeeCategoryAsync(companyId, sessionId, false);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                int userId = GetCurrentUserId();
                string IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _feeCategoryService.DeleteFeeCategory(ids, userId, IPAddress);
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
                int userId = GetCurrentUserId();
                request.DoneBy = userId;
                if (string.IsNullOrWhiteSpace(request.IpAddress))
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _feeCategoryService.ToggleFeeCategoryStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("CopyFeeCategory")]
        public async Task<IActionResult> CopyFeeCategory([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                request.UserID = GetCurrentUserId();
                var result = await _feeCategoryService.CopyFeeCategoryToSession(request);

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

        [HttpGet("GetFeeCategoryDropdownList")]
        public IActionResult GetFeeCategoryDropdownList(int companyId, int sessionID, int? categoryGroupId)
        {
            try
            {
                int userId = GetCurrentUserId();
                var result = _feeCategoryService.GetFeeCategoryDropdownList(companyId, sessionID, userId, categoryGroupId);

                return Ok(new ApiResponse<List<DropdownModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

    }
}
