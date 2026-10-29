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
    public class NoticeBoardAPIController : ControllerBase
    {
        private readonly INoticeBoardService _noticeBoardService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/NoticeBoardAPI/Index";

        public NoticeBoardAPIController(INoticeBoardService noticeBoardService, IUserMenuPermissionService menuPerm)
        {
            _noticeBoardService = noticeBoardService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        // ------------------------------------------------------------
        // POST : api/NoticeBoardAPI/Save
        // INSERT OR UPDATE
        // ------------------------------------------------------------
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] NoticeBoardRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _noticeBoardService.UpsertNoticeBoardAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // ------------------------------------------------------------
        // GET : api/NoticeBoardAPI/GetById/{id}
        // ------------------------------------------------------------
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _noticeBoardService.GetNoticeBoardByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // POST : api/NoticeBoardAPI/GetAllCountryWithPage
        // ------------------------------------------------------------
        [HttpPost("GetAllNoticeBoardWithPage")]
        public async Task<IActionResult> GetAllNoticeBoardWithPage([FromBody] NoticeBoardSearchRequest request)
        {
            try
            {
                if (request.CompanyId == 0 || request.SessionId == 0)
                    return Ok(ApiResponse<List<NoticeBoardModel>>.SuccessResponse(new List<NoticeBoardModel>()));
                request.UserID = GetCurrentUserId();
                var data = await _noticeBoardService.GetAllNoticeBoardWithPage(request);
                return Ok(ApiResponse<PagedResult<NoticeBoardModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete classes." });

                int userId = GetCurrentUserId();
                var (success, message) = _noticeBoardService.DeleteNoticeBoard(ids, userId);
                return Ok(new { success, message });
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
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetCurrentUserId();
                var (success, message) = _noticeBoardService.ToggleNoticeBoardStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("UpdateNoticeBoardAttachment")]
        public async Task<IActionResult> UpdateNoticeBoardAttachment([FromBody] NoticeBoardAttachmentRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserId = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var response = await _noticeBoardService.UpdateNoticeBoardAttachmentAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
