using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClassApiController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Academics/Class";

        public ClassApiController(IClassService classService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _classService = classService;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false,int? sessionId=null,int? companyId = null, int? staffID = null)
        {
            try
            {
                int userId = GetCurrentUserId();                
                if (sessionId == null)
                {
                    sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (companyId == null)
                {
                    companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<MstClassViewModel>>.SuccessResponse(new List<MstClassViewModel>()));

                var data = _classService.GetAllClasses(companyId.Value, sessionId.Value, includeDeleted, staffID);
                return Ok(ApiResponse<List<MstClassViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _classService.GetClassByID(id);
            if (data == null) return NotFound(ApiResponse<MstClassViewModel>.ErrorResponse("Class not found"));
            return Ok(ApiResponse<MstClassViewModel>.SuccessResponse(data));
        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MstClassUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.ClassID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add classes." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit classes." });
                if (request.CompanyId == null) 
                {
                    request.CompanyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionId == null) 
                {
                    request.SessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));

                var (success, message) = _classService.UpsertClass(request, request.CompanyId.Value, request.SessionId.Value, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success=false, message=ex.Message });
            }
            
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete classes." });

            int userId = GetCurrentUserId();
            var (success, message) = _classService.DeleteClass(ids, userId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetCurrentUserId();
                var (success, message) = _classService.ToggleClassStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }


        [HttpPost("GetAllClassWithPage")]
        public async Task<IActionResult> GetAllClassWithPage([FromBody] ClassSearchRequest request)
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
                    return Ok(ApiResponse<List<MstClassViewModel>>.SuccessResponse(new List<MstClassViewModel>()));

                var data = await _classService.GetAllClassWithPage(request);
                return Ok(ApiResponse<PagedResult<MstClassViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
