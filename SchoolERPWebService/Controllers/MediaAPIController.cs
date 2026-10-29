using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MediaAPIController : ControllerBase
    {
        private readonly IMediaService _service;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/ManageLessonPlan/Add";

        public MediaAPIController(IMediaService service, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _service = service;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
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

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] MediaRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();

                var isCreate = request.MediaID <= 0;
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

                var response = await _service.Upsert(request, userId);
                return Ok(ApiResponse<dynamic>.SuccessResponse(response));
                //return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllMediaWithPage")]
        public async Task<IActionResult> GetAllMediaWithPage([FromBody] SearchRequest request)
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
                    return Ok(ApiResponse<List<MediaViewModel>>.SuccessResponse(new List<MediaViewModel>()));
                var data = await _service.GetAllMediaWithPage(request);
                return Ok(ApiResponse<PagedResult<MediaViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

    }
}
