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
    public class RoutesApiController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Transport/Routes";

        public RoutesApiController(IRouteService routeService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _routeService = routeService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int GetUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("UserId"), out var id) ? id : 0;
        private int GetCompanyId() => _companySvc.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionSvc.GetUserCurrentSession(GetUserId()) ?? 0;

        [HttpGet("GetAllRoutes")]
        public IActionResult GetAllRoutes(int companyId, int sessionId)
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
                var data = _routeService.GetAllRoutes(companyId, sessionId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        [HttpGet("GetRouteByID/{id}")]
        public IActionResult GetRouteByID(int id)
        {
            var data = _routeService.GetRouteByID(id);
            if (data == null) return Ok(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data });
        }

        [HttpPost("UpsertRoute")]
        public async Task<IActionResult> UpsertRoute([FromBody] RouteUpsertRequest req)
        {
            var isCreate = req.RouteID <= 0;
            if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add routes." });
            if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit routes." });
            if (req.CompanyID == 0 || req.CompanyID == null)
            {
                req.CompanyID =  GetCompanyId();
            }
            if (req.SessionID == 0 || req.SessionID == null)
            {
                req.SessionID =  GetSessionId();
            }
            var res = _routeService.UpsertRoute(req, req.CompanyID, req.SessionID, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        [HttpPost("DeleteRoute")]
        public async Task<IActionResult> DeleteRoute(List<int> id)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete routes." });

            var res = _routeService.DeleteRoute(id, GetUserId());
            return Ok(new { success = res.Success, message = res.Message });
        }

        //[HttpPost("ToggleRouteStatus")]
        //public async Task<IActionResult> ToggleRouteStatus(int id, bool isActive)
        //{
        //    if (!await _menuPerm.Has(User, MenuPath, "Edit"))
        //        return Ok(new { success = false, message = "You do not have permission to change route status." });

        //    var res = _routeService.ToggleRouteStatus(id, isActive, GetUserId());
        //    return Ok(new { success = res.Success, message = res.Message });
        //}

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change route status." });

                int userId = GetUserId();
                var (success, message) = _routeService.ToggleRoutesStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
