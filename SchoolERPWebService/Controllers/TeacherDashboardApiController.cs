using Microsoft.AspNetCore.Http;
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
    public class TeacherDashboardApiController : ControllerBase
    {
        private readonly ITeacherDashboardService _service;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public TeacherDashboardApiController(ITeacherDashboardService service, ICompanyService companySvc, ISessionService sessionSvc)
        {
            _service = service;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;
        private int GetStaffId() => int.TryParse(User.FindFirstValue("StaffID"), out var id) ? id : 0;

        [HttpGet("GetTimetable")]
        public IActionResult GetTimetable()
        {
            try
            {
                var data = _service.GetTimetable(GetCompanyId(), GetSessionId(), GetStaffId());
                return Ok(ApiResponse<List<TeacherTimetableDto>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetStudents")]
        public IActionResult GetStudents()
        {
            try
            {
                var data = _service.GetStudents(GetCompanyId(), GetSessionId(), GetStaffId());
                return Ok(ApiResponse<List<TeacherStudentDto>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetHomework")]
        public IActionResult GetHomework()
        {
            try
            {
                var data = _service.GetHomework(GetCompanyId(), GetSessionId(), GetStaffId());
                return Ok(ApiResponse<List<TeacherHomeworkDto>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetHWSubmissions")]
        public IActionResult GetHWSubmissions()
        {
            try
            {
                var data = _service.GetHWSubmissions(GetCompanyId(), GetSessionId(), GetStaffId());
                return Ok(ApiResponse<List<TeacherHWSubmissionDto>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetAttendance")]
        public IActionResult GetAttendance()
        {
            try
            {
                var data = _service.GetAttendance(GetCompanyId(), GetSessionId(), GetStaffId());
                return Ok(ApiResponse<List<TeacherAttendanceDto>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


    }
}
