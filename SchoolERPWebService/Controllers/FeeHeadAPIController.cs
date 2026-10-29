using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Security.Claims;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeHeadAPIController : ControllerBase
    {
        private readonly IFeeHeadService _feeHeadService;
        private readonly IUserMenuPermissionService _menuPerm;
        public FeeHeadAPIController(IFeeHeadService feeHeadService, IUserMenuPermissionService menuPerm)
        {
            _feeHeadService = feeHeadService;
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
        public async Task<IActionResult> Save([FromBody] FeeHeadRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserId = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _feeHeadService.UpsertFeeHeadAsync(model);
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

                var response = await _feeHeadService.GetFeeHeadByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllFeeHeadWithPage")]
        public async Task<IActionResult> GetAllFeeHeadWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FeeHeadViewModel>>.SuccessResponse(new List<FeeHeadViewModel>()));

                var data = await _feeHeadService.GetAllFeeHeadWithPage(request);
                return Ok(ApiResponse<PagedResult<FeeHeadViewModel>>.SuccessResponse(data));

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
                int userId = GetCurrentUserId();
                string IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _feeHeadService.DeleteFeeHead(ids, userId, IPAddress);
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
                var (success, message) = _feeHeadService.ToggleFeeHeadStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetFeeHeadDropdownList")]
        public IActionResult GetFeeHeadDropdownList(int companyId, int sessionID)
        {
            try
            {
                int userId = GetCurrentUserId();
                var result = _feeHeadService.GetFeeHeadDropdownList(companyId, sessionID, userId);

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
