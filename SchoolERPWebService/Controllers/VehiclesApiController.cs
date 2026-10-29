using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolERPWebService.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesApiController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Transport/Vehicles";

        public VehiclesApiController(IVehicleService vehicleService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _vehicleService = vehicleService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllVehicles")]
        public IActionResult GetAllVehicles(int companyId, int sessionId)
        {
            try
            {
                if (companyId == 0 || companyId == null)
                {
                    companyId = GetCompanyId();
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = GetSessionId();
                }
                var data = _vehicleService.GetAllVehicles(companyId, sessionId);
                return Ok(new { success = true, data });
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpGet("GetVehicleByID/{id}")]
        public IActionResult GetVehicleByID(int id)
        {
            var data = _vehicleService.GetVehicleByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertVehicle")]
        public async Task<IActionResult> UpsertVehicle([FromForm] VehicleFormModel form)
        {
            try
            {
                var isCreate = form.VehicleID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add vehicles." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit vehicles." });

                var req = new VehicleUpsertRequest
                {
                    VehicleID = form.VehicleID,
                    VehicleNumber = form.VehicleNumber,
                    VehicleModel = form.VehicleModel,
                    VehicleYearMade = form.VehicleYearMade,
                    VehicleRegNo = form.VehicleRegNo,
                    VehicleChasisNo = form.VehicleChasisNo,
                    VehicleMaxCapicity = form.VehicleMaxCapicity,
                    VehicleDriverName = form.VehicleDriverName,
                    VehicleDriverLicense = form.VehicleDriverLicense,
                    VehicleDriverContact = form.VehicleDriverContact,
                    VehicleNote = form.VehicleNote,
                    IsActive = form.IsActive
                };

                if (form.DriverPhoto != null && form.DriverPhoto.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        //vanita tempory code commit
                        //await form.DriverPhoto.CopyToAsync(ms);
                        //req.VehicleDriverPhotoAttach = ms.ToArray();
                        //req.VehicleDriverPhotoName = form.DriverPhoto.FileName;
                        //req.VehicleDriverPhotoType = form.DriverPhoto.ContentType;
                    }
                }
                if (form.CompanyID == 0 || form.CompanyID == null)
                {
                    form.CompanyID =  GetCompanyId();
                }
                if (form.SessionID == 0 || form.SessionID == null)
                {
                    form.SessionID =  GetSessionId();
                }
                var res = _vehicleService.UpsertVehicle(req, form.CompanyID, form.SessionID, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteVehicle")]
        public async Task<IActionResult> DeleteVehicle(List<int> id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete vehicles." });

            var res = _vehicleService.DeleteVehicle(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetUserId();
                var (success, message) = _vehicleService.ToggleVehicleStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
