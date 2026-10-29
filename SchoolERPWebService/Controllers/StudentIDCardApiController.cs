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
    public class StudentIDCardApiController : ControllerBase
    {
        private readonly IStudentIDCardService _idCardService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Country/Index";

        public StudentIDCardApiController(IStudentIDCardService idCardService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _idCardService = idCardService;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(int companyId, int sessionId)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (companyId == 0 || companyId == null)
                {
                    companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                var data = _idCardService.GetAll(companyId, sessionId);
                return Ok(ApiResponse<List<StudentIDCardViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<List<StudentIDCardViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _idCardService.GetByID(id);
                return Ok(ApiResponse<StudentIDCardViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<StudentIDCardViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] StudentIDCardUpsertRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                if (request.CompanyID == 0 || request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == 0 || request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                var (result, message) = _idCardService.Upsert(request, userId, request.CompanyID, request.SessionID);
                if (result > 0)
                {
                    return Ok(ApiResponse<int>.SuccessResponse(result, message));
                }
                return Ok(ApiResponse<int>.ErrorResponse(message));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<int>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] List<int> id)
        {
            try
            {
                int userId = GetCurrentUserId();
                var (result, message) = _idCardService.Delete(id, userId);
                if (result > 0)
                {
                    return Ok(ApiResponse<int>.SuccessResponse(result, message));
                }
                return Ok(ApiResponse<int>.ErrorResponse(message));
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
                var (success, message) = _idCardService.ToggleStudentIDCardStatus(request);
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
