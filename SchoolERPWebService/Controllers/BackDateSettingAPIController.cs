using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BackDateSettingAPIController : ControllerBase
    {
        private readonly IBackDateSettingService _backDateSettingService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Academics/Class";

        public BackDateSettingAPIController(IBackDateSettingService backDateSettingService, IUserMenuPermissionService menuPerm)
        {
            _backDateSettingService = backDateSettingService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        // ------------------------------------------------------------
        // POST : api/BackDateSettingAPI/Save
        // INSERT OR UPDATE
        // ------------------------------------------------------------
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] SaveBackDateSettingRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _backDateSettingService.UpsertAttendanceBackDateSettingAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // GET : api/BackDateSettingAPI/GetById/{id}
        // ------------------------------------------------------------
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _backDateSettingService.GetAttendanceBackDateSettingByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // POST : api/BackDateSettingAPI/GetAllAttendanceBackDateSettingWithPage
        // ------------------------------------------------------------
        [HttpPost("GetAllAttendanceBackDateSettingWithPage")]
        public async Task<IActionResult> GetAllAttendanceBackDateSettingWithPage([FromBody] BackDateSettingSearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<BackDateSettingViewModel>>.SuccessResponse(new List<BackDateSettingViewModel>()));
                var data = await _backDateSettingService.GetAllAttendanceBackDateSettingWithPage(request);
                return Ok(ApiResponse<PagedResult<BackDateSettingViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        // ------------------------------------------------------------
        // GET : api/BackDateSettingAPI/GetById/{id}
        // ------------------------------------------------------------
        [HttpGet("GetBackDateCount")]
        public async Task<IActionResult> GetBackDateCount(int companyId, int staffID, int roleId)
        {
            try
            {
                if (companyId <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _backDateSettingService.GetBackDateCount(companyId, staffID, roleId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet("GetAttendanceBackDateSettingByStaffId")]
        public async Task<IActionResult> GetAttendanceBackDateSettingByStaffId(int staffID)
        {
            try
            {
                if (staffID <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _backDateSettingService.GetAttendanceBackDateSettingByStaffIdAsync(staffID);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
