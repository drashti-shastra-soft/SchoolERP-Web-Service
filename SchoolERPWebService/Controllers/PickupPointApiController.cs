using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PickupPointApiController : ControllerBase
    {
        private readonly IPickupPointService _pickupPointService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Transport/PickupPoint";

        public PickupPointApiController(IPickupPointService pickupPointService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _pickupPointService = pickupPointService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllPickupPoints")]
        public IActionResult GetAllPickupPoints(int companyId,int sessionId)
        {
            try
            {
                if (companyId == 0 || companyId == null)
                {
                    companyId =  GetCompanyId();
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId =  GetSessionId();
                }
                var data = _pickupPointService.GetAllPickupPoints(companyId, sessionId);
                return Ok(new { success = true, data });
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpGet("GetPickupPointByID/{id}")]
        public IActionResult GetPickupPointByID(int id)
        {
            var data = _pickupPointService.GetPickupPointByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertPickupPoint")]
        public async Task<IActionResult> UpsertPickupPoint([FromBody] PickupPointUpsertRequest req)
        {
            try
            {
                var isCreate = req.PickupPointID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add pickup points." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit pickup points." });
                if (req.CompanyID == 0 || req.CompanyID == null)
                {
                    req.CompanyID = GetCompanyId();
                }
                if (req.SessionID == 0 || req.SessionID == null)
                {
                    req.SessionID = GetSessionId();
                }
                var res = _pickupPointService.UpsertPickupPoint(req, req.CompanyID, req.SessionID, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeletePickupPoint")]
        public async Task<IActionResult> DeletePickupPoint(List<int> id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete pickup points." });

            var res = _pickupPointService.DeletePickupPoint(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        //[HttpPost("TogglePickupPointStatus")]
        //public async Task<IActionResult> TogglePickupPointStatus(int id, bool isActive)
        //{
        //    if (!await _menuPerm.Has(User, MenuPath, "Edit"))
        //        return Ok(new { success = false, message = "You do not have permission to change pickup point status." });

        //    var res = _pickupPointService.TogglePickupPointStatus(id, isActive, GetUserId());
        //    return Ok(new { success = res.Success, message = res.Message });
        //}

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change pickup point status." });

                var (success, message) = _pickupPointService.TogglePickupPointStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
