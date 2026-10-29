using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WeeklyHolidaysSettingAPIController : ControllerBase
    {
        private readonly IWeeklyHolidaysSettingService _weeklyHolidaysSetting;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Academics/Class";

        public WeeklyHolidaysSettingAPIController(IWeeklyHolidaysSettingService weeklyHolidaysSetting, IUserMenuPermissionService menuPerm)
        {
            _weeklyHolidaysSetting = weeklyHolidaysSetting;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
        // ------------------------------------------------------------
        // POST : api/HostelType/Save
        // INSERT OR UPDATE
        // ------------------------------------------------------------
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] WeeklyHolidayBatchRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                //// Capture client IP if not provided
                //if (string.IsNullOrWhiteSpace(model.IPAddress))
                //    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _weeklyHolidaysSetting.UpsertWeeklyHolidaysSettingAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // GET : api/HostelType/GetById/{id}
        // ------------------------------------------------------------
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _weeklyHolidaysSetting.GetWeeklyHolidaysSettingByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // GET : api/HostelType/GetAll?companyId=1&sessionId=1
        // ------------------------------------------------------------
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int companyId)
        {
            try
            {
                if (companyId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _weeklyHolidaysSetting.GetAllWeeklyHolidaysSettingAsync(companyId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ------------------------------------------------------------
        // POST : api/HostelType/GetAllWeeklyHolidaysSettingWithPage
        // ------------------------------------------------------------
        [HttpPost("GetAllWeeklyHolidaysSettingWithPage")]
        public async Task<IActionResult> GetAllWeeklyHolidaysSettingWithPage([FromBody] WeeklyHolidaysSettingSearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<WeeklyHolidaysSettingModel>>.SuccessResponse(new List<WeeklyHolidaysSettingModel>()));

                var data = await _weeklyHolidaysSetting.GetAllWeeklyHolidaysSettingWithPage(request);
                return Ok(ApiResponse<PagedResult<WeeklyHolidaysSettingModel>>.SuccessResponse(data));

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
                var (success, message) = _weeklyHolidaysSetting.DeleteWeeklyHolidaysSetting(ids, userId);
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
                var (success, message) = _weeklyHolidaysSetting.ToggleWeeklyHolidaysSettingStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }



    }
}
