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
    public class FeeTypeAPIController : ControllerBase
    {
        private readonly IFeeTypeService _feeTypeService;
        private readonly IUserMenuPermissionService _menuPerm;
        public FeeTypeAPIController(IFeeTypeService feeTypeService, IUserMenuPermissionService menuPerm)
        {
            _feeTypeService = feeTypeService;
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
        public async Task<IActionResult> Save([FromBody] FeeTypeRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserId = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _feeTypeService.UpsertFeeTypeAsync(model);
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

                var response = await _feeTypeService.GetFeeTypeByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllFeeTypeWithPage")]
        public async Task<IActionResult> GetAllFeeTypeWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FeeTypeViewModel>>.SuccessResponse(new List<FeeTypeViewModel>()));

                var data = await _feeTypeService.GetAllFeeTypeWithPage(request);
                return Ok(ApiResponse<PagedResult<FeeTypeViewModel>>.SuccessResponse(data));

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
                var (success, message) = _feeTypeService.DeleteFeeType(ids, userId, IPAddress);
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
                var (success, message) = _feeTypeService.ToggleFeeTypeStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetFeeTypeDropdownList")]
        public IActionResult GetFeeTypeDropdownList(int companyId, int sessionID)
        {
            try
            {
                int userId = GetCurrentUserId();
                var result = _feeTypeService.GetFeeTypeDropdownList(companyId, sessionID, userId);

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
