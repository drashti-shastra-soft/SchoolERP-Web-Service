using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class DashboardApiController : Controller
    {
        private readonly IDashboardApiService _service;

        public DashboardApiController(IDashboardApiService service)
        {
            _service = service;
        }
        // ── HELPERS ───────────────────────────────────────────────
        private int UserId => int.TryParse(User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private string IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

        // ─────────────────────────────────────────────────────────
        // GET  api/DashboardApi
        // Returns all active dashboards
        // ─────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(new ApiResponse<List<DashboardModel>>
            {
                Success = true,
                Message = "Data fetched successfully.",
                Data = data
            });
        }

        // ─────────────────────────────────────────────────────────
        // GET  api/DashboardApi/{id}
        // Returns single dashboard by ID
        // ─────────────────────────────────────────────────────────
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
                return NotFound(new ApiResponse<DashboardModel>
                {
                    Success = false,
                    Message = "Dashboard not found."
                });

            return Ok(new ApiResponse<DashboardModel>
            {
                Success = true,
                Message = "Data fetched successfully.",
                Data = data
            });
        }

        // ─────────────────────────────────────────────────────────
        // POST  api/DashboardApi/Save
        // Insert (DashboardID=0) or Update (DashboardID>0)
        // ─────────────────────────────────────────────────────────
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] DashboardRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid request data."
                });

            var (success, message, dashboardId) =
                await _service.InsertUpdateAsync(request, UserId, IpAddress);

            return Ok(new ApiResponse<object>
            {
                Success = success,
                Message = message,
                Data = new { DashboardID = dashboardId }
            });
        }

        // ─────────────────────────────────────────────────────────
        // POST  api/DashboardApi/ToggleStatus
        // Activate / Deactivate a dashboard
        // ─────────────────────────────────────────────────────────
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            request.DoneBy = UserId;
            request.IpAddress = IpAddress;
            var (success, message) =
                await _service.ToggleStatusAsync(request);

            return Ok(new ApiResponse<object>
            {
                Success = success,
                Message = message
            });
        }

        // ─────────────────────────────────────────────────────────
        // POST  api/DashboardApi/DeleteMultiple
        // Soft-delete one or more dashboards
        // ─────────────────────────────────────────────────────────
        [HttpPost("DeleteMultiple")]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> dashboardIDs)
        {
            if (dashboardIDs == null || !dashboardIDs.Any())
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Please select at least one dashboard to delete."
                });

            var (success, message) =
                await _service.DeleteMultipleAsync(dashboardIDs, UserId, IpAddress);

            return Ok(new ApiResponse<object>
            {
                Success = success,
                Message = message
            });
        }

        // ─────────────────────────────────────────────────────────
        // GET  api/DashboardApi/{id}
        // Returns single dashboard by ID
        // ─────────────────────────────────────────────────────────
        [HttpGet("GetByRoleId/{id}")]
        public async Task<IActionResult> GetByRoleId(int id)
        {
            try
            {
                var data = await _service.GetByRoleIdAsync(id);

                if (data == null)
                    return NotFound(new ApiResponse<DashboardModel>
                    {
                        Success = false,
                        Message = "Dashboard not found."
                    });

                return Ok(new ApiResponse<DashboardModel>
                {
                    Success = true,
                    Message = "Data fetched successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<DashboardModel>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            
        }

        // ─────────────────────────────────────────────────────────
        // GET  api/DashboardApi/{id}
        // Returns single dashboard by ID
        // ─────────────────────────────────────────────────────────
        
        [HttpPost("GetStudentAttendanceCount")]
        public async Task<IActionResult> GetStudentAttendanceCount([FromBody] ReqAttendanceCountModel req)
        {
            try
            {
                req.UserId = UserId;
                var data = await _service.GetStudentAttendanceCount(req);

                if (data == null)
                    return NotFound(new ApiResponse<AttendanceCountModel>
                    {
                        Success = false,
                        Message = "Dashboard not found."
                    });

                return Ok(new ApiResponse<AttendanceCountModel>
                {
                    Success = true,
                    Message = "Data fetched successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<AttendanceCountModel>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new AttendanceCountModel()
                });
            }
            
        }

        [HttpGet("GetUserRoleWiseCount")]
        public async Task<IActionResult> GetUserRoleWiseCount(int companyId, int sessionId)
        {
            try
            {
                var data = await _service.GetUserRoleWiseCount(companyId, sessionId, UserId);

                if (data == null)
                    return NotFound(new ApiResponse<List<UserRoleWiseCountModel>>
                    {
                        Success = false,
                        Message = "Dashboard not found."
                    });

                return Ok(new ApiResponse<List<UserRoleWiseCountModel>>
                {
                    Success = true,
                    Message = "Data fetched successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<List<UserRoleWiseCountModel>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }

        }
    }
}
