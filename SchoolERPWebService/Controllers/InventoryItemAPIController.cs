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
    public class InventoryItemAPIController : ControllerBase
    {
        private readonly IInventoryItemService _service;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/ItemCategory/Class";

        public InventoryItemAPIController(IInventoryItemService service, IUserMenuPermissionService menuPerm)
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

        // ------------------------------------------------------------
        // POST : api/HostelType/Save
        // INSERT OR UPDATE
        // ------------------------------------------------------------
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] InventoryItemModelRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserId = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _service.UpsertInventoryItemAsync(model);
                return Ok(response);
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

                var response = await _service.GetInventoryItemByIdAsync(id);
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
        [HttpPost("GetAllInventoryItemWithPage")]
        public async Task<IActionResult> GetAllInventoryItemWithPage([FromBody] SearchInventoryItemRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<InventoryItemModel>>.SuccessResponse(new List<InventoryItemModel>()));

                var data = await _service.GetAllInventoryItemWithPage(request);
                return Ok(ApiResponse<PagedResult<InventoryItemModel>>.SuccessResponse(data));

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
                var (success, message) = _service.DeleteInventoryItem(ids, userId);
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
                var (success, message) = _service.ToggleInventoryItemStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

    }
}
