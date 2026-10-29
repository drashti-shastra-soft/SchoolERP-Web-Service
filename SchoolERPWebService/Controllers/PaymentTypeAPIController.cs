using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    /// <summary>
    /// API Controller for managing Payment Types in the School ERP system.
    /// Provides endpoints for CRUD operations, status toggling, attachment management, and dropdown data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentTypeAPIController : ControllerBase
    {
        private readonly IPaymentTypeService _paymentTypeService;
        private readonly IUserMenuPermissionService _menuPerm;
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentTypeAPIController"/> class.
        /// </summary>
        /// <param name="paymentTypeService">Service for payment type operations.</param>
        /// <param name="menuPerm">Service for user menu permissions.</param>
        public PaymentTypeAPIController(IPaymentTypeService paymentTypeService, IUserMenuPermissionService menuPerm)
        {
            _paymentTypeService = paymentTypeService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Retrieves the current user's ID from the authentication claims.
        /// </summary>
        /// <returns>User ID as integer. Returns 1 if not found, or 0 on error.</returns>
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
        /// <summary>
        /// Creates or updates a payment type record.
        /// </summary>
        /// <param name="model">The payment type data to save.</param>
        /// <returns>
        /// 200 OK with operation result and message.<br/>
        /// 400 BadRequest if model validation fails.<br/>
        /// 500 Internal Server Error on exception.
        /// </returns>
        [HttpPost("Save")]
        public IActionResult Save([FromBody] PaymentTypeRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var spResult = _paymentTypeService.UpsertPaymentTypeAsync(model); // SpPaymentType

                var apiResponse = new ApiResponse<UpsertPaymentTypeResponse>
                {
                    Success = spResult?.Result == 1,
                    Message = spResult?.Message,
                    Data = new UpsertPaymentTypeResponse { Result = spResult }
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UpsertPaymentTypeResponse>
                {
                    Success = false,
                    Message = "Unable to save payment type. Please try again.",
                    Data = null
                });
            }
        }
        /// <summary>
        /// Retrieves a payment type by its unique identifier.
        /// </summary>
        /// <param name="id">The payment type ID.</param>
        /// <returns>
        /// 200 OK with the payment type data.<br/>
        /// 400 BadRequest if the ID is invalid.
        /// </returns>
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _paymentTypeService.GetPaymentTypeByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Retrieves a paginated list of payment types based on search criteria.
        /// </summary>
        /// <param name="request">Search and pagination parameters.</param>
        /// <returns>200 OK with paged result of payment types.</returns>
        [HttpPost("GetAllPaymentTypeWithPage")]
        public async Task<IActionResult> GetAllPaymentTypeWithPage([FromBody] SearchRequest request)
        {
            try
            {
                var data = await _paymentTypeService.GetAllPaymentTypeWithPage(request);
                return Ok(ApiResponse<PagedResult<PaymentTypeModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes one or more payment types by their IDs.
        /// </summary>
        /// <param name="ids">List of payment type IDs to delete.</param>
        /// <returns>
        /// 200 OK with success status and message.
        /// </returns>
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                int userId = GetCurrentUserId();
                string IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _paymentTypeService.DeletePaymentType(ids, userId, IPAddress);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Toggles the active status of one or more payment types.
        /// </summary>
        /// <param name="request">Status update request containing IDs and new status.</param>
        /// <returns>
        /// 200 OK with success status and message.
        /// </returns>
        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                request.DoneBy = userId;
                if (string.IsNullOrWhiteSpace(request.IpAddress))
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _paymentTypeService.TogglePaymentTypeStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves all payment types, optionally filtered by active status.
        /// </summary>
        /// <param name="isActive">Optional filter for active/inactive types.</param>
        /// <returns>200 OK with list of payment types.</returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null)
        {
            try
            {               
                var response = await _paymentTypeService.GetAllPaymentTypeAsync(isActive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates or updates the attachment file (e.g., logo) for a payment type.
        /// </summary>
        /// <param name="req">Attachment upsert request containing payment type ID and file URL.</param>
        /// <returns>
        /// 200 OK with success status and message.
        /// </returns>
        [HttpPost("UpsertPaymentTypeAttachmentFile")]
        public IActionResult UpsertPaymentTypeAttachmentFile([FromBody] PaymentTypeAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();
                var (success, message) = _paymentTypeService.UpsertPaymentTypeAttachmentFile(req, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves a list of payment types formatted for use in dropdown lists.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of dropdown models (ID and Name).
        /// </returns>
        [HttpGet("GetPaymenTypeDropdownList")]
        public IActionResult GetPaymenTypeDropdownList()
        {
            try
            {
                var result = _paymentTypeService.GetPaymenTypeDropdownList();

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
