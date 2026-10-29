using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlumniEventApiController : ControllerBase
    {
        private readonly IAlumniEventService _alumniEventService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        public AlumniEventApiController(IAlumniEventService alumniEventService,
            ICompanyService companyService,
            ISessionService sessionService)
        {
            _alumniEventService = alumniEventService;
            _companyService = companyService;
            _sessionService = sessionService;
        }
        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;
        /// <summary>
        /// Retrieves alumni events.
        /// </summary>
        /// <param name="searchText">Search text.</param>
        /// <returns>List of alumni events.</returns>
        [HttpGet]
        [Route("GetEvents")]
        public IActionResult GetEvents(string? searchText,int companyId)
        {
            try
            {
                if(companyId==0 || companyId == null) 
                {
                    companyId = GetCompanyId();
                }
                var result = _alumniEventService.GetEvents(
                    searchText,
                    companyId);

                return Ok(new ApiResponse<List<AlumniEventViewModel>>
                {
                    Success = true,
                    Data = result,
                    Message = "Events retrieved successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<AlumniEventViewModel>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Creates or updates an alumni event.
        /// </summary>
        /// <param name="request">Event details.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("UpsertEvent")]
        public  IActionResult UpsertEvent(
            [FromBody] AlumniEventUpsertRequest request)
        {
            try
            {
                if(request.CompanyId==0 || request.CompanyId == null) 
                {
                    request.CompanyId = GetCompanyId();
                }
                int userId =GetUserId();

                var result = _alumniEventService.UpsertEvent(
                    request,
                    request.CompanyId.Value,
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
        /// Deletes an alumni event.
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <returns>Operation result.</returns>
        
        [HttpPost("DeleteEvent")]
        public IActionResult DeleteEvent(List<int> id)
        {
            try
            {
                int companyId = GetCompanyId();
                int userId = GetUserId();

                var result = _alumniEventService.DeleteEvent(
                    id,
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
        /// Activates or deactivates an alumni event.
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <param name="isActive">Active status.</param>
        /// <returns>Operation result.</returns>
        [HttpPost]
        [Route("ToggleEventStatus")]
        public IActionResult ToggleEventStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                int companyId = GetCompanyId();
                int sessionId = GetSessionId();

                var result = _alumniEventService.ToggleEventStatus(request,
                    companyId,
                    sessionId);

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
        /// Retrieves the photo associated with an alumni event.
        /// </summary>
        /// <param name="eventId">Event identifier.</param>
        /// <returns>Event photo file.</returns>
        [HttpGet]
        [Route("GetEventPhoto")]
        public IActionResult GetEventPhoto(int eventId)
        {
            try
            {
                int companyId = GetCompanyId();

                var result = _alumniEventService.GetEventPhoto(
                    eventId,
                    companyId);

                if (result.Bytes == null)
                    return NotFound();

                return File(
                    result.Bytes,
                    result.ContentType ?? "application/octet-stream",
                    result.FileName ?? "EventPhoto");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("GetAllEventsWithPage")]
        public async Task<IActionResult> GetAllEventsWithPage([FromBody] SearchRequest request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }

                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<AlumniEventViewModel>>.SuccessResponse(new List<AlumniEventViewModel>()));

                var data = await _alumniEventService.GetAllEventsWithPage(request);
                return Ok(ApiResponse<PagedResult<AlumniEventViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetEventsById")]
        public IActionResult GetEventsById(int id)
        {
            try
            {
                var result = _alumniEventService.GetEventsById(id);

                return Ok(new ApiResponse<AlumniEventViewModel>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<AlumniEventViewModel>
                {
                    Success = true,
                    Message = ex.Message,
                    Data = null
                });
            }

        }

        [HttpPost("AlumniEventAttachmentFile")]
        public IActionResult AlumniEventAttachmentFile([FromBody] AlumniEventAttachmentUpsertRequest req)
        {
            try
            {
                int userId = GetUserId();
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _alumniEventService.AlumniEventAttachmentFile(req, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }
    }
}
