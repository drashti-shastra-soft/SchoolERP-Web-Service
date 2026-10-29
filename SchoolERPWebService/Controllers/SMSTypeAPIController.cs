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
    /// API Controller for managing SMS Type entities and related operations.
    /// Provides endpoints for CRUD, status toggling, attachment management, and dropdown data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SMSTypeAPIController : ControllerBase
    {
        private readonly ISMSTypeService _smsTypeService;
        private readonly ISmsService _smsService;
        private readonly IUserMenuPermissionService _menuPerm;
        /// <summary>
        /// Initializes a new instance of the <see cref="SMSTypeAPIController"/> class.
        /// </summary>
        /// <param name="smsTypeService">Service for SMS Type operations.</param>
        /// <param name="menuPerm">Service for
        public SMSTypeAPIController(ISMSTypeService smsTypeService, IUserMenuPermissionService menuPerm, ISmsService smsService)
        {
            _smsTypeService = smsTypeService;
            _menuPerm = menuPerm;
            _smsService = smsService;
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
        /// Creates or updates an SMS Type record.
        /// </summary>
        /// <param name="model">The SMS Type data to save.</param>
        /// <returns>
        /// 200 OK with operation result and message.
        /// 400 BadRequest if model validation fails.
        /// 500 Internal Server Error on exception.
        /// </returns>
        [HttpPost("Save")]
        public IActionResult Save([FromBody] SMSTypeRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var spResult = _smsTypeService.UpsertSMSTypeAsync(model); // SpPaymentType

                var apiResponse = new ApiResponse<UpsertSMSTypeResponse>
                {
                    Success = spResult?.Result == 1,
                    Message = spResult?.Message,
                    Data = new UpsertSMSTypeResponse { Result = spResult }
                };

                return Ok(apiResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UpsertSMSTypeResponse>
                {
                    Success = false,
                    Message = "Unable to save payment type. Please try again.",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Retrieves an SMS Type by its unique identifier.
        /// </summary>
        /// <param name="id">The SMS Type ID.</param>
        /// <returns>
        /// 200 OK with the SMS Type data.
        /// 400 BadRequest if the ID is invalid.
        /// </returns>
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });


                //var companyId = 24;// await GetCompanyId();
                //await _smsService.SendAsync(companyId, "+919265248992", "Hi vanita, your registration is successful.");
                return Ok(new { success = true, message = "Student registered successfully." });

                //var response = await _smsTypeService.GetSMSByIdAsync(id);
                //return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }



        /// <summary>
        /// Retrieves a paginated list of SMS Types based on search criteria.
        /// </summary>
        /// <param name="request">Search and pagination parameters.</param>
        /// <returns>200 OK with paged result of SMS Types.</returns>
        [HttpPost("GetAllSMSTypeWithPage")]
        public async Task<IActionResult> GetAllSMSTypeWithPage([FromBody] SearchRequest request)
        {
            try
            {
                var data = await _smsTypeService.GetAllSMSTypeWithPage(request);
                return Ok(ApiResponse<PagedResult<SMSTypeModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes one or more SMS Types by their IDs.
        /// </summary>
        /// <param name="ids">List of SMS Type IDs to delete.</param>
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
                var (success, message) = _smsTypeService.DeleteSMSType(ids, userId, IPAddress);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Toggles the active status of one or more SMS Types.
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
                var (success, message) = _smsTypeService.ToggleSMSTypeStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
        /// <summary>
        /// Retrieves all SMS Types, optionally filtered by active status.
        /// </summary>
        /// <param name="isActive">Optional filter for active/inactive types.</param>
        /// <returns>200 OK with list of SMS Types.</returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null)
        {
            try
            {
                var response = await _smsTypeService.GetAllSMSTypeAsync(isActive);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Creates or updates the attachment file (e.g., logo) for an SMS Type.
        /// </summary>
        /// <param name="req">Attachment upsert request containing SMS Type ID and file URL.</param>
        /// <returns>
        /// 200 OK with success status and message.
        /// </returns>
        [HttpPost("UpsertSMSAttachmentFile")]
        public IActionResult UpsertSMSAttachmentFile([FromBody] SMSTypeAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();
                var (success, message) = _smsTypeService.UpsertSMSAttachmentFile(req, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves a list of SMS Types formatted for use in dropdown lists.
        /// </summary>
        /// <returns>
        /// 200 OK with a list of dropdown models (ID and Name).
        /// </returns>
        [HttpGet("GetSMSTypeDropdownList")]
        public IActionResult GetSMSTypeDropdownList()
        {
            try
            {
                var result = _smsTypeService.GetSMSTypeDropdownList();

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
