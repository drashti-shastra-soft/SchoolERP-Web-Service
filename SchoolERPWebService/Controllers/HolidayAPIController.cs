using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HolidayAPIController : ControllerBase
    {
        private readonly IHolidayService _holidayService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Academics/Class";

        public HolidayAPIController(IHolidayService holidayService, IUserMenuPermissionService menuPerm)
        {
            _holidayService = holidayService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        // ------------------------------------------------------------
        // POST : api/HolidayType/Save
        // INSERT OR UPDATE
        // ------------------------------------------------------------
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] HolidayRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _holidayService.UpsertHolidayAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // GET : api/HolidayType/GetById/{id}
        // ------------------------------------------------------------
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _holidayService.GetHolidayByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // GET : api/HolidayType/GetAll?companyId=1&sessionId=1
        // ------------------------------------------------------------
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

                var response = await _holidayService.GetAllHolidayAsync(companyId, sessionId, false);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ------------------------------------------------------------
        // POST : api/HolidayType/GetAllHolidayTypeWithPage
        // ------------------------------------------------------------
        [HttpPost("GetAllHolidayWithPage")]
        public async Task<IActionResult> GetAllHolidayWithPage([FromBody] HostelTypeSearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<HolidayModel>>.SuccessResponse(new List<HolidayModel>()));
                var data = await _holidayService.GetAllHolidayWithPage(request);
                return Ok(ApiResponse<PagedResult<HolidayModel>>.SuccessResponse(data));
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
                var (success, message) = _holidayService.DeleteHoliday(ids, userId);
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
                request.DoneBy = userId;
                var (success, message) = _holidayService.ToggleHolidayStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
