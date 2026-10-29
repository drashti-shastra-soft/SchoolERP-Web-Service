using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TopicAPIController : ControllerBase
    {
        private readonly ITopicService _service;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Topic/Add";

        public TopicAPIController(ITopicService service, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _service = service;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
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

        [HttpGet("GetAll")]
        public IActionResult GetAll(int companyId, int sessionId)
        {
            try
            {
                int userId = GetCurrentUserId();

                if (companyId == null || companyId == 0)
                {
                    companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (sessionId == null || sessionId == 0)
                {
                    sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<TopicViewModel>>.SuccessResponse(new List<TopicViewModel>()));

                var data = _service.GetAll(companyId, sessionId,userId, false);
                return Ok(ApiResponse<List<TopicViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetByID")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _service.GetByID(id);
                if (data == null) return NotFound(ApiResponse<TopicViewModel>.ErrorResponse("Lesson plan not found"));
                return Ok(ApiResponse<TopicViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] TopicModelRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.LessonId <= 0;
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

                var response = await _service.Upsert(request, userId);
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

        [HttpPost("GetAllTopicWithPage")]
        public async Task<IActionResult> GetAllTopicWithPage([FromBody] TopicSearchRequest request)
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
                    return Ok(ApiResponse<List<TopicViewModel>>.SuccessResponse(new List<TopicViewModel>()));
                var data = await _service.GetAllTopicWithPage(request);
                return Ok(ApiResponse<PagedResult<TopicViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("GetAllMapTopic")]
        public IActionResult GetAllMapTopic(int companyId, int sessionId, int topicId)
        {
            try
            {
                int userId = GetCurrentUserId();

                if (companyId == null || companyId == 0)
                {
                    companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (sessionId == null || sessionId == 0)
                {
                    sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<TopicMap>>.SuccessResponse(new List<TopicMap>()));

                var data = _service.GetAllMapTopic(companyId, sessionId, topicId);
                return Ok(ApiResponse<List<TopicMap>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        [HttpPost("GetAllTopicSyllaBussStatusWithPage")]
        public async Task<IActionResult> GetAllTopicSyllaBussStatusWithPage([FromBody] TopicSearchRequest request)
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
                    return Ok(ApiResponse<List<LessonSyllabusStatusResponse>>.SuccessResponse(new List<LessonSyllabusStatusResponse>()));
                var data = await _service.GetAllTopicSyllaBussStatusWithPage(request);
                return Ok(ApiResponse<PagedResult<LessonSyllabusStatusResponse>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }



        [HttpPost("ToggleTopicCompleteStatus")]
        public async Task<IActionResult> ToggleTopicCompleteStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change status." });

                int userId = GetCurrentUserId();
                var (success, message) = _service.ToggleTopicCompleteStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        [HttpGet("BindTopicDropDwonList")]
        public async Task<IActionResult> BindTopicDropDwonList(int lessonMapId)
        {
            try
            {
                
                var data =await _service.BindTopicDropDwonList(lessonMapId);
                return Ok(ApiResponse<List<TopicDropDwonResponse>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
