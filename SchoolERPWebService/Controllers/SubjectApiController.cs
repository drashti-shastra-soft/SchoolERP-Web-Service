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
    public class SubjectApiController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Academics/Subject";

        public SubjectApiController(ISubjectService subjectService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _subjectService = subjectService;
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

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] SubjectSearchRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                
                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstSubjectViewModel>>.SuccessResponse(new List<MstSubjectViewModel>()));
                if (IsSuperAdmin())
                {
                    var datas = await _subjectService.GetAllSubjects(request);
                    return Ok(ApiResponse<PagedResult<MstSubjectViewModel>>.SuccessResponse(datas));
                }
                var data =await _subjectService.GetAllSubjects(request);
                return Ok(ApiResponse<PagedResult<MstSubjectViewModel>>.SuccessResponse(data));
                
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _subjectService.GetSubjectByID(id);
            if (data == null) return NotFound(ApiResponse<MstSubjectViewModel>.ErrorResponse("Subject not found"));
            return Ok(ApiResponse<MstSubjectViewModel>.SuccessResponse(data));
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstSubjectUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.SubjectID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add subjects." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit subjects." });


                if (request.CompanyId == null || request.CompanyId == 0)
                {
                    request.CompanyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionId == null || request.SessionId == 0)
                {
                    request.SessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var (success, message) = _subjectService.UpsertSubject(request, request.CompanyId.Value, request.SessionId.Value, userId);
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
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete subjects." });

            int userId = GetCurrentUserId();
            var data = _subjectService.DeleteSubject(ids, userId);
            return Ok(new { data.success, data.message });
        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to change subject status." });

            int userId = GetCurrentUserId();
            var (success, message) = _subjectService.ToggleSubjectStatus(request);
            return Ok(new { success, message });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        [HttpPost("SubjectsDropdowBind")]
        public async Task<IActionResult> SubjectsDropdowBind([FromBody] DropdowRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (request.CompanyID == null) 
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstSubjectViewModel>>.SuccessResponse(new List<MstSubjectViewModel>()));

                var data = await _subjectService.SubjectsDropdowBinding(request);
                return Ok(ApiResponse<List<Dropdowbinding>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet("GetSubjectGropBySubjectDropdownList")]
        public IActionResult GetSubjectGropBySubjectDropdownList(int subjectGroupId)
        {
            try
            {
                var result = _subjectService.GetSubjectGropBySubjectDropdownList(subjectGroupId);

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


        [HttpPost("GetSubjectListForDashboard")]
        public async Task<IActionResult> GetSubjectListForDashboard([FromBody] SubjectSearchRequest request)
        {
            try
            {
                // ?? Same pattern as your existing GetAll ??????????????????
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;

                if (request.SessionID == null || request.SessionID == 0)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                // Auto-set CompanyID from current user (same as GetAll)
                if (companyId == 0)
                    return Ok(ApiResponse<PagedResult<MstSubjectViewModel>>
                        .SuccessResponse(new PagedResult<MstSubjectViewModel>()));

                request.CompanyID = companyId;  // ? override with real company
                request.PageNumber = 1;
                request.PageSize = 10;

                var data = await _subjectService.GetAllSubjects(request);

                return Ok(ApiResponse<PagedResult<MstSubjectViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}
