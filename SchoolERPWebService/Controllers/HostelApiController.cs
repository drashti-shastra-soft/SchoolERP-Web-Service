using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HostelApiController : ControllerBase
    {
        private readonly IHostelService _hostelService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string RoomTypeMenuPath = "/Hostel/RoomType";
        private const string HostelMenuPath = "/Hostel";
        private const string HostelRoomMenuPath = "/Hostel/HostelRoom";

        public HostelApiController(IHostelService hostelService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _hostelService = hostelService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        [HttpGet("GetAllRoomTypes")]
        public IActionResult GetAllRoomTypes(bool includeDeleted = false, int? sessionId = null,int? companyId=null)
        {
            try
            {
                if (sessionId == null) 
                {
                    sessionId = SessionId;
                }
                if (companyId == null)
                {
                    companyId = CompanyId;
                }
                var data = _hostelService.GetAllRoomTypes(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<RoomTypeViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<RoomTypeViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetRoomTypeByID/{id}")]
        public IActionResult GetRoomTypeByID(int id)
        {
            try
            {
                var data = _hostelService.GetRoomTypeByID(id);
                if (data == null) return NotFound(ApiResponse<RoomTypeViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<RoomTypeViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<RoomTypeViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertRoomType")]
        public async Task<IActionResult> UpsertRoomType([FromBody] RoomTypeUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.RoomTypeID <= 0;
                if (isCreate && !await _menuPerm.Has(User, RoomTypeMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add room types." });
                if (!isCreate && !await _menuPerm.Has(User, RoomTypeMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit room types." });
                if (req.SessionID == null && req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                if (req.CompanyID == null && req.CompanyID == 0)
                {
                    req.CompanyID = CompanyId;
                }
                var (success, message) = _hostelService.UpsertRoomType(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteRoomType")]
        public async Task<IActionResult> DeleteRoomType(List<int> id)
        {
            try
            {
                if (!await _menuPerm.Has(User, RoomTypeMenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete room types." });

                var result = _hostelService.DeleteRoomType(id, UserId);
                return Ok(new { success = true, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleRoomTypeStatus")]
        public async Task<IActionResult> ToggleRoomTypeStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, RoomTypeMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change room type status." });
                request.DoneBy = UserId;
                var (success, message) =  _hostelService.ToggleRoomTypeStatus(request);
                return Ok(new { success, message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ─── HOSTEL ─────────────────────────────────────────────
        [HttpGet("GetAllHostels")]
        public IActionResult GetAllHostels(bool includeDeleted = false, int? sessionId = null,int ? companyId = null)
        {
            try
            {
                if (sessionId == null) 
                {
                    sessionId = SessionId;
                }
                if (companyId == null)
                {
                    companyId = CompanyId;
                }
                var data = _hostelService.GetAllHostels(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<HostelViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<HostelViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetHostelByID/{id}")]
        public IActionResult GetHostelByID(int id)
        {
            try
            {
                var data = _hostelService.GetHostelByID(id);
                if (data == null) return NotFound(ApiResponse<HostelViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<HostelViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<HostelViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertHostel")]
        public async Task<IActionResult> UpsertHostel([FromBody] HostelUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.HostelID <= 0;
                if (isCreate && !await _menuPerm.Has(User, HostelMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add hostels." });
                if (!isCreate && !await _menuPerm.Has(User, HostelMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit hostels." });
                if(req.SessionID==null && req.SessionID == 0) 
                {
                    req.SessionID = SessionId;
                }
                if (req.CompanyID == null && req.CompanyID == 0)
                {
                    req.CompanyID = CompanyId;
                }
                var (success, message) = _hostelService.UpsertHostel(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success , message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteHostel")]
        public async Task<IActionResult> DeleteHostel(List<int> id)
        {
            try
            {
                if (!await _menuPerm.Has(User, HostelMenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete hostels." });

                var result = _hostelService.DeleteHostel(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleHostelStatus")]
        public async Task<IActionResult> ToggleHostelStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, HostelMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change hostel status." });
                request.DoneBy = UserId;
                var (success, message) = _hostelService.ToggleHostelStatus(request);
                return Ok(new { success , message  });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ─── HOSTEL ROOM ────────────────────────────────────────
        [HttpGet("GetAllHostelRooms")]
        public IActionResult GetAllHostelRooms(bool includeDeleted = false, int? sessionId = null)
        {
            try
            {
                if (sessionId == null) 
                {
                     sessionId= SessionId;
                }
                var data = _hostelService.GetAllHostelRooms(CompanyId, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<HostelRoomViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<HostelRoomViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetHostelRoomByID/{id}")]
        public IActionResult GetHostelRoomByID(int id)
        {
            try
            {
                var data = _hostelService.GetHostelRoomByID(id);
                if (data == null) return NotFound(ApiResponse<HostelRoomViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<HostelRoomViewModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<HostelRoomViewModel>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("UpsertHostelRoom")]
        public async Task<IActionResult> UpsertHostelRoom([FromBody] HostelRoomUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.RoomId <= 0;
                if (isCreate && !await _menuPerm.Has(User, HostelRoomMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add hostel rooms." });
                if (!isCreate && !await _menuPerm.Has(User, HostelRoomMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit hostel rooms." });
                
                if (req.SessionID == null && req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                if (req.CompanyID == null && req.CompanyID == 0)
                {
                    req.CompanyID = CompanyId;
                }
                var (success, message) = _hostelService.UpsertHostelRoom(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("DeleteHostelRoom")]
        public async Task<IActionResult> DeleteHostelRoom(List<int> id)
        {
            try
            {
                if (!await _menuPerm.Has(User, HostelRoomMenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete hostel rooms." });

                var result = _hostelService.DeleteHostelRoom(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleHostelRoomStatus")]
        public async Task<IActionResult> ToggleHostelRoomStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, HostelRoomMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change hostel room status." });
                request.DoneBy = UserId;
                var result = _hostelService.ToggleHostelRoomStatus(request);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("GetAllRoomTypeWithPage")]
        public async Task<IActionResult> GetAllRoomTypeWithPage([FromBody] ClassSearchRequest request)
        {
            try
            {
                int userId = UserId;

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companySvc.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionSvc.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstClassViewModel>>.SuccessResponse(new List<MstClassViewModel>()));

                var data = await _hostelService.GetAllRoomTypeWithPage(request);
                return Ok(ApiResponse<PagedResult<RoomTypeViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost("GetAllHotelWithPage")]
        public async Task<IActionResult> GetAllHotelWithPage([FromBody] HotelSearchRequest request)
        {
            try
            {
                int userId = UserId;

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companySvc.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionSvc.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<HostelViewModel>>.SuccessResponse(new List<HostelViewModel>()));

                var data = await _hostelService.GetAllHostelWithPage(request);
                return Ok(ApiResponse<PagedResult<HostelViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpPost("GetAllHostelRoomlWithPage")]
        public async Task<IActionResult> GetAllHostelRoomlWithPage([FromBody] HotelSearchRequest request)
        {
            try
            {
                int userId = UserId;

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companySvc.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionSvc.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<HostelRoomViewModel>>.SuccessResponse(new List<HostelRoomViewModel>()));

                var data = await _hostelService.GetAllHostelRoomlWithPage(request);
                return Ok(ApiResponse<PagedResult<HostelRoomViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet("GetHostelRoomRateByID/{id}")]
        public async Task<IActionResult> GetHostelRoomRateByID(int id)
        {
            try
            {
                var data = await _hostelService.GetHostelRoomRateByID(id);

                if (data == null || !data.Any())
                {
                    return NotFound(
                        ApiResponse<List<HostelRoomRateViewModel>>
                            .ErrorResponse("No Record Found."));
                }

                return Ok(
                    ApiResponse<List<HostelRoomRateViewModel>>
                        .SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return BadRequest(
                    ApiResponse<List<HostelRoomRateViewModel>>
                        .ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAllRoomOccupancyByRoomTypesWise")]
        public IActionResult GetAllRoomOccupancyByRoomTypesWise(int roomTypeId)
        {
            try
            {
                var data = _hostelService.GetAllRoomOccupancyByRoomTypesWise(roomTypeId);
                return Ok(ApiResponse<List<RoomTypeViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<RoomTypeViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetHostelSummary")]
        public IActionResult GetHostelSummary(int hostelId)
        {
            try
            {
                var data = _hostelService.GetHostelSummary(hostelId);
                return Ok(ApiResponse<List<HostelSummary>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<HostelSummary>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost("GetAllHostelReportWithPage")]
        public async Task<IActionResult> GetAllHostelReportWithPage([FromBody] HotelReportSearchRequest request)
        {
            try
            {
                int userId = UserId;

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companySvc.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = _sessionSvc.GetUserCurrentSession(userId) ?? 0;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<HostelReportResponse>>.SuccessResponse(new List<HostelReportResponse>()));

                var data = await _hostelService.GetAllHostelReportWithPage(request);
                return Ok(ApiResponse<List<HostelReportResponse>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
