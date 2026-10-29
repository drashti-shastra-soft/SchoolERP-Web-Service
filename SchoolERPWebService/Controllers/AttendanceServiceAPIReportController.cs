using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class AttendanceServiceAPIReportController : ControllerBase
    {
        private readonly IAttendanceServiceReport _attendanceService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;

        public AttendanceServiceAPIReportController(IAttendanceServiceReport attendanceService,
            ICompanyService companyService,
            ISessionService sessionService)
        {
            _companyService = companyService;
            _sessionService = sessionService;
            _attendanceService = attendanceService;
        }

        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpPost("GetAttendanceMonthlyReport")]
        public async Task<IActionResult> GetAttendanceMonthlyReport([FromBody] AttendanceMonthlyReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.ClassId == 0 || request.SectionId == 0 || request.Month == 0 || request.Year == 0)
                    return Ok(ApiResponse<AttendanceMonthlyPagedResult<AttendanceMonthlyResponse>>.SuccessResponse(
                        new AttendanceMonthlyPagedResult<AttendanceMonthlyResponse>()));

                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;

                var data = await _attendanceService.GetAttendanceMonthlyReport(request);
                return Ok(ApiResponse<AttendanceMonthlyPagedResult<AttendanceMonthlyResponse>>.SuccessResponse(data));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("GetAttendanceTypeReport")]
        public async Task<IActionResult> GetAttendanceTypeReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<AttendanceTypeReportResponse>>.SuccessResponse(new List<AttendanceTypeReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _attendanceService.GetAttendanceTypeReport(request);
                return Ok(ApiResponse<PagedResult<AttendanceTypeReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetAttendanceDailyReport")]
        public async Task<IActionResult> GetAttendanceDailyReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<AttendanceDailyReportResponse>>.SuccessResponse(new List<AttendanceDailyReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _attendanceService.GetAttendanceDailyReport(request);
                return Ok(ApiResponse<PagedResult<AttendanceDailyReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetAttendanceDailyWiseReport")]
        public async Task<IActionResult> GetAttendanceDailyWiseReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<AttendanceDayWiseReportResponse>>.SuccessResponse(new List<AttendanceDayWiseReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _attendanceService.GetAttendanceDailyWiseReport(request);
                return Ok(ApiResponse<PagedResult<AttendanceDayWiseReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetAttendanceStaffMonthlyReport")]
        public async Task<IActionResult> GetAttendanceStaffMonthlyReport([FromBody] AttendanceMonthlyReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.ClassId == 0 || request.SectionId == 0 || request.Month == 0 || request.Year == 0)
                    return Ok(ApiResponse<AttendanceMonthlyPagedResult<AttendanceStaffMonthlyResponse>>.SuccessResponse(
                        new AttendanceMonthlyPagedResult<AttendanceStaffMonthlyResponse>()));

                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;

                var data = await _attendanceService.GetAttendanceStaffMonthlyReport(request);
                return Ok(ApiResponse<AttendanceMonthlyPagedResult<AttendanceStaffMonthlyResponse>>.SuccessResponse(data));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("GetStaffAttendanceDayWiseReport")]
        public async Task<IActionResult> GetStaffAttendanceDayWiseReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<StaffAttendanceDayWiseReportResponse>>.SuccessResponse(new List<StaffAttendanceDayWiseReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _attendanceService.GetStaffAttendanceDayWiseReport(request);
                return Ok(ApiResponse<PagedResult<StaffAttendanceDayWiseReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
