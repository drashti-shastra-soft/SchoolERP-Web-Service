using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VoucherTypeFieldAPIController : ControllerBase
    {
        private readonly IVoucherTypeFieldService _voucherTypeFieldService;
        private readonly IUserMenuPermissionService _menuPerm;
        public VoucherTypeFieldAPIController(IVoucherTypeFieldService voucherTypeFieldService, IUserMenuPermissionService menuPerm)
        {
            _voucherTypeFieldService = voucherTypeFieldService;
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
        public IActionResult Save([FromBody] VoucherFieldRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var spResult = _voucherTypeFieldService.UpsertVoucherFieldAsync(model);


                return Ok(spResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UpsertPaymentTypeResponse>
                {
                    Success = false,
                    Message = "Unable to save payment type. Please try again.",
                    Data = null
                });
            }
        }

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _voucherTypeFieldService.GetVoucherTypeFieldByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllVoucherTypeFielddWithPage")]
        public async Task<IActionResult> GetAllVoucherTypeFielddWithPage([FromBody] SearchRequest request)
        {
            try
            {
                var data = await _voucherTypeFieldService.GetAllVoucherTypeFielddWithPage(request);
                return Ok(ApiResponse<PagedResult<VoucherTypeFieldModel>>.SuccessResponse(data));

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
                var (success, message) = _voucherTypeFieldService.DeleteVoucherTypeField(ids, userId, IPAddress);
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
                var (success, message) = _voucherTypeFieldService.ToggleVoucherTypeFieldStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetVoucherTypeFieldDropdownList")]
        public IActionResult GetVoucherTypeFieldDropdownList(int companyID)
        {
            try
            {
                var result = _voucherTypeFieldService.GetVoucherTypeFieldDropdownList(companyID);

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

        [HttpGet("GetVoucherDynamicFields")]
        public async Task<IActionResult> GetVoucherDynamicFields(int companyId,int voucherId,int sessionId)
        {
            try
            {
                var result = await _voucherTypeFieldService.GetVoucherDynamicFieldsAsync(
                companyId,
                voucherId, sessionId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }
    }
}
