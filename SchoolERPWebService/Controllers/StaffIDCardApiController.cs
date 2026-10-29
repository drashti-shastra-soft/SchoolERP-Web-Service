using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffIDCardApiController : ControllerBase
    {
        private readonly IStaffIDCardService _idCardService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Country/Index";

        public StaffIDCardApiController(IStaffIDCardService idCardService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _idCardService = idCardService;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                var data = _idCardService.GetAll(companyId, sessionId);
                return Ok(ApiResponse<List<StaffIDCardViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<List<StaffIDCardViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _idCardService.GetByID(id);
                return Ok(ApiResponse<StaffIDCardViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<StaffIDCardViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] StaffIDCardUpsertRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                var (result, message) = _idCardService.Upsert(request, userId, companyId, sessionId);
                return Ok(ApiResponse<int>.SuccessResponse(result, message));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<int>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Delete")]
        public IActionResult Delete(List<int> id)
        {
            try
            {
                int userId = GetCurrentUserId();
                var (result, message) = _idCardService.Delete(id, userId);
                return Ok(ApiResponse<int>.SuccessResponse(result, message));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<int>.ErrorResponse(ex.Message));
            }
        }

        //[HttpPost("ToggleStatus")]
        //public IActionResult ToggleStatus(int id, bool isActive)
        //{
        //    try
        //    {
        //        int userId = GetCurrentUserId();
        //        var (result, message) = _idCardService.ToggleStatus(id, isActive, userId);
        //        return Ok(ApiResponse<int>.SuccessResponse(result, message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponse<int>.ErrorResponse(ex.Message));
        //    }
        //}

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetCurrentUserId();
                var (success, message) = _idCardService.ToggleStaffIDCardStatus(request);
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
    }
}
