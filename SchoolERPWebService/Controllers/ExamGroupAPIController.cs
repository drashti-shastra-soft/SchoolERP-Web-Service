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
    public class ExamGroupAPIController : ControllerBase
    {
        private readonly IExamGroupService _examGroupService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/ExamGroup/Index";

        public ExamGroupAPIController(IExamGroupService examGroupService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _examGroupService = examGroupService;
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


        [HttpPost("GetAllExamGroupWithPage")]
        public async Task<IActionResult> GetAllExamGroupWithPage([FromBody] ExamListRequest request)
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
                    return Ok(ApiResponse<List<ExamGroupViewModel>>.SuccessResponse(new List<ExamGroupViewModel>()));
                if (IsSuperAdmin())
                {
                    var datas = await _examGroupService.GetAllExamGroupWithPage(request);
                    return Ok(ApiResponse<PagedResult<ExamGroupViewModel>>.SuccessResponse(datas));
                }
                var data = await _examGroupService.GetAllExamGroupWithPage(request);
                return Ok(ApiResponse<PagedResult<ExamGroupViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _examGroupService.GetExamGroupByID(id);
                if (data == null) return NotFound(ApiResponse<ExamGroupViewModel>.ErrorResponse("Subject not found"));
                return Ok(ApiResponse<ExamGroupViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] ExamGroupRequestModel request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.ExamGroupID <= 0;
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
                request.UserID = userId;
                var (success, message) = _examGroupService.UpsertExamGroup(request);
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
                var data = _examGroupService.DeleteExamGroup(ids, userId, null);
                return Ok(new { data.success, data.message });
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
                var (success, message) = _examGroupService.ToggleExamGroupStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("CopyExamGroup")]
        public async Task<IActionResult> CopyExamGroup([FromBody] CopyRequest request)
        {
            try
            {

                // ── Execute copy ──────────────────────────────────────────
                var result = await _examGroupService.CopyExamGroupToSession(request);

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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int companyId, [FromQuery] int sessionId)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _examGroupService.GetAllExamGroupAsync(companyId, sessionId, false);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
