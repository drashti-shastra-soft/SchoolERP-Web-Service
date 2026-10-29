using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonPlanAPIController : ControllerBase
    {
        private readonly ILessonPlanService _service;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/LessonPlan/Add";

        public LessonPlanAPIController(ILessonPlanService service, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _service = service;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
        }
        private bool IsSuperAdmin()
        {
            if (User?.Identity?.IsAuthenticated != true) return false;

            return User.Claims.Any(c =>
                (string.Equals(c.Type, ClaimTypes.Role, StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "Role", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserTypeName", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase)) &&
                IsSuperAdminValue(c.Value));
        }
        private static bool IsSuperAdminValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var normalized = value.Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Trim();
            return string.Equals(normalized, "superadmin", StringComparison.OrdinalIgnoreCase);
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
        public IActionResult GetAll(int companyId,int sessionId)
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
                    return Ok(ApiResponse<List<LessonPlanModel>>.SuccessResponse(new List<LessonPlanModel>()));

                var data = _service.GetAll(companyId, sessionId, false);
                return Ok(ApiResponse<List<LessonPlanModel>>.SuccessResponse(data));
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
                if (data == null) return NotFound(ApiResponse<LessonPlanModel>.ErrorResponse("Lesson plan not found"));
                return Ok(ApiResponse<LessonPlanModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] LessonPlanModelRequest request)
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

                var response = await _service.Upsert(request,userId);
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

        [HttpPost("GetAllLessonPlanWithPage")]
        public async Task<IActionResult> GetAllLessonPlanWithPage([FromBody] LessonPlanSearchRequest request)
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
                    return Ok(ApiResponse<List<LessonPlanModel>>.SuccessResponse(new List<LessonPlanModel>()));
                var data = await _service.GetAllLessonPlanWithPage(request);
                return Ok(ApiResponse<PagedResult<LessonPlanModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("GetAllMapLesson")]
        public IActionResult GetAllMapLesson(int companyId, int sessionId, int lessonId)
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
                    return Ok(ApiResponse<List<LessonPlanMap>>.SuccessResponse(new List<LessonPlanMap>()));

                var data = _service.GetAllMapLesson(companyId, sessionId, lessonId);
                return Ok(ApiResponse<List<LessonPlanMap>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("BindLessonDropDwonList")]
        public async Task<IActionResult> BindLessonDropDwonList(LessonDropDwonReq req)
        {
            try
            {
                int userId = GetCurrentUserId();

                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (req.CompanyID == 0 || req.SessionID == 0)
                    return Ok(ApiResponse<List<LessonDropDwonResponse>>.SuccessResponse(new List<LessonDropDwonResponse>()));
                if (IsSuperAdmin())
                {
                    var datas = await _service.BindLessonDropDwonList(req, null);
                    return Ok(ApiResponse<List<LessonDropDwonResponse>>.SuccessResponse(datas));
                }
                var data =await _service.BindLessonDropDwonList(req,userId);
                return Ok(ApiResponse<List<LessonDropDwonResponse>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
