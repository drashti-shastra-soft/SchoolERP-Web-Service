using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeworkApiController : ControllerBase
    {
        private readonly IHomeworkService _service;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Homework/Add";

        public HomeworkApiController(IHomeworkService service, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _service = service;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<HomeworkViewModel>>.SuccessResponse(new List<HomeworkViewModel>()));

                var data = _service.GetAll(companyId, sessionId, includeDeleted);
                return Ok(ApiResponse<List<HomeworkViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpGet("GetByID")]
        public IActionResult GetByID(int id, int? studentId = null)
        {
            try
            {
                var data = _service.GetByID(id, studentId);
                if (data == null) return NotFound(ApiResponse<HomeworkViewModel>.ErrorResponse("Homework not found"));
                return Ok(ApiResponse<HomeworkViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] HomeworkUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.HomeworkID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add homework." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit homework." });

                
                if (request.CompanyID == null) 
                {
                    request.CompanyID= _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (request.CompanyID == 0 || request.SessionID == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var response =await _service.Upsert(request, request.CompanyID, request.SessionID, userId);
                return Ok(ApiResponse<dynamic>.SuccessResponse(response));
                //return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete homework." });

                int userId = GetCurrentUserId();
                var (success, message) = _service.Delete(ids, userId);
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
                    return Ok(new { success = false, message = "You do not have permission to change status." });

                int userId = GetCurrentUserId();
                var (success, message) = _service.ToggleStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        private int GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
            }
            catch (Exception ex)
            {
                return 0;
            }
            
        }


        [HttpPost("GetAllHomeWorkWithPage")]
        public async Task<IActionResult> GetAllHomeWorkWithPage([FromBody] SearchRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (request.CompanyID == null || request.CompanyID == 0)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null || request.SessionID==0)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<HomeworkViewModel>>.SuccessResponse(new List<HomeworkViewModel>()));
                var data = await _service.GetAllHomeWorkWithPage(request);
                return Ok(ApiResponse<PagedResult<HomeworkViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message});
            }
        }

        [HttpPost("UpsertAttachment")]
        public async Task<IActionResult> UpsertAttachment([FromBody] HomeworkAttachmentUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.HomeworkID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add homework." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit homework." });


                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (request.CompanyID == 0 || request.SessionID == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var response =await _service.UpsertAttachment(request,userId);
                return Ok(ApiResponse<dynamic>.SuccessResponse(response));
                //return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetAllHomeWorkAttechmentById")]
        public IActionResult GetAllHomeWorkAttechmentById(int homeWorkId)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<HomeworkAttachmentViewModel>>.SuccessResponse(new List<HomeworkAttachmentViewModel>()));

                var data = _service.GetAllHomeWorkAttechmentById(homeWorkId, userId);
                return Ok(ApiResponse<List<HomeworkAttachmentViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        [HttpPost("UpsertSubmission")]
        public async Task<IActionResult> UpsertSubmission([FromBody] HomeworkSubmissionUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.HomeworkID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add homework." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit homework." });


                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null || request.SessionID==0)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (request.CompanyID == 0 || request.SessionID == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var response = await _service.UpsertSubmission(request, userId);
                return Ok(ApiResponse<dynamic>.SuccessResponse(response));
                //return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("UpsertSubmissionAttachment")]
        public async Task<IActionResult> UpsertSubmissionAttachment([FromBody] HomeworkSubmissionAttachmentUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.SubmissionID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add homework." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit homework." });


                if (request.CompanyID == null || request.CompanyID == 0)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null || request.SessionID==0)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (request.CompanyID == 0 || request.SessionID == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var response = await _service.UpsertSubmissionAttachment(request, userId);
                return Ok(ApiResponse<dynamic>.SuccessResponse(response));
                //return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllHomeWorkSubmissionWithPage")]
        public async Task<IActionResult> GetAllHomeWorkSubmissionWithPage([FromBody] SearchRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (request.CompanyID == null || request.CompanyID == 0)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null || request.SessionID == 0)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<HomeworkSubmissionListDto>>.SuccessResponse(new List<HomeworkSubmissionListDto>()));
                var data = await _service.GetAllHomeWorkSubmissionWithPage(request);
                return Ok(ApiResponse<PagedResult<HomeworkSubmissionListDto>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UpsertEvaluateHomework")]
        public async Task<IActionResult> UpsertEvaluateHomework([FromBody] HomeworkSubmissionEvaluateUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.HomeworkID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add homework." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit homework." });


                if (request.CompanyID == null || request.CompanyID == 0)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null || request.SessionID == 0)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (request.CompanyID == 0 || request.SessionID == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var response = await _service.UpsertEvaluateHomework(request, userId);
                return Ok(ApiResponse<dynamic>.SuccessResponse(response));
                //return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetAllHomeWorkSubmissionAttechmentById")]
        public IActionResult GetAllHomeWorkSubmissionAttechmentById(int submissionID)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<HomeworkAttachmentViewModel>>.SuccessResponse(new List<HomeworkAttachmentViewModel>()));

                var data = _service.GetAllHomeWorkSubmissionAttechmentById(submissionID, userId);
                return Ok(ApiResponse<List<HomeworkAttachmentViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        [HttpGet("GetHomeworkSubmissionDashboard")]
        public async Task<IActionResult> GetHomeworkSubmissionDashboard()
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<HomeworkSubmissionDashboardDto>>.SuccessResponse(new List<HomeworkSubmissionDashboardDto>()));
                
                int? studentId = null;
                var studentIdClaim = User.FindFirstValue("StudentID");
                if (!string.IsNullOrEmpty(studentIdClaim) && int.TryParse(studentIdClaim, out var sid))
                    studentId = sid;

                var data = await _service.GetHomeworkSubmissionDashboard(companyId, sessionId);

                return Ok(ApiResponse<List<HomeworkSubmissionDashboardDto>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetHomeworkDashboard")]
        public async Task<IActionResult> GetHomeworkDashboard()
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<HomeworkDashboardDto>>.SuccessResponse(new List<HomeworkDashboardDto>()));

                var data = await Task.Run(() => _service.GetAll(companyId, sessionId));

                return Ok(ApiResponse<List<HomeworkDashboardDto>>.SuccessResponse(
                    data.Select(h => new HomeworkDashboardDto
                    {
                        HomeworkID = h.HomeworkID,
                        Description = h.Description,
                        HomeworkDate = h.HomeworkDate,
                        SubmissionDate = h.SubmissionDate,
                        ClassName = h.ClassName,
                        SectionName = h.SectionName,
                        SubjectName = h.SubjectName,
                        SubjectGroupName = h.SubjectGroupName,
                        IsActive = h.IsActive
                    }).ToList()
                ));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
