using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.ComponentModel.Design;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FrontOfficeApiController : ControllerBase
    {
        private readonly IFrontOfficeService _svc;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string SetupMenuPath = "/FrontOffice/Setup";
        private const string ComplaintMenuPath = "/FrontOffice/Complaint";
        private const string PostalReceiveMenuPath = "/FrontOffice/PostalReceive";
        private const string PostalDispatchMenuPath = "/FrontOffice/PostalDispatch";
        private const string PhoneCallLogMenuPath = "/FrontOffice/PhoneCallLog";

        public FrontOfficeApiController(IFrontOfficeService svc, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _svc = svc;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        // ─── PURPOSE ────────────────────────────────────────────
        [HttpGet("GetAllPurposes")]
        public IActionResult GetAllPurposes(bool includeDeleted = false, int? companyId = null, int? sessionId = null   )
        {
            try
            {
                if(companyId==0 || companyId == null) 
                {
                    companyId = CompanyId;
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = SessionId;
                }
                var data = _svc.GetAllPurposes(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<MstFOPurposeViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
          
        }

        [HttpGet("GetPurposeByID/{id}")]
        public IActionResult GetPurposeByID(int id)
        {
            try
            {
                var data = _svc.GetPurposeByID(id);
                if (data == null) return NotFound(ApiResponse<MstFOPurposeViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<MstFOPurposeViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("UpsertPurpose")]
        public async Task<IActionResult> UpsertPurpose([FromBody] MstFOPurposeUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.PurposeID <= 0;
                if (isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add purposes." });
                if (!isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit purposes." });
                if (req.CompanyID == null)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null)
                {
                    req.SessionID = SessionId;
                }
                var (success, message) = _svc.UpsertPurpose(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        [HttpPost("DeletePurpose")]
        public async Task<IActionResult> DeletePurpose(List<int> id)
        {
            try
            {
                if (!await _menuPerm.Has(User, SetupMenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete purposes." });

                var (success, message) = _svc.DeletePurpose(id, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message }); 
            }
            
        }

        [HttpPost("TogglePurposeStatus")]
        public async Task<IActionResult> TogglePurposeStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change purpose status." });
                request.DoneBy = UserId;
                var (success, message) = _svc.TogglePurposeStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        // ─── COMPLAINT TYPE ─────────────────────────────────────
        [HttpGet("GetAllComplaintTypes")]
        public IActionResult GetAllComplaintTypes(bool includeDeleted = false,int? companyId=null,int? sessionId=null)
        {
            try
            {
                if(companyId==0 || companyId == null) 
                {
                    companyId = CompanyId;
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = SessionId;
                }
                var data = _svc.GetAllComplaintTypes(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<MstFOComplaintTypeViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message }); 
            }
            
        }

        [HttpGet("GetComplaintTypeByID/{id}")]
        public IActionResult GetComplaintTypeByID(int id)
        {
            try
            {
                var data = _svc.GetComplaintTypeByID(id);
                if (data == null) return NotFound(ApiResponse<MstFOComplaintTypeViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<MstFOComplaintTypeViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("UpsertComplaintType")]
        public async Task<IActionResult> UpsertComplaintType([FromBody] MstFOComplaintTypeUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.ComplaintTypeID <= 0;
                if (isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add complaint types." });
                if (!isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit complaint types." });
                if (req.CompanyID == null) 
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null) 
                {
                    req.SessionID = SessionId;
                }
                var (success, message) = _svc.UpsertComplaintType(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeleteComplaintType")]
        public async Task<IActionResult> DeleteComplaintType(List<int> id)
        {
            try
            {
                if (!await _menuPerm.Has(User, SetupMenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete complaint types." });

                var res = _svc.DeleteComplaintType(id, UserId);
                return Ok(new { res.success, res.message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("ToggleComplaintTypeStatus")]
        public async Task<IActionResult> ToggleComplaintTypeStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change complaint type status." });
                request.DoneBy = UserId;
                var (success, message) = _svc.ToggleComplaintTypeStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        // ─── SOURCE ─────────────────────────────────────────────
        [HttpGet("GetAllSources")]
        public IActionResult GetAllSources(bool includeDeleted = false, int? companyId = null, int? sessionId = null)
        {
            try
            {
                if (companyId == 0 || companyId == null)
                {
                    companyId = CompanyId;
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = SessionId;
                }

                var data = _svc.GetAllSources(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<MstFOSourceViewModel>>.SuccessResponse(data));
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpGet("GetSourceByID/{id}")]
        public IActionResult GetSourceByID(int id)
        {
            var data = _svc.GetSourceByID(id);
            if (data == null) return NotFound(ApiResponse<MstFOSourceViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<MstFOSourceViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertSource")]
        public async Task<IActionResult> UpsertSource([FromBody] MstFOSourceUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var isCreate = req.SourceID <= 0;
            if (isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Add"))
                return Ok(new { success = false, message = "You do not have permission to add sources." });
            if (!isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                return Ok(new { success = false, message = "You do not have permission to edit sources." });
            if (req.SessionID == null) 
            {
                req.SessionID = SessionId;
            }
            if (req.CompanyID == null)
            {
                req.CompanyID = CompanyId;
            }
            var (success, message) = _svc.UpsertSource(req, req.CompanyID, req.SessionID, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("DeleteSource")]
        public async Task<IActionResult> DeleteSource(List<int> id)
        {
            if (!await _menuPerm.Has(User, SetupMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete sources." });

            var resp = _svc.DeleteSource(id, UserId);
            return Ok(new { resp.success, resp.message });
        }

        [HttpPost("ToggleSourceStatus")]
        public async Task<IActionResult> ToggleSourceStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change source status." });

                var (success, message) = _svc.ToggleSourceStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        // ─── REFERENCE ──────────────────────────────────────────
        [HttpGet("GetAllReferences")]
        public IActionResult GetAllReferences(bool includeDeleted = false, int? companyId = null, int? sessionId = null)
        {
            try
            {
                if (companyId == 0 || companyId == null)
                {
                    companyId = CompanyId;
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = SessionId;
                }
                var data = _svc.GetAllReferences(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<MstFOReferenceViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        [HttpGet("GetReferenceByID/{id}")]
        public IActionResult GetReferenceByID(int id)
        {
            var data = _svc.GetReferenceByID(id);
            if (data == null) return NotFound(ApiResponse<MstFOReferenceViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<MstFOReferenceViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertReference")]
        public async Task<IActionResult> UpsertReference([FromBody] MstFOReferenceUpsertRequest req)
        {
            try
            {
                if (req.CompanyID == 0 || req.CompanyID == null)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == 0 || req.SessionID == null)
                {
                    req.SessionID = SessionId;
                }
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.ReferenceID <= 0;
                if (isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add references." });
                if (!isCreate && !await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit references." });

                var (success, message) = _svc.UpsertReference(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeleteReference")]
        public async  Task<IActionResult> DeleteReference(List<int> id)
        {
            if (!await _menuPerm.Has(User, SetupMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete references." });

            var (success, message) = _svc.DeleteReference(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleReferenceStatus")]
        public async Task<IActionResult> ToggleReferenceStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, SetupMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change reference status." });
                request.DoneBy = UserId;
            var (success, message) = _svc.ToggleReferenceStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }
        
        // ─── COMPLAINT ──────────────────────────────────────────
        [HttpGet("GetAllComplaints")]
        public IActionResult GetAllComplaints(bool includeDeleted = false)
        {
            var data = _svc.GetAllComplaints(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<FOComplaintViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetComplaintByID/{id}")]
        public IActionResult GetComplaintByID(int id)
        {
            var data = _svc.GetComplaintByID(id);
            if (data == null) return NotFound(ApiResponse<FOComplaintViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOComplaintViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertComplaint")]
        public async Task<IActionResult> UpsertComplaint([FromBody] FOComplaintUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.ComplaintID <= 0;
                if (isCreate && !await _menuPerm.Has(User, ComplaintMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add complaints." });
                if (!isCreate && !await _menuPerm.Has(User, ComplaintMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit complaints." });
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                var result = _svc.UpsertComplaint(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeleteComplaint")]
        public async Task<IActionResult> DeleteComplaint(List<int> id)
        {
            if (!await _menuPerm.Has(User, ComplaintMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete complaints." });

            var (success, message) = _svc.DeleteComplaint(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleComplaintStatus")]
        public async Task<IActionResult> ToggleComplaintStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, ComplaintMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change complaint status." });

                var (success, message) = _svc.ToggleComplaintStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        // ─── POSTAL RECEIVE ─────────────────────────────────────
        [HttpGet("GetAllPostalReceives")]
        public IActionResult GetAllPostalReceives(bool includeDeleted = false)
        {
            var data = _svc.GetAllPostalReceives(CompanyId, SessionId, includeDeleted);
            return Ok(ApiResponse<List<FOPostalReceiveViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetPostalReceiveByID/{id}")]
        public IActionResult GetPostalReceiveByID(int id)
        {
            var data = _svc.GetPostalReceiveByID(id);
            if (data == null) return NotFound(ApiResponse<FOPostalReceiveViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOPostalReceiveViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertPostalReceive")]
        public async Task<IActionResult> UpsertPostalReceive([FromBody] FOPostalReceiveUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.PostalReceiveID <= 0;
                if (isCreate && !await _menuPerm.Has(User, PostalReceiveMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add postal receives." });
                if (!isCreate && !await _menuPerm.Has(User, PostalReceiveMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit postal receives." });
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                if (req.CompanyID == null || req.CompanyID==0) 
                {
                    req.CompanyID = CompanyId;
                }
                var result = _svc.UpsertPostalReceive(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeletePostalReceive")]
        public async Task<IActionResult> DeletePostalReceive(List<int> id)
        {
            if (!await _menuPerm.Has(User, PostalReceiveMenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete postal receives." });

            var (success, message) = _svc.DeletePostalReceive(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("TogglePostalReceiveStatus")]
        public async Task<IActionResult> TogglePostalReceiveStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, PostalReceiveMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change postal receive status." });

                var (success, message) = _svc.TogglePostalReceiveStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        // ─── POSTAL DISPATCH ─────────────────────────────────────
        [HttpGet("GetAllPostalDispatches")]
        public IActionResult GetAllPostalDispatches(bool includeDeleted = false)
        {
            try
            {
                var data = _svc.GetAllPostalDispatches(CompanyId, SessionId, includeDeleted);
                return Ok(ApiResponse<List<FOPostalDispatchViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }            
        }

        [HttpPost("GetAllPostalDispatchesWithPageIndex")]
        public async Task<IActionResult> GetAllPostalDispatchesWithPageIndex([FromBody] FOPostalDispatchSearchRequest request)
        {
            try
            {

                if (request.CompanyID == null)
                {
                    request.CompanyID =CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FOPostalDispatchViewModel>>.SuccessResponse(new List<FOPostalDispatchViewModel>()));

                var data = await _svc.GetAllPostalDispatchesWithPageIndex(request);
                return Ok(ApiResponse<PagedResult<FOPostalDispatchViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetPostalDispatchByID/{id}")]
        public IActionResult GetPostalDispatchByID(int id)
        {
            try
            {
                var data = _svc.GetPostalDispatchByID(id);
                if (data == null) return NotFound(ApiResponse<FOPostalDispatchViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<FOPostalDispatchViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("UpsertPostalDispatch")]
        public async Task<IActionResult> UpsertPostalDispatch([FromBody] FOPostalDispatchUpsertRequest req)
        {
            try
            {

                if (req.CompanyID == 0 || req.CompanyID == null)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == 0 || req.SessionID == null)
                {
                    req.SessionID = SessionId;
                }
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.PostalDispatchID <= 0;
                if (isCreate && !await _menuPerm.Has(User, PostalDispatchMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add postal dispatches." });
                if (!isCreate && !await _menuPerm.Has(User, PostalDispatchMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit postal dispatches." });
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _svc.UpsertPostalDispatch(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { result });
            }
            catch (Exception  ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeletePostalDispatch")]
        public async Task<IActionResult> DeletePostalDispatch(List<int> id)
        {
            try
            {
                if (!await _menuPerm.Has(User, PostalDispatchMenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete postal dispatches." });

                var (success, message) = _svc.DeletePostalDispatch(id, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        [HttpPost("TogglePostalDispatchStatus")]
        public async Task<IActionResult> TogglePostalDispatchStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, PostalDispatchMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change postal dispatch status." });
                request.DoneBy = UserId;
                var (success, message) = _svc.TogglePostalDispatchStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        // ─── PHONE CALL LOG ─────────────────────────────────────
        [HttpGet("GetAllPhoneCallLogs")]
        public IActionResult GetAllPhoneCallLogs(bool includeDeleted = false)
        {
            try
            {
                var data = _svc.GetAllPhoneCallLogs(CompanyId, SessionId, includeDeleted);
                return Ok(ApiResponse<List<FOPhoneCallLogViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("GetPhoneCallLogChartData")]
        public async Task<IActionResult> GetPhoneCallLogChartData([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<PhoneCallLogChartResponse>
                        .SuccessResponse(null));

                var data = await _svc.GetPhoneCallLogChartDataAsync(request.CompanyID, request.SessionID);

                return Ok(ApiResponse<PhoneCallLogChartResponse>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("GetAllPhoneCallLogsWithPage")]
        public async Task<IActionResult> GetAllPhoneCallLogsWithPage([FromBody] FOPhoneCallLogSearchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FOPhoneCallLogViewModel>>.SuccessResponse(new List<FOPhoneCallLogViewModel>()));

                var data = await _svc.GetAllPhoneCallLogsWithPage(request);
                return Ok(ApiResponse<PagedResult<FOPhoneCallLogViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet("GetPhoneCallLogByID/{id}")]
        public IActionResult GetPhoneCallLogByID(int id)
        {
            try
            {
                var data = _svc.GetPhoneCallLogByID(id);
                if (data == null) return NotFound(ApiResponse<FOPhoneCallLogViewModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<FOPhoneCallLogViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        [HttpPost("UpsertPhoneCallLog")]
        public async Task<IActionResult> UpsertPhoneCallLog([FromBody] FOPhoneCallLogUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var isCreate = req.PhoneCallLogID <= 0;
                if (isCreate && !await _menuPerm.Has(User, PhoneCallLogMenuPath, "Add"))
                    return Ok(new { success = false, message = "You do not have permission to add phone call logs." });
                if (!isCreate && !await _menuPerm.Has(User, PhoneCallLogMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to edit phone call logs." });

                if (req.SessionID == null)
                {
                    req.SessionID = SessionId;
                }
                if (req.CompanyID == null)
                {
                    req.CompanyID = CompanyId;
                }
                var (success, message) = _svc.UpsertPhoneCallLog(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        [HttpPost("DeletePhoneCallLog")]
        public async Task<IActionResult> DeletePhoneCallLog(List<int> id)
        {
            try
            {
                if (!await _menuPerm.Has(User, PhoneCallLogMenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete phone call logs." });

                var (success, message) = _svc.DeletePhoneCallLog(id, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("TogglePhoneCallLogStatus")]
        public async Task<IActionResult> TogglePhoneCallLogStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, PhoneCallLogMenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change phone call log status." });
                request.DoneBy = UserId;
                var (success, message) = _svc.TogglePhoneCallLogStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        // ─── VISITOR BOOK ─────────────────────────────────────
        [HttpGet("GetAllVisitors")]
        public IActionResult GetAllVisitors(bool includeDeleted = false, int? companyId = null, int? sessionId = null)
        {
            try
            {
                if (companyId == 0 || companyId == null)
                {
                    companyId = CompanyId;
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = SessionId;
                }
                var data = _svc.GetAllVisitors(companyId.Value, sessionId.Value, includeDeleted);
                return Ok(ApiResponse<List<FOVisitorBookViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("GetAllVisitorsWithPageIndex")]
        public async Task<IActionResult> GetAllVisitorsWithPageIndex([FromBody] FOVisitorBookSerchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FOVisitorBookViewModel>>.SuccessResponse(new List<FOVisitorBookViewModel>()));

                var data = await _svc.GetAllVisitorsWithPageIndex(request);
                return Ok(ApiResponse<PagedResult<FOVisitorBookViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetVisitorByID/{id}")]
        public IActionResult GetVisitorByID(int id)
        {
            var data = _svc.GetVisitorByID(id);
            if (data == null) return NotFound(ApiResponse<FOVisitorBookViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOVisitorBookViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertVisitor")]
        public IActionResult UpsertVisitor([FromBody] FOVisitorBookUpsertRequest req)
        {
            try
            {
                if (req.CompanyID == 0 || req.CompanyID == null)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == 0 || req.SessionID == null)
                {
                    req.SessionID = SessionId;
                }

                if (!ModelState.IsValid) return BadRequest(ModelState);
                var result = _svc.UpsertVisitor(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { result });
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        [HttpPost("DeleteVisitor")]
        public IActionResult DeleteVisitor(List<int> id)
        {
            var (success, message) = _svc.DeleteVisitor(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("ToggleVisitorStatus")]
        public IActionResult ToggleVisitorStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {                
                int userId = UserId;
                var (success, message) = _svc.ToggleVisitorStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        // ─── ADMISSION INQUIRY ──────────────────────────────────────
        [HttpGet("GetAllAdmissionInquiries")]
        public IActionResult GetAllAdmissionInquiries(DateTime? fromDate = null, DateTime? toDate = null, int sourceId = 0, int classId = 0, string? status = null)
        {
            var data = _svc.GetAllAdmissionInquiries(CompanyId, SessionId, fromDate, toDate, sourceId, classId, status);
            return Ok(ApiResponse<List<FOAdmissionInquiryViewModel>>.SuccessResponse(data));
        }

        [HttpGet("GetAdmissionInquiryByID/{id}")]
        public IActionResult GetAdmissionInquiryByID(int id)
        {
            var data = _svc.GetAdmissionInquiryByID(id);
            if (data == null) return NotFound(ApiResponse<FOAdmissionInquiryViewModel>.ErrorResponse("Not found"));
            return Ok(ApiResponse<FOAdmissionInquiryViewModel>.SuccessResponse(data));
        }

        [HttpPost("UpsertAdmissionInquiry")]
        public IActionResult UpsertAdmissionInquiry([FromBody] FOAdmissionInquiryUpsertRequest req)
        {
            try
            {
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.UpsertAdmissionInquiry(req, req.CompanyID, req.SessionID, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("DeleteAdmissionInquiry")]
        public IActionResult DeleteAdmissionInquiry(List<int> id)
        {
            var (success, message) = _svc.DeleteAdmissionInquiry(id, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("SaveInquiryFollowUp")]
        public IActionResult SaveInquiryFollowUp([FromBody] FOInquiryFollowUpSaveRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.SaveInquiryFollowUp(req, UserId);
            return Ok(new { success, message });
        }

        
        [HttpPost("GetAllAdmissionInquiriesWithPageIndex")]
        public async Task<IActionResult> GetAllAdmissionInquiriesWithPageIndex([FromBody] EnquirySearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FOAdmissionInquiryViewModel>>.SuccessResponse(new List<FOAdmissionInquiryViewModel>()));
                var data = await _svc.GetAllAdmissionInquiriesWithPageIndex(request);
                return Ok(ApiResponse<PagedResult<FOAdmissionInquiryViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetInquiryStatusCount")]
        public async Task<IActionResult> GetInquiryStatusCount([FromQuery] InquiryStatusCountRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<InquiryStatusSummaryViewModel>.SuccessResponse(new InquiryStatusSummaryViewModel()));

                var data = await _svc.GetInquiryStatusCount(request);
                return Ok(ApiResponse<InquiryStatusSummaryViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("UpsertVisitorAttachmentFile")]
        public IActionResult UpsertVisitorAttachmentFile([FromBody] FOVisitorBookAttachmentUpsertRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, message) = _svc.UpsertVisitorAttachmentFile(req, UserId);
            return Ok(new { success, message });
        }

        [HttpPost("UpsertPostalDispatchAttachmentFile")]
        public IActionResult UpsertPostalDispatchAttachmentFile([FromBody] FOPostalDispatchAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.UpsertPostalDispatchAttachmentFile(req, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("GetAllPostalReceiveWithPage")]
        public async Task<IActionResult> GetAllPostalReceiveWithPage([FromBody] ClassSearchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FOPostalReceiveViewModel>>.SuccessResponse(new List<FOPostalReceiveViewModel>()));

                var data = await _svc.GetAllPostalReceiveWithPage(request);
                return Ok(ApiResponse<PagedResult<FOPostalReceiveViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("UpsertPostalReceiveAttachmentFile")]
        public IActionResult UpsertPostalReceiveAttachmentFile([FromBody] FOPostalReceiveAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.UpsertPostalReceiveAttachmentFile(req, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllComplaintsWithPage")]
        public async Task<IActionResult> GetAllComplaintsWithPage([FromBody] ComplaintSearchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<FOComplaintViewModel>>.SuccessResponse(new List<FOComplaintViewModel>()));

                var data = await _svc.GetAllComplaintsWithPage(request);
                return Ok(ApiResponse<PagedResult<FOComplaintViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllPurposesWithPage")]
        public async Task<IActionResult> GetAllPurposesWithPage([FromBody] ClassSearchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstFOPurposeViewModel>>.SuccessResponse(new List<MstFOPurposeViewModel>()));

                var data = await _svc.GetAllPurposesWithPage(request);
                return Ok(ApiResponse<PagedResult<MstFOPurposeViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllComplaintTypesWithPage")]
        public async Task<IActionResult> GetAllComplaintTypesWithPage([FromBody] ClassSearchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstFOComplaintTypeViewModel>>.SuccessResponse(new List<MstFOComplaintTypeViewModel>()));

                var data = await _svc.GetAllComplaintTypesWithPage(request);
                return Ok(ApiResponse<PagedResult<MstFOComplaintTypeViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllSourceWithPage")]
        public async Task<IActionResult> GetAllSourceWithPage([FromBody] ClassSearchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstFOSourceViewModel>>.SuccessResponse(new List<MstFOSourceViewModel>()));

                var data = await _svc.GetAllSourceWithPage(request);
                return Ok(ApiResponse<PagedResult<MstFOSourceViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllReferenceWithPage")]
        public async Task<IActionResult> GetAllReferenceWithPage([FromBody] ClassSearchRequest request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = CompanyId;
                }
                if (request.SessionID == null)
                {
                    request.SessionID = SessionId;
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<MstFOReferenceViewModel>>.SuccessResponse(new List<MstFOReferenceViewModel>()));

                var data = await _svc.GetAllReferenceWithPage(request);
                return Ok(ApiResponse<PagedResult<MstFOReferenceViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("UpsertComplaintAttachmentFile")]
        public IActionResult UpsertComplaintAttachmentFile([FromBody] FOComplaintAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _svc.UpsertComplaintAttachmentFile(req, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("CopyPurpose")]
        public async Task<IActionResult> CopyPurpose([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = UserId;
                }              
                // ── Execute copy ──────────────────────────────────────────
                var result = await _svc.CopyPurpose(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost("CopyComplaintType")]
        public async Task<IActionResult> CopyComplaintType([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = UserId;
                }
                // ── Execute copy ──────────────────────────────────────────
                var result = await _svc.CopyComplaintType(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost("CopySource")]
        public async Task<IActionResult> CopySource([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = UserId;
                }
                // ── Execute copy ──────────────────────────────────────────
                var result = await _svc.CopySource(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost("CopyReference")]
        public async Task<IActionResult> CopyReference([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = UserId;
                }
                // ── Execute copy ──────────────────────────────────────────
                var result = await _svc.CopyReference(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpGet("GetSetUpFOMasterDropdowmList")]
        public IActionResult GetSetUpFOMasterDropdowmList(int companyId, int sessionID, string type)
        {
            try
            {
                var result = _svc.GetSetUpFOMasterDropdowm(companyId, sessionID, type);

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

        [HttpGet("GetVisitorDashboard")]
        public async Task<IActionResult> GetVisitorDashboard()
        {
            try
            {

                int userId = GetCurrentUserId();
                int companyId = _companySvc.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionSvc.GetUserCurrentSession(userId) ?? 0;

                if (companyId == 0 || sessionId == 0)
                    return Ok(ApiResponse<List<VisitorDashboardDto>>.SuccessResponse(new List<VisitorDashboardDto>()));

                var data = await Task.Run(() => _svc.GetAllVisitors(companyId, sessionId));

                return Ok(ApiResponse<List<VisitorDashboardDto>>.SuccessResponse(
                    data.Select(v => new VisitorDashboardDto
                    {
                        VisitorBookID = v.VisitorBookID,
                        Name = v.Name,
                        Phone = v.Phone,
                        PurposeName = v.PurposeName,
                        Date = v.Date,
                        InTime = v.InTime,
                        OutTime = v.OutTime,
                        MeetingWith = v.MeetingWith,
                        StudentName = v.StudentName,
                        StaffName = v.StaffName,
                        NoOfPersons = v.NoOfPersons,
                        Note = v.Note
                    }).ToList()
                ));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}
