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
    public class ExamMarksheetDownloadPermissionAPIController : ControllerBase
    {
        private readonly IExamMarksheetDownloadPermissionServices _examPermissionService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/SubjectType/Index";

        public ExamMarksheetDownloadPermissionAPIController(IExamMarksheetDownloadPermissionServices examPermissionService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _examPermissionService = examPermissionService;
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
        public async Task<IActionResult> Upsert([FromBody] PermissionRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.PermissionId <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add subjects." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit subjects." });


                if (request.CompanyID == null || request.CompanyID == 0)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionId == null || request.SessionId == 0)
                {
                    request.SessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionId == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));
                request.UserId = userId;
                var response = _examPermissionService.SavePermissionAsync(request);
                return Ok(new { response });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllExamMarksheetDownloadPermission")]
        public async Task<IActionResult> GetAllExamMarksheetDownloadPermission(ExamGroupGetAllRequest request)
        {
            try
            {
                if (request.CompanyID <= 0 || request.SessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _examPermissionService.GetAllExamMarksheetDownloadPermission(request);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("SetAccess")]
        public async Task<IActionResult> SetAccess(
    [FromBody] ExamMarksheetDownloadPermissionSetAccessRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new
                    {
                        Result = 0,
                        Message = "Invalid request."
                    });
                }

                // Optional validation
                request.Target = request.Target?.Trim().ToUpper();

                if (request.Target != "M" && request.Target != "S")
                {
                    return BadRequest(new
                    {
                        Result = 0,
                        Message = "Invalid target. Use M (MarkSheet) or S (StripSheet)."
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Ids))
                {
                    return BadRequest(new
                    {
                        Result = 0,
                        Message = "Select at least one record."
                    });
                }

                request.UserId = GetCurrentUserId();

                var response = await _examPermissionService
                    .SetAccessAsync(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Result = 0,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(DeleteRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete subjects." });

                int userId = GetCurrentUserId();
                var data = _examPermissionService.Delete(request, userId);
                return Ok(new { data.success, data.message });
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
