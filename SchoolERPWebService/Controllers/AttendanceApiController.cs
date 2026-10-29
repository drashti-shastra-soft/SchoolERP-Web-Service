using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceApiController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        public AttendanceApiController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// Retrieves the attendance history of a specific student for the specified year.
        /// Returns monthly attendance summaries and daily attendance status details.
        /// </summary>
        /// <param name="studentId">
        /// The unique identifier of the student.
        /// </param>
        /// <param name="year">
        /// The academic year for which attendance history is required.
        /// </param>
        /// <param name="companyId">
        /// The unique identifier of the company/school.
        /// </param>
        /// <returns>
        /// Returns attendance summaries and daily attendance records.
        /// </returns>
        [HttpGet("GetStudentAttendanceHistory")]
        public IActionResult GetStudentAttendanceHistory(int studentId, int year, int companyId)
        {
            try
            {
                var result = _attendanceService.GetStudentAttendanceHistory(studentId,year,companyId);

                return Ok(new ApiResponse<StudentAttendanceHistoryViewModel>
                {
                    Success = true,
                    Message = "Attendance history retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<StudentAttendanceHistoryViewModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            
        }

        /// <summary>
        /// Retrieves the student attendance list.
        /// </summary>
        [HttpGet]
        [Route("GetStudentAttendanceList")]
        public IActionResult GetStudentAttendanceList(int classId, int sectionId, DateTime date, int companyId)
        {
            try
            {
                var result = _attendanceService.GetStudentAttendanceList(classId, sectionId, date, companyId);

                return Ok(ApiResponse<List<StudentAttendanceViewModel>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<StudentAttendanceViewModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        /// <summary>
        /// Saves attendance records for multiple students.
        /// </summary>
        /// <param name="request">Attendance details.</param>
        /// <returns>Success or failure response.</returns>
        [HttpPost]
        [Route("SaveBulkAttendance")]
        public IActionResult SaveBulkAttendance([FromBody] AttendanceUpsertRequest request)
        {
            try
            {
                //int companyId = Convert.ToInt32(User.FindFirst("CompanyID")?.Value ?? "0");
                int userId = Convert.ToInt32(User.FindFirst("UserID")?.Value ?? "0");

                var result = _attendanceService.SaveBulkAttendance(request,request.CompanyID,userId);

                return Ok(new ApiResponse<object>
                {
                    Success = result.Success,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("GetAllStudentAttendanceWithPage")]
        public async Task<IActionResult> GetAllStudentAttendanceWithPage([FromBody] StudentAttendanceSearchRequest request)
        {
            try
            {                
                var data = await _attendanceService.GetAllStudentAttendanceWithPage(request);
                return Ok(ApiResponse<PagedResult<StudentAttendanceViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }

        }

        [HttpPost("GetAllStudentAttendanceReportWithPage")]
        public async Task<IActionResult> GetAllStudentAttendanceReportWithPage([FromBody] StudentAttendanceSearchRequest request)
        {
            try
            {
                var data = await _attendanceService.GetAllStudentAttendanceReportWithPage(request);
                return Ok(ApiResponse<PagedResult<StudentAttendanceViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }

        }

        [HttpGet("GetStudentAttendanceSummary")]
        public async Task<IActionResult> GetStudentAttendanceSummary(int companyId, int sessionId ,int studentId, int? year, int? status)
        {
            try
            {
                var result = await _attendanceService.GetStudentAttendanceSummary(
                companyId, sessionId, studentId, year, status);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }


        [HttpGet("GetPresentStudentCount")]
        public async Task<IActionResult> GetPresentStudentCount(int companyId, int sessionId, int attendanceStatus, DateTime date)
        {
            try
            {
                var result = await _attendanceService.GetPresentStudentCount(companyId, sessionId, attendanceStatus, date);

                return Ok(new ApiResponse<AttendanceCountViewModel>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("GetPresentStaffCount")]
        public async Task<IActionResult> GetPresentStaffCount(int companyId, int sessionId, DateTime? date = null)
        {
            try
            {
                var result = await _attendanceService.GetPresentStaffCount(companyId, sessionId, date);

                return Ok(new ApiResponse<StaffAttendanceCountViewModel>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

    }
}
