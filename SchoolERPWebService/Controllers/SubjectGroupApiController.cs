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
    public class SubjectGroupApiController : ControllerBase
    {
        private readonly ISubjectGroupService _service;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Academics/SubjectGroup";

        public SubjectGroupApiController(ISubjectGroupService service, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
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
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false,int? sessionId = null, int? companyId=null)
        {
            int userId = GetCurrentUserId();
           
            if (companyId == null || companyId==0)
            {
                companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
            }
            if (sessionId == null || sessionId==0) 
            {
                sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
            }
            if (companyId == 0 || sessionId == 0)
                return Ok(ApiResponse<List<MstSubjectGroupViewModel>>.SuccessResponse(new List<MstSubjectGroupViewModel>()));
            if (IsSuperAdmin())
            {
                var datas = _service.GetAll(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<MstSubjectGroupViewModel>>.SuccessResponse(datas));
            }
            var data = _service.GetAll(companyId.Value, sessionId.Value, includeDeleted);
            return Ok(ApiResponse<List<MstSubjectGroupViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _service.GetByID(id);
            if (data == null) return NotFound(ApiResponse<MstSubjectGroupViewModel>.ErrorResponse("Subject Group not found"));
            return Ok(ApiResponse<MstSubjectGroupViewModel>.SuccessResponse(data));
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstSubjectGroupUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.SubjectGroupID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add subject groups." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit subject groups." });


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

                var (success, message) = _service.Upsert(request, request.CompanyID, request.SessionID, userId);
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
                    return Ok(new { success = false, message = "You do not have permission to delete subject groups." });

                int userId = GetCurrentUserId();
                var res = _service.Delete(ids, userId);
                return Ok(new { res.success, res.message });
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
                return Ok(new { success = false, message = ex.Message});
            }
            
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }


        [HttpGet("GetAllSubjectByClassandSectionId")]
        public IActionResult GetAllSubjectByClassandSectionId(int companyId, int sessionId, int classId, int sectionId)
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

                var result = _service.GetAllSubjectByClassandSectionId(companyId, sessionId, classId, sectionId);

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

        [HttpPost("GetAllSubjectGroupWithPage")]
        public async Task<IActionResult> GetAllSubjectGroupWithPage([FromBody] SubjectGroupSearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstSubjectGroupViewModel>>.SuccessResponse(new List<MstSubjectGroupViewModel>()));
                var data = await _service.GetAllSubjectGroupWithPage(request);
                return Ok(ApiResponse<PagedResult<MstSubjectGroupViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
