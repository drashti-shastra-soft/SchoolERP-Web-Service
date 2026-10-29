using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcademicsApiController : ControllerBase
    {
        private readonly IAcademicsService _svc;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;

        public AcademicsApiController(IAcademicsService svc, ICompanyService companySvc, ISessionService sessionSvc) 
        {
            _svc = svc;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        /// <summary>
        /// Retrieves the timetable for a specific class and section.
        /// Use this to get all scheduled periods for a class-section combination.
        /// </summary>
        [HttpGet("GetTimeTableByClass")]
        public IActionResult GetTimeTableByClass(int classId, int sectionId, int companyId, int sessionId)
        {
            try
            {
                if (companyId == 0 || companyId == null) 
                {
                    companyId = _companySvc.GetUserCurrentCompany(UserId) ?? companyId;
                }
                if(sessionId==0 || sessionId==null)
                {
                    sessionId = _sessionSvc.GetUserCurrentSession(UserId) ?? sessionId;
                }
                    var list = _svc.GetTimeTableByClass(companyId, sessionId, classId, sectionId);
                return Ok(ApiResponse<List<TimeTableViewModel>>.SuccessResponse(list));
            }
            catch (Exception ex) { return Ok(ApiResponse<List<TimeTableViewModel>>.ErrorResponse(ex.Message)); }
        }
        
        /// <summary>
        /// Retrieves the timetable for a specific staff member (teacher).
        /// Use this to get all periods assigned to a staff member, filtered by company and session if provided.
        /// </summary>
        [HttpPost("GetTimeTableByStaff")]
        public IActionResult GetTimeTableByStaff(TimeTableSearchRequest request)
        {
            try
            {
                int userId = UserId;

                if (request.CompanyID == null || request.CompanyID == 0) 
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null || request.SessionID == 0)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<ClassTeacherViewModel>>.SuccessResponse(new List<ClassTeacherViewModel>()));

                var list = _svc.GetTimeTableByStaff(request);
                return Ok(ApiResponse<List<TimeTableViewModel>>.SuccessResponse(list));
            }
            catch (Exception ex) { return Ok(ApiResponse<List<TimeTableViewModel>>.ErrorResponse(ex.Message)); }
        }

        /// <summary>
        /// Creates or updates a timetable slot (period) for a class/section/subject/staff.
        /// Use this to add or edit a timetable entry. Ensures company and session context is set.
        /// </summary>
        [HttpPost("UpsertTimeTable")]
        public IActionResult UpsertTimeTable([FromBody] TimeTableUpsertRequest req)
        {
            try
            {
                if(req.CompanyID==null || req.CompanyID == 0) 
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.UpsertTimeTable(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<List<TimeTableViewModel>>.ErrorResponse(ex.Message));
            }
            
        }

        /// <summary>
        /// Deletes a timetable slot by its unique ID.
        /// Use this to remove a specific period from the timetable.
        /// </summary>
        [HttpPost("DeleteTimeTableSlot/{id}")]
        public IActionResult DeleteTimeTableSlot(int id)
        {
            try
            {
                var (success, message) = _svc.DeleteTimeTableSlot(id, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves all class teacher assignments for the given classes and sections in a session.
        /// Use this to get which staff are assigned as class teachers for multiple classes/sections.
        /// </summary>
        [HttpGet("GetAllClassTeachers")]
        public IActionResult GetAllClassTeachers([FromQuery] List<int> classId, [FromQuery] List<int> sectionId, [FromQuery] int sessionID)
        {
            try
            {
                var list = _svc.GetAllClassTeachers(CompanyId, sessionID, classId,sectionId);
                return Ok(ApiResponse<List<ClassTeacherViewModel>>.SuccessResponse(list));
            }
            catch (Exception ex) { return Ok(ApiResponse<List<ClassTeacherViewModel>>.ErrorResponse(ex.Message)); }
        }

        /// <summary>
        /// Creates or updates a class teacher assignment.
        /// Use this to assign or update staff as class teachers for classes/sections.
        /// </summary>
        [HttpPost("UpsertClassTeacher")]
        public IActionResult UpsertClassTeacher([FromBody] ClassTeacherUpsertRequest req)
        {
            try
            {
                if(req.CompanyID==null && req.CompanyID == 0) 
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null && req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.UpsertClassTeacher(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success=false, message=ex.Message });
            }            
        }

        /// <summary>
        /// Deletes one or more class teacher assignments by their IDs.
        /// Use this to remove class teacher assignments.
        /// </summary>
        [HttpPost("DeleteClassTeacher")]
        public IActionResult DeleteClassTeacher(List<int> ids)
        {
            try
            {
                var res = _svc.DeleteClassTeacher(ids, UserId);
                return Ok(new { res.success, res.message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves all students eligible for promotion in a given class and section.
        /// Use this to get the list of students for the promotion process.
        /// </summary>
        [HttpGet("GetStudentsForPromotion")]
        public IActionResult GetStudentsForPromotion(int classId, int sectionId,int companyId,int sessionId)
        {
            try
            {
                var list = _svc.GetStudentsForPromotion(companyId, sessionId, classId, sectionId);
                return Ok(ApiResponse<List<StudentPromotionViewModel>>.SuccessResponse(list));
            }
            catch (Exception ex) { return Ok(ApiResponse<List<StudentPromotionViewModel>>.ErrorResponse(ex.Message)); }
        }

        /// <summary>
        /// Promotes students to the next class or section.
        /// Use this to finalize and save student promotions.
        /// </summary>
        [HttpPost("PromoteStudents")]
        public IActionResult PromoteStudents([FromBody] PromotionRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.PromoteStudents(req, CompanyId, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        /// <summary>
        /// Retrieves a paginated list of class teacher assignments based on search and filter criteria.
        /// Use this for displaying assignments in a paged grid or table.
        /// </summary>
        [HttpPost("GetAllClassTeachersWithPage")]
        public async Task<IActionResult> GetAllClassTeachersWithPage([FromBody] AcademicsSearchRequest request)
        {
            try
            {
                int userId = UserId;

                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<ClassTeacherViewModel>>.SuccessResponse(new List<ClassTeacherViewModel>()));

                var data = await _svc.GetAllClassTeachersWithPage(request);
                return Ok(ApiResponse<PagedResult<ClassTeacherViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves a paginated list of students eligible for promotion, based on search and filter criteria.
        /// Use this for displaying students in a paged grid or table during the promotion process.
        /// </summary>
        [HttpPost("GetForPromotionPageIndex")]
        public async Task<IActionResult> GetForPromotionPageIndex(SearchPromotedStudent req)
        {
            try
            {
                var list =await _svc.GetForPromotionPageIndex(req);
                return Ok(ApiResponse<PagedResult<StudentPromotionViewModel>>.SuccessResponse(list));
            }
            catch (Exception ex) { return Ok(ApiResponse<PagedResult<StudentPromotionViewModel>>.ErrorResponse(ex.Message)); }
        }

        /// <summary>
        /// Retrieves a timetable slot by its unique ID.
        /// Use this to fetch details of a specific timetable entry for viewing or editing.
        /// </summary>
        [HttpGet("GetTimeTableById")]
        public IActionResult GetTimeTableById(int companyId, int sessionId, int timeTableID)
        {
            try
            {
                var data = _svc.GetTimeTableById(companyId, sessionId, timeTableID);
                if (data == null) return NotFound(ApiResponse<TimeTableViewModel>.ErrorResponse("TimeTable not found"));
                return Ok(ApiResponse<TimeTableViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Toggles the active status (enable/disable) of one or more class teacher assignments.
        /// Use this to activate or deactivate class teacher assignments without deleting them.
        /// </summary>
        [HttpPost("ToggleAcademicsClassTeacherStatus")]
        public async Task<IActionResult> ToggleAcademicsClassTeacherStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                request.DoneBy = UserId;
                var (success, message) = _svc.ToggleAcademicsClassTeacherStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Creates or updates multiple timetable slots in bulk.
        /// Use this to add or edit several timetable entries at once (e.g., for a whole class or section).
        /// </summary>
        [HttpPost("UpsertBulkTimeTable")]
        public IActionResult UpsertBulkTimeTable([FromBody] BulkTimeTableUpsertRequest req)
        {
            try
            {
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.UpsertBulkTimeTable(req, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<List<TimeTableViewModel>>.ErrorResponse(ex.Message));
            }

        }

        /// <summary>
        /// Retrieves a paginated list of students eligible for promotion, based on search and filter criteria.
        /// Use this for displaying students in a paged grid or table during the promotion process.
        /// </summary>
        [HttpPost("GetForDemotionPageIndex")]
        public async Task<IActionResult> GetForDemotionPageIndex(SearchPromotedStudent req)
        {
            try
            {
                var list = await _svc.GetForDemotionPageIndex(req);
                return Ok(ApiResponse<PagedResult<StudentDemotionViewModel>>.SuccessResponse(list));
            }
            catch (Exception ex) { return Ok(ApiResponse<PagedResult<StudentDemotionViewModel>>.ErrorResponse(ex.Message)); }
        }

        [HttpPost("DemoteStudents")]
        public IActionResult DemoteStudents([FromBody] DemotionRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (req.CompanyId == null || req.CompanyId==0)
                {
                    req.CompanyId = CompanyId;
                }                
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, response) = _svc.DemoteStudents(req, UserId, ipAddress);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new DemoteResponse { Success = false, Message = ex.Message });
            }
        }
    }
}
