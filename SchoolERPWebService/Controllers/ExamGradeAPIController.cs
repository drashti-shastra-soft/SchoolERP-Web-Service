using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ExamGradeAPIController : ControllerBase
    {
        private readonly IExamGradeService _examGradeService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/SubjectType/Index";

        public ExamGradeAPIController(IExamGradeService examGradeService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _examGradeService = examGradeService;
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

        [HttpGet("GetByFilter")]
        public async Task<IActionResult> GetByFilter([FromQuery] int companyId, [FromQuery] int sessionId
            , [FromQuery] int subjectTypeID
            , [FromQuery] int classID)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _examGradeService.GetByFilterAsync(companyId, sessionId, subjectTypeID, classID);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] ExamGradeRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.ExamGradeID <= 0;
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
                var response =await _examGradeService.UpsertExamGradeAsync(request);
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("CopyExamGrade")]
        public async Task<IActionResult> CopyExamGrade([FromBody] ExamGradeCopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var response = await _examGradeService.CopyExamGradeAsync(request);

                if (response.Result !=null && response.Result !=0)
                {
                    return Ok(new { success = response.Result, message = response.Message });
                }
                else
                {
                    return Ok(new { success = false, message = response.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet("GetExamGradeByClassList")]
        public async Task<IActionResult> GetExamGradeByClassList([FromQuery] int companyId, [FromQuery] int sessionId, [FromQuery] int classId)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _examGradeService.GetExamGradeByClassListAsync(companyId, sessionId, classId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
