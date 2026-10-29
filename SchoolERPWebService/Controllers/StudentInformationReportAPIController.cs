using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class StudentInformationReportAPIController : ControllerBase
    {
        private readonly IStudentInformationReportService _studentService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IAttendanceService _attendanceService;

        public StudentInformationReportAPIController(IStudentInformationReportService studentService,
            ICompanyService companyService,
            ISessionService sessionService,
            IAttendanceService attendanceService)
        {
            _studentService = studentService;
            _companyService = companyService;
            _sessionService = sessionService;
            _attendanceService = attendanceService;
        }

        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpPost("GetStudentReport")]
        public async Task<IActionResult> GetStudentReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<StudentInformationReportModel>>.SuccessResponse(new List<StudentInformationReportModel>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetStudentReport(request);
                return Ok(ApiResponse<PagedResult<StudentInformationReportModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GETStudentCountReport")]
        public async Task<IActionResult> GETStudentCountReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<StudentCountReportResponse>>.SuccessResponse(new List<StudentCountReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GETStudentCountReport(request);
                return Ok(ApiResponse<PagedResult<StudentCountReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GETStudentGuardianReport")]
        public async Task<IActionResult> GETStudentGuardianReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<GuardianReportResponse>>.SuccessResponse(new List<GuardianReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GETStudentGuardianReport(request);
                return Ok(ApiResponse<PagedResult<GuardianReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost("GETLoginCredentialReport")]
        public async Task<IActionResult> GETLoginCredentialReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<LoginCredentialReportResponse>>.SuccessResponse(new List<LoginCredentialReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GETLoginCredentialReport(request);
                return Ok(ApiResponse<PagedResult<LoginCredentialReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GETAdmissionReport")]
        public async Task<IActionResult> GETAdmissionReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<AdmissionReportResponse>>.SuccessResponse(new List<AdmissionReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GETAdmissionReport(request);
                return Ok(ApiResponse<PagedResult<AdmissionReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetStudentGenderRatioReportReport")]
        public async Task<IActionResult> GetStudentGenderRatioReportReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<StudentGenderRatioReportResponse>>.SuccessResponse(new List<StudentGenderRatioReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetStudentGenderRatioReportReport(request);
                return Ok(ApiResponse<PagedResult<StudentGenderRatioReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetClassSubjectReport")]
        public async Task<IActionResult> GetClassSubjectReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<TimeTableClassSubjectReportResponse>>.SuccessResponse(new List<TimeTableClassSubjectReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetClassSubjectReport(request);
                return Ok(ApiResponse<PagedResult<TimeTableClassSubjectReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetStudentTeacherRatioReport")]
        public async Task<IActionResult> GetStudentTeacherRatioReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<StudentTeacherRatioReportResponse>>.SuccessResponse(new List<StudentTeacherRatioReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetStudentTeacherRatioReport(request);
                return Ok(ApiResponse<PagedResult<StudentTeacherRatioReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetStudentHostelReport")]
        public async Task<IActionResult> GetStudentHostelReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<StudentHostelReportResponse>>.SuccessResponse(new List<StudentHostelReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetStudentHostelReport(request);
                return Ok(ApiResponse<PagedResult<StudentHostelReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetStaffReport")]
        public async Task<IActionResult> GetStaffReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<StaffReportResponse>>.SuccessResponse(new List<StaffReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetStaffReport(request);
                return Ok(ApiResponse<PagedResult<StaffReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetStockReport")]
        public async Task<IActionResult> GetStockReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<ItemStockReportResponse>>.SuccessResponse(new List<ItemStockReportResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetStockReport(request);
                return Ok(ApiResponse<PagedResult<ItemStockReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetAddItemReport")]
        public async Task<IActionResult> GetAddItemReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<AddItemResponse>>.SuccessResponse(new List<AddItemResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetAddItemReport(request);
                return Ok(ApiResponse<PagedResult<AddItemResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("GetIssueItemReport")]
        public async Task<IActionResult> GetIssueItemReport([FromBody] StudentReportRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<ItemIssueResponse>>.SuccessResponse(new List<ItemIssueResponse>()));
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                request.UserId = currentUserId;
                request.IpAddress = ip;
                var data = await _studentService.GetIssueItemReport(request);
                return Ok(ApiResponse<PagedResult<ItemIssueResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
