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
    public class SMSTypeFieldAPIController : ControllerBase
    {
        private readonly ISMSTypeFieldService _smsTypeFieldService;
        private readonly IUserMenuPermissionService _menuPerm;
        public SMSTypeFieldAPIController(ISMSTypeFieldService smsTypeFieldService, IUserMenuPermissionService menuPerm)
        {
            _smsTypeFieldService = smsTypeFieldService;
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
        public IActionResult Save([FromBody] SMSFieldRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var spResult = _smsTypeFieldService.UpsertSMSTypeFieldAsync(model); // SpPaymentType


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

                var response = await _smsTypeFieldService.GetSMSTypeFieldByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllSMSTypeFieldWithPage")]
        public async Task<IActionResult> GetAllSMSTypeFieldWithPage([FromBody] SearchRequest request)
        {
            try
            {
                var data = await _smsTypeFieldService.GetAllSMSTypeFieldWithPage(request);
                return Ok(ApiResponse<PagedResult<SMSTypeFieldModel>>.SuccessResponse(data));

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
                var (success, message) = _smsTypeFieldService.DeleteSMSTypeField(ids, userId, IPAddress);
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
                var (success, message) = _smsTypeFieldService.ToggleSMSTypeFieldStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetAllPaymentTypeByIdAsync")]
        public async Task<IActionResult> GetAllPaymentTypeByIdAsync(int smsTypeId, bool? isActive = null)
        {
            try
            {
                var response = await _smsTypeFieldService.GetAllSMSTypeByIdAsync(smsTypeId, isActive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
