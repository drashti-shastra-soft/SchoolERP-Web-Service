using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoutePickupPointsApiController : ControllerBase
    {
        private readonly IRoutePickupPointService _rppService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Transport/RoutePickupPoints";

        public RoutePickupPointsApiController(IRoutePickupPointService rppService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _rppService = rppService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllRoutePickupPoints")]
        public IActionResult GetAllRoutePickupPoints(int companyId, int sessionId)
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
                var data = _rppService.GetAllRoutePickupPoints(companyId, sessionId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetRoutePickupPointByID/{id}")]
        public IActionResult GetRoutePickupPointByID(int id)
        {
            try
            {
                var data = _rppService.GetRoutePickupPointByID(id);
                if (data == null) return Ok(new { success = false, message = "Record not found" });
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("UpsertRoutePickupPoint")]
        public async Task<IActionResult> UpsertRoutePickupPoint([FromBody] RoutePickupPointUpsertRequest req)
        {
            try
            {
                var isCreate = req.RoutePickupPointID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add route pickup points." });
                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit route pickup points." });
                if (req.CompanyID == 0 || req.CompanyID == null)
                {
                    req.CompanyID =  GetCompanyId();
                }
                if (req.SessionID == 0 || req.SessionID == null)
                {
                    req.SessionID =  GetSessionId();
                }
                var res = _rppService.UpsertRoutePickupPoint(req, req.CompanyID, req.SessionID, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeleteRoutePickupPoint")]
        public async Task<IActionResult> DeleteRoutePickupPoint(List<int> id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete route pickup points." });

            var res = _rppService.DeleteRoutePickupPoint(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        //[HttpPost("ToggleRoutePickupPointStatus")]
        //public async Task<IActionResult> ToggleRoutePickupPointStatus(int id, bool isActive)
        //{
        //    if (!await _menuPerm.Has(User, MenuPath, "Edit"))
        //        return Ok(new { success = false, message = "You do not have permission to change status." });

        //    var res = _rppService.ToggleRoutePickupPointStatus(id, isActive, GetUserId());
        //    return Ok(new { success = res.Success, message = res.Message });
        //}


        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetUserId();
                var (success, message) = _rppService.ToggleRoutePickupPointStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
