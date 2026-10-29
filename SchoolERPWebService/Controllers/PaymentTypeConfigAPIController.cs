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
    public class PaymentTypeConfigAPIController : ControllerBase
    {
        private readonly IPaymentTypeConfigService _paymentTypeConfigService;
        private readonly IUserMenuPermissionService _menuPerm;
        public PaymentTypeConfigAPIController(IPaymentTypeConfigService paymentTypeConfigService, IUserMenuPermissionService menuPerm)
        {
            _paymentTypeConfigService = paymentTypeConfigService;
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
        public IActionResult Save([FromBody] PaymentTypeConfigRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var spResult = _paymentTypeConfigService.UpsertPaymentTypeConfigAsync(model); // SpPaymentType

                var apiResponse = new ApiResponse<UpsertPaymentTypeResponse>
                {
                    Success = spResult?.Result == 1,
                    Message = spResult?.Message,
                    Data = new UpsertPaymentTypeResponse { Result = spResult }
                };

                return Ok(apiResponse);
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

        [HttpPost("SaveBatch")]
        public async Task<IActionResult> SaveBatch([FromBody] SaveBatchRequest req)
        {
            try
            {
                req.UserId= GetCurrentUserId();
                var (result, message) = await _paymentTypeConfigService.SavePaymentTypeConfigBatchAsync(
                    req.CompanyId, req.PaymentTypeId, req.Fields, req.UserId, req.UserName);
                return Ok(new { success = result, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
        [HttpGet("GetByCompanyPaymentType")]
        public async Task<IActionResult> GetByCompanyPaymentType([FromQuery] int paymentTypeId, [FromQuery] int companyID)
        {
            try
            {
                var response = await _paymentTypeConfigService.GetByCompanyPaymentTypeAsync(paymentTypeId, companyID);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
