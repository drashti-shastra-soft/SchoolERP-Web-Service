using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaperMarksDetailsAPIController : ControllerBase
    {
        private readonly IPaperMarksDetailsService _paperMarksDetailsService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/PaperMarksDetails/Index";

        public PaperMarksDetailsAPIController(IPaperMarksDetailsService paperMarksDetailsService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _paperMarksDetailsService = paperMarksDetailsService;
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


        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _paperMarksDetailsService.GetPaperMarksDetailsByID(id);
                if (data == null) return NotFound(ApiResponse<PaperMarksDetailsViewModel>.ErrorResponse("Subject not found"));
                return Ok(ApiResponse<PaperMarksDetailsViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] PaperMarksDetailsSaveRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                //var isCreate = request.p <= 0;
                //if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                //    return Ok(new { success = false, message = "You do not have permission to add subjects." });
                //if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                //    return Ok(new { success = false, message = "You do not have permission to edit subjects." });


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
                var (success, message) = _paperMarksDetailsService.UpsertPaperMarksDetails(request);
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
                var data = _paperMarksDetailsService.DeletePaperMarksDetails(ids, userId, null);
                return Ok(new { data.success, data.message });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpPost("CopyPaperMarksDetails")]
        public async Task<IActionResult> CopyPaperMarksDetails([FromBody] PaperMarksDetailsCopyRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                //var isCreate = request.RemarkGroupID <= 0;
                //if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                //    return Ok(new { success = false, message = "You do not have permission to add subjects." });
                //if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                //    return Ok(new { success = false, message = "You do not have permission to edit subjects." });


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
                var (success, message) = _paperMarksDetailsService.CopyPaperMarksDetails(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
