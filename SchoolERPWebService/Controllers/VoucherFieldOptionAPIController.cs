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
    public class VoucherFieldOptionAPIController : ControllerBase
    {
        private readonly IVoucherFieldOptionService _voucherTypeFielOptiondService;
        private readonly IUserMenuPermissionService _menuPerm;
        public VoucherFieldOptionAPIController(IVoucherFieldOptionService voucherTypeFieldOptionService, IUserMenuPermissionService menuPerm)
        {
            _voucherTypeFielOptiondService = voucherTypeFieldOptionService;
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
        public IActionResult Save([FromBody] VoucherFieldOptionRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var spResult = _voucherTypeFielOptiondService.UpsertVoucherFieldOptionAsync(model);


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

                var response = await _voucherTypeFielOptiondService.GetVoucherFieldOptionIDByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllVoucherFieldOptionWithPage")]
        public async Task<IActionResult> GetAllVoucherFieldOptionWithPage([FromBody] SearchRequest request)
        {
            try
            {
                var data = await _voucherTypeFielOptiondService.GetAllVoucherFieldOptionWithPage(request);
                return Ok(ApiResponse<PagedResult<VoucherFieldOptionModel>>.SuccessResponse(data));

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
                var (success, message) = _voucherTypeFielOptiondService.DeleteVoucherFieldOption(ids, userId, IPAddress);
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
                var (success, message) = _voucherTypeFielOptiondService.ToggleVoucherFieldOptionStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetLinkedOptions")]
        public async Task<IActionResult> GetLinkedOptions(
    int fieldId,
    string optionValue,
    int? companyId,
    int? sessionId)
        {
            var result = await _voucherTypeFielOptiondService.GetLinkedOptionsAsync(
                fieldId,
                optionValue,
                companyId,
                sessionId);

            return Ok(result);
        }

    }
}
