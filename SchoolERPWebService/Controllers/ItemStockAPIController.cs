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
    public class ItemStockAPIController : ControllerBase
    {
        private readonly IItemStockService _service;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/ItemCategory/Class";

        public ItemStockAPIController(IItemStockService service, IUserMenuPermissionService menuPerm)
        {
            _service = service;
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

        [HttpPost("Save")]
        public IActionResult Save([FromBody] ItemStockModelRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserId = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var result = _service.UpsertItemStockAsync(model);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        // ------------------------------------------------------------
        // GET : api/HostelType/GetById/{id}
        // ------------------------------------------------------------
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _service.GetItemStockByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // POST : api/ItemCategory/GetAllItemCategoryWithPage
        // ------------------------------------------------------------
        [HttpPost("GetAllItemStockWithPage")]
        public async Task<IActionResult> GetAllItemStockWithPage([FromBody] SearchItemStockRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<ItemStockModel>>.SuccessResponse(new List<ItemStockModel>()));

                var data = await _service.GetAllItemStockWithPage(request);
                return Ok(ApiResponse<PagedResult<ItemStockModel>>.SuccessResponse(data));

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
                var (success, message) = _service.DeleteItemStock(ids, userId);
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
                var (success, message) = _service.ToggleItemStockStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("UpsertPostalDispatchAttachmentFile")]
        public IActionResult UpsertPostalDispatchAttachmentFile([FromBody] ItemStockAttachmentUpsertRequest req)
        {
            try
            {
                var userid = GetCurrentUserId();
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _service.UpsertItemStockAttachmentFile(req, userid);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
