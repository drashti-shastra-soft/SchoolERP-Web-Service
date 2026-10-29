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
    public class CommunicationAPIController : ControllerBase
    {
        private readonly ICommunicationService _communicationService;
        private readonly IUserMenuPermissionService _menuPerm;
        public CommunicationAPIController(ICommunicationService communicationService, IUserMenuPermissionService menuPerm)
        {
            _communicationService = communicationService;
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
        public async Task<IActionResult> Save([FromBody] CommunicationSaveRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _communicationService.SaveCommunicationAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetTodayBirthdays")]
        public async Task<IActionResult> GetTodayBirthdays(
        [FromBody] BirthdayTodayRequest request)
        {
            try
            {
                var result = await _communicationService.GetTodayBirthdaysAsync(request);
                return Ok(ApiResponse<List<BirthdayTodayResponse>>.SuccessResponse(result));

            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        [HttpPost("GetCommunicationLogWithPage")]
        public async Task<IActionResult> GetCommunicationLogWithPage([FromBody] CommunicationLogRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<CommunicationLogResponse>>.SuccessResponse(new List<CommunicationLogResponse>()));

                var data = await _communicationService.GetCommunicationLogWithPageAsync(request);
                return Ok(ApiResponse<PagedResult<CommunicationLogResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
