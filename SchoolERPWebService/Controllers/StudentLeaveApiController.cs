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
    public class StudentLeaveApiController : ControllerBase
    {
        private readonly IStudentLeaveService _studentLeaveService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        public StudentLeaveApiController(IStudentLeaveService studentLeaveService,
            ICompanyService companyService,
            ISessionService sessionService)
        {
            _studentLeaveService = studentLeaveService;
            _companyService = companyService;
            _sessionService = sessionService;
        }
        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;
        /// <summary>
        /// Retrieves student leave applications.
        /// </summary>
        /// <param name="classId">Class identifier.</param>
        /// <param name="sectionId">Section identifier.</param>
        /// <param name="status">Leave status.</param>
        /// <returns>List of leave applications.</returns>
        [HttpGet]
        [Route("GetLeaveApplications")]
        public IActionResult GetLeaveApplications(
            int? classId,
            int? sectionId,
            int? status)
        {
            try
            {
                int companyId = GetCompanyId();

                var result = _studentLeaveService.GetLeaveApplications(
                    classId,
                    sectionId,
                    status,
                    companyId);

                return Ok(new ApiResponse<List<StudentLeaveViewModel>>
                {
                    Success = true,
                    Data = result,
                    Message = "Data retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<StudentLeaveViewModel>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates the status of a student leave application.
        /// </summary>
        /// <param name="leaveAppId">The leave application identifier.</param>
        /// <param name="status">The new leave status.</param>
        /// <returns>The operation result.</returns>
        [HttpPost]
        [Route("UpdateLeaveStatus")]
        public IActionResult UpdateLeaveStatus(int leaveAppId, int status)
        {
            try
            {
                int companyId = GetCompanyId();

                int userId = Convert.ToInt32(
                    User.FindFirst("UserID")?.Value ?? "0");

                var result = _studentLeaveService.UpdateLeaveStatus(
                    leaveAppId,
                    status,
                    companyId,
                    userId);

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

        /// <summary>
        /// Creates or updates a student leave application.
        /// </summary>
        /// <param name="request">Leave application details.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("UpsertLeaveApplication")]
        public IActionResult UpsertLeaveApplication(
            [FromBody] StudentLeaveUpsertRequest request)
        {
            try
            {
                if(request.CompanyID==null || request.CompanyID == 0) 
                {
                    request.CompanyID = GetCompanyId();
                }
                int companyId = GetCompanyId();
                int userId = GetUserId();

                var result = _studentLeaveService.UpsertLeaveApplication(
                    request,
                    request.CompanyID,
                    userId);

                return Ok(new { result });
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

        /// <summary>
        /// Downloads the attachment associated with a leave application.
        /// </summary>
        /// <param name="leaveAppId">The leave application identifier.</param>
        /// <returns>Attachment file.</returns>
        [HttpGet]
        [Route("GetLeaveAttachment")]
        public IActionResult GetLeaveAttachment(int leaveAppId)
        {
            try
            {
                int companyId = GetCompanyId();

                var result = _studentLeaveService.GetLeaveAttachment(
                    leaveAppId,
                    companyId);

                if (result.Bytes == null)
                    return NotFound("Attachment not found.");

                return File(
                    result.Bytes,
                    result.ContentType ?? "application/octet-stream",
                    result.FileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetAllLeaveApplicationsWithPage")]
        public async Task<IActionResult> GetAllLeaveApplicationsWithPage([FromBody] StudentLeaveSearchRequest request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.CompanyID == 0 )
                    return Ok(ApiResponse<List<StudentLeaveViewModel>>.SuccessResponse(new List<StudentLeaveViewModel>()));

                var data = await _studentLeaveService.GetAllLeaveApplicationsWithPage(request);
                return Ok(ApiResponse<PagedResult<StudentLeaveViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet]
        [Route("GetLeaveApplicationsById")]
        public IActionResult GetLeaveApplicationsById(
           int? leaveAppId,
           int? companyId)
        {
            try
            {

                var result = _studentLeaveService.GetLeaveApplicationsById(
                    leaveAppId,
                    companyId);

                return Ok(new ApiResponse<StudentLeaveViewModel>
                {
                    Success = true,
                    Data = result,
                    Message = "Data retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<StudentLeaveViewModel>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Updates/replaces the attachment file for an existing student leave application
        /// without affecting the other leave application fields.
        /// </summary>
        /// <param name="req">
        /// Request payload containing the leave application ID and the new attachment
        /// (file data, type, and name).
        /// </param>
        /// <param name="userId">ID of the user performing the update (for audit/history).</param>
        /// <returns>
        /// A tuple indicating whether the update succeeded (<c>success</c>) and a corresponding
        /// user-friendly <c>message</c>.
        /// </returns>
        [HttpPost("UpsertLeaveApplicationAttachmentFile")]
        public IActionResult UpsertLeaveApplicationAttachmentFile([FromBody] LeaveApplicationAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _studentLeaveService.UpsertLeaveApplicationAttachmentFile(req, GetUserId());
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("DeleteLeaveApplication")]
        public IActionResult DeleteLeaveApplication(List<int> ids,int companyId)
        {
            try
            {
                var res = _studentLeaveService.DeleteLeaveApplication(ids, GetUserId(), companyId);
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }            
        }
    }
}
