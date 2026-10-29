using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Security.Claims;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RemarkGroupAPIController : ControllerBase
    {
        private readonly IRemarkGroupService _remarkGroupService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/SubjectType/Index";

        public RemarkGroupAPIController(IRemarkGroupService remarkGroupService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _remarkGroupService = remarkGroupService;
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

        [HttpPost("GetAllRemarkGroupWithPage")]
        public async Task<IActionResult> GetAllRemarkGroupWithPage([FromBody] SubjectSearchRequest request)
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
                    return Ok(ApiResponse<List<RemarkGroupViewModel>>.SuccessResponse(new List<RemarkGroupViewModel>()));
                if (IsSuperAdmin())
                {
                    var datas = await _remarkGroupService.GetAllRemarkGroupWithPage(request);
                    return Ok(ApiResponse<PagedResult<RemarkGroupViewModel>>.SuccessResponse(datas));
                }
                var data = await _remarkGroupService.GetAllRemarkGroupWithPage(request);
                return Ok(ApiResponse<PagedResult<RemarkGroupViewModel>>.SuccessResponse(data));

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
                var data = _remarkGroupService.GetRemarkGroupByID(id);
                if (data == null) return NotFound(ApiResponse<RemarkGroupViewModel>.ErrorResponse("Subject not found"));
                return Ok(ApiResponse<RemarkGroupViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] RemarkGroupRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.RemarkGroupID <= 0;
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
                var (success, message) = _remarkGroupService.UpsertRemarkGroup(request);
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
                var data = _remarkGroupService.DeleteRemarkGroup(ids, userId, null);
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
                request.DoneBy = userId;
                var (success, message) = _remarkGroupService.ToggleRemarkGroupStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("GetSubjectsTypeDropdownList")]
        public IActionResult GetSubjectsTypeDropdownList([FromQuery] int companyId, [FromQuery] int sessionId)
        {
            try
            {
                int userId = GetCurrentUserId();
                var result = _remarkGroupService.GetRemarkGroupDropdownList(companyId, sessionId, userId);

                return Ok(new ApiResponse<List<DropdownModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
