using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VoucherHeaderAPIController : ControllerBase
    {
        private readonly IVoucherHeaderService _voucherHeaderService;
        private readonly IUserMenuPermissionService _menuPerm;
        public VoucherHeaderAPIController(IVoucherHeaderService voucherHeaderService, IUserMenuPermissionService menuPerm)
        {
            _voucherHeaderService = voucherHeaderService;
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
        public IActionResult Save([FromBody] VoucherHeaderRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var spResult = _voucherHeaderService.UpsertVoucherHeaderAsync(model);


                return Ok(spResult);
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

        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _voucherHeaderService.GetVoucherHeaderIDByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost("GetAllVoucherHeaderWithPage")]
        public async Task<IActionResult> GetAllVoucherHeaderWithPage([FromBody] VoucherHeaderSearchRequest request)
        {
            try
            {
                var data = await _voucherHeaderService.GetAllVoucherHeaderWithPage(request);
                return Ok(ApiResponse<PagedResult<VoucherHeaderModel>>.SuccessResponse(data));

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
                int userId = GetCurrentUserId();
                string IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _voucherHeaderService.DeleteVoucherHeader(ids, userId, IPAddress);
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
                int userId = GetCurrentUserId();
                request.DoneBy = userId;
                if (string.IsNullOrWhiteSpace(request.IpAddress))
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _voucherHeaderService.ToggleVoucherHeaderStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        #region Voucher Header Value
        [HttpPost("UpsertVoucherHeaderValue")]
        public IActionResult UpsertVoucherHeaderValue([FromBody] VoucherHeaderValueRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var spResult = _voucherHeaderService.UpsertVoucherHeaderValueAsync(model);


                return Ok(spResult);
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
        
        [HttpGet("GetVoucherHeaderValueList")]
        public IActionResult GetVoucherHeaderValueList(int companyID, int sessionID, int fieldID, bool? isActive)
        {
            try
            {
                var result = _voucherHeaderService.GetVoucherHeaderValueList(companyID, sessionID, fieldID, isActive);

                return Ok(new ApiResponse<List<VoucherHeaderValue>>
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

        [HttpGet("GetVoucherHeaderValueById/{id:int}")]
        public async Task<IActionResult> GetVoucherHeaderValueById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _voucherHeaderService.GetVoucherHeaderValueByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Voucher Details
        [HttpPost("UpsertVoucherDetail")]
        public IActionResult UpsertVoucherDetail([FromBody] VoucherDetailRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var spResult = _voucherHeaderService.UpsertVoucherDetailAsync(model);


                return Ok(spResult);
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

        [HttpGet("GetVoucherDetailList")]
        public IActionResult GetVoucherDetailList(int companyID, int sessionID, int voucherHeaderID, bool? isActive)
        {
            try
            {
                var result = _voucherHeaderService.GetVoucherDetailList(companyID, sessionID, voucherHeaderID, isActive);

                return Ok(new ApiResponse<List<VoucherDetail>>
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

        [HttpGet("GetVoucherDetailById/{id:int}")]
        public async Task<IActionResult> GetVoucherDetailById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _voucherHeaderService.GetVoucherDetailByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Voucher Details Values

        [HttpPost("UpsertVoucherDetailValue")]
        public IActionResult UpsertVoucherDetailValue([FromBody] VoucherDetailValueRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var spResult = _voucherHeaderService.UpsertVoucherDetailValueAsync(model);


                return Ok(spResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<VoucherDetailValueRequest>
                {
                    Success = false,
                    Message = "Unable to save payment type. Please try again.",
                    Data = null
                });
            }
        }

        [HttpPost("GetVoucherDetailValueList")]
        public IActionResult GetVoucherDetailValueList(VoucherValueSearchRequest request)
        {
            try
            {
                var result = _voucherHeaderService.GetVoucherDetailValueList(request);

                return Ok(new ApiResponse<List<VoucherDetailValue>>
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

        [HttpGet("GetVoucherDetailValueById/{id:int}")]
        public async Task<IActionResult> GetVoucherDetailValueById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _voucherHeaderService.GetVoucherDetailValueByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        [HttpPost("SaveVoucher")]
        public async Task<IActionResult> SaveVoucher(
    [FromBody] VoucherSaveFullRequest request)
        {
            try
            {
                var result = await _voucherHeaderService.SaveVoucherFullAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Result = 0,
                    Message = "Unable to save voucher. Please try again.",
                    TECHNICALMESSAGE = ex.Message
                });
            }
        }

        [HttpGet("GetStaffDropdownList")]
        public IActionResult GetStaffDropdownList(int companyID, int sessionId)
        {
            try
            {
                var result = _voucherHeaderService.GetStaffDropdownList(companyID, sessionId);

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

        [HttpGet("GetStudentsDropdownList")]
        public IActionResult GetStudentsDropdownList(int companyID, int sessionId)
        {
            try
            {
                var result = _voucherHeaderService.GetStudentsDropdownList(companyID, sessionId);

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

        [HttpPost("UpsertVoucherHeaderAttachmentFile")]
        public IActionResult UpsertVoucherHeaderAttachmentFile([FromBody] VoucherHeaderAttachmentUpsertRequest req)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _voucherHeaderService.UpsertVoucherHeaderAttachmentFile(req, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("GetForClearing")]
        public IActionResult GetForClearing(VoucherBankClearingRequest req)
        {
            try
            {
                var result = _voucherHeaderService.GetForClearing(req);

                return Ok(new ApiResponse<List<VoucherDetailClearingResponse>>
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

        [HttpPost("SaveVoucherClearing")]
        public IActionResult SaveVoucherClearing([FromBody] VoucherBankClearingSaveRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                model.UserId = GetCurrentUserId();
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var spResult = _voucherHeaderService.SaveVoucherClearingAsync(model);


                return Ok(spResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<SpResult>
                {
                    Success = false,
                    Message = "Unable to save payment type. Please try again.",
                    Data = null
                });
            }
        }
    }
}
