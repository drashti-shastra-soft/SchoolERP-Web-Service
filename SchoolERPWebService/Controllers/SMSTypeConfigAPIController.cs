using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SMSTypeConfigAPIController : ControllerBase
    {
        private readonly ISMSTypeConfigService _smsTypeConfigService;
        private readonly IUserMenuPermissionService _menuPerm;
        public SMSTypeConfigAPIController(ISMSTypeConfigService smsTypeConfigService, IUserMenuPermissionService menuPerm)
        {
            _smsTypeConfigService = smsTypeConfigService;
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

        [HttpPost("SaveBatch")]
        public async Task<IActionResult> SaveBatch([FromBody] SaveSMSBatchRequest req)
        {
            try
            {
                req.UserId = GetCurrentUserId();
                var (result, message) = await _smsTypeConfigService.SaveSMSTypeConfigBatchAsync(
                    req.CompanyId, req.SMSTypeId, req.Fields, req.UserId);
                return Ok(new { success = result, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetByCompanysmsType")]
        public async Task<IActionResult> GetByCompanysmsType([FromQuery] int smsTypeId, [FromQuery] int companyID)
        {
            try
            {
                var response = await _smsTypeConfigService.GetByCompanysmsTypeAsync(smsTypeId, companyID);
                return Ok(new ApiResponse<List<FieldConfigResponse>> { Success = true, Data = response });
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
