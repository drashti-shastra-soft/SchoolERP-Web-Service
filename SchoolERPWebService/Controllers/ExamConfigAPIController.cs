using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamConfigAPIController : ControllerBase
    {
        private readonly IExamConfigService _examConfigService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/ExamConfig/Index";

        public ExamConfigAPIController(IExamConfigService examConfigService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _examConfigService = examConfigService;
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] ExamConfigRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.ExamConfigID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add subjects." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit subjects." });


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
                request.UserId = userId;
                var (success, message) = _examConfigService.UpsertExamConfig(request);
                return Ok(new { success, message });
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
                    return Ok(new { success = false, message = "You do not have permission to delete subjects." });

                int userId = GetCurrentUserId();
                var data = _examConfigService.DeleteExamConfig(ids, userId, null);
                return Ok(new { data.success, data.message });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("CopyExamConfig")]
        public async Task<IActionResult> CopyExamConfig([FromBody] ExamConfigCopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _examConfigService.CopyExamConfigToSession(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet("GetExamConfigByClass")]
        public async Task<IActionResult> GetExamConfigByClass([FromQuery] int companyId, [FromQuery] int sessionId, [FromQuery] int classID)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _examConfigService.GetAllExamConfigByClassIdAsync(companyId, sessionId, classID);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _examConfigService.GetExamConfigByID(id);
                if (data == null) return NotFound(ApiResponse<ExamConfigViewModel>.ErrorResponse("Subject not found"));
                return Ok(ApiResponse<ExamConfigViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("GetAllExamConfigWithPage")]
        public async Task<IActionResult> GetAllExamConfigWithPage([FromBody] ExamConfigListRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;

                if (request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (companyId == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<ExamConfigViewModel>>.SuccessResponse(new List<ExamConfigViewModel>()));
                if (IsSuperAdmin())
                {
                    var datas = await _examConfigService.GetAllExamConfigWithPage(request);
                    return Ok(ApiResponse<PagedResult<ExamConfigViewModel>>.SuccessResponse(datas));
                }
                var data = await _examConfigService.GetAllExamConfigWithPage(request);
                return Ok(ApiResponse<PagedResult<ExamConfigViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change subject status." });

                int userId = GetCurrentUserId();
                var (success, message) = _examConfigService.ToggleExamStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("GetAllExamSubjectPaperMarksList")]
        public async Task<IActionResult> GetAllExamSubjectPaperMarksList([FromQuery] int companyId, [FromQuery] int sessionId, [FromQuery] int classID, [FromQuery] int sectionId, [FromQuery] int examId)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _examConfigService.GetAllExamSubjectPaperMarksListAsyc(companyId, sessionId, classID, sectionId, examId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("GetAllExamBySubjectIdList")]
        public async Task<IActionResult> GetAllExamBySubjectIdList([FromQuery] int companyId, [FromQuery] int sessionId, [FromQuery] int classID, [FromQuery] int subjectID)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _examConfigService.GetAllExamBySubjectIdListAsyc(companyId, sessionId, classID, subjectID);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
