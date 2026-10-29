using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;
using System.ComponentModel.Design;
using SchoolERPWebService.Services;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountHeadApiController : ControllerBase
    {
        private readonly IAccountHeadService _accountHeadService;
        private readonly ICompanyService _companySvc;
        private readonly ISessionService _sessionSvc;
        private readonly IUserMenuPermissionService _menuPerm;

        public AccountHeadApiController(IAccountHeadService accountHeadService, ICompanyService companySvc, ISessionService sessionSvc, IUserMenuPermissionService menuPerm)
        {
            _accountHeadService = accountHeadService;
            _companySvc = companySvc;
            _sessionSvc = sessionSvc;
            _menuPerm = menuPerm;
        }

        private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "1");
        private int CompanyId => _companySvc.GetUserCurrentCompany(UserId) ?? 0;
        private int SessionId => _sessionSvc.GetUserCurrentSession(UserId) ?? 0;

        [HttpGet("GetAllAccountHeads")]
        public IActionResult GetAllAccountHeads(string headType, bool includeDeleted = false,int ? companyId = null, int? sessionId = null)
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
                var data = _accountHeadService.GetAllAccountHeads(companyId.Value, sessionId.Value, headType, includeDeleted);
                return Ok(ApiResponse<List<AccountHeadViewModel>>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<List<AccountHeadViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetAccountHeadByID/{id}")]
        public IActionResult GetAccountHeadByID(int id)
        {
            try
            {
                var data = _accountHeadService.GetAccountHeadByID(id);
                if (data == null) return NotFound(ApiResponse<AccountHeadModel>.ErrorResponse("Not found"));
                return Ok(ApiResponse<AccountHeadModel>.SuccessResponse(data));
            }
            catch (System.Exception ex)
            {
                return Ok(ApiResponse<AccountHeadModel>.ErrorResponse(ex.Message));
            }
        }

        //[HttpPost("UpsertAccountHead")]
        //public async Task<IActionResult> UpsertAccountHead([FromBody] AccountHeadUpsertRequest req)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid) return BadRequest(ModelState);
        //        string menuPath = (req.HeadType == "Income") ? "/Income/IncomeHead" : "/Expense/ExpenseHead";
        //        var isCreate = req.AccountHeadID <= 0;
        //        if (isCreate && !await _menuPerm.Has(User, menuPath, "Add"))
        //            return Ok(new { success = false, message = $"You do not have permission to add {req.HeadType.ToLower()} heads." });
        //        if (!isCreate && !await _menuPerm.Has(User, menuPath, "Edit"))
        //            return Ok(new { success = false, message = $"You do not have permission to edit {req.HeadType.ToLower()} heads." });
        //        if (req.CompanyID == null || req.CompanyID==0)
        //        {
        //            req.CompanyID = CompanyId;
        //        }
        //        if (req.SessionID == null || req.SessionID==0)
        //        {
        //            req.SessionID = SessionId;
        //        }
        //        var result = _accountHeadService.UpsertAccountHead(req, req.CompanyID, req.SessionID, UserId);
        //        return Ok(new { success = result.Success, message = result.Message });
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return Ok(new { success = false, message = ex.Message });
        //    }
        //}

        [HttpPost("DeleteAccountHead")]
        public async Task<IActionResult> DeleteAccountHead(List<int> id)
        {
            try
            {
                //foreach (var accountEntryId in id)
                //{
                //    var entry = _accountHeadService.GetAccountHeadByID(accountEntryId);

                //    if (entry == null)
                //    {
                //        return Ok(new
                //        {
                //            success = false,
                //            message = $"Entry not found. ID : {accountEntryId}"
                //        });
                //    }

                //    string menuPath = entry.HeadType == "Income"
                //        ? "/Income"
                //        : "/Expense";

                //    if (!await _menuPerm.Has(User, menuPath, "Delete"))
                //    {
                //        return Ok(new
                //        {
                //            success = false,
                //            message = $"You do not have permission to delete {entry.HeadType.ToLower()} heads."
                //        });
                //    }
                //}
                
                var result = _accountHeadService.DeleteAccountHead(id, UserId);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("ToggleAccountHeadStatus")]
        public async Task<IActionResult> ToggleAccountHeadStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
               
                request.DoneBy = UserId;
                var result = _accountHeadService.ToggleAccountHeadStatus(request);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("GetAllAccountHeadWithPageIndex")]
        public async Task<IActionResult> GetAllAccountHeadWithPageIndex([FromBody] AccountHeadSearchRequest request)
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
                    return Ok(ApiResponse<List<AccountHeadViewModel>>.SuccessResponse(new List<AccountHeadViewModel>()));

                var data = await _accountHeadService.GetAllAccountHeadWithPageIndex(request);
                return Ok(ApiResponse<PagedResult<AccountHeadViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("CopyAccountHead")]
        public async Task<IActionResult> CopyAccountHead([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = UserId;
                }
                // ── Execute copy ──────────────────────────────────────────
                var result = await _accountHeadService.CopyAccountHead(request);

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

        [HttpPost("UpsertAccountHead")]
        public async Task<IActionResult> UpsertAccountHead([FromBody] AccountRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = CompanyId;
                }
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = SessionId;
                }
                req.UserID = UserId;
                var result = _accountHeadService.UpsertAccountHead(req);
                return Ok(new { result });
            }
            catch (System.Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetAccountParentDropdownList")]
        public IActionResult GetAccountParentDropdownList(int? excludeAccountID = null, int? companyId = null, int? sessionId = null)
        {
            try
            {
                var result = _accountHeadService.GetAccountParentDropdownList(excludeAccountID, companyId, sessionId);

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

        [HttpPost("GetAllListAccountHeadWithPageIndex")]
        public async Task<IActionResult> GetAllListAccountHeadWithPageIndex([FromBody] SearchAccountHeadRequest request)
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
                    return Ok(ApiResponse<List<AccountHeadModel>>.SuccessResponse(new List<AccountHeadModel>()));

                var data = await _accountHeadService.GetAllListAccountHeadWithPageIndex(request);
                return Ok(ApiResponse<PagedResult<AccountHeadModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("UpsertAccountHeadAttachmentFile")]
        public IActionResult UpsertAccountHeadAttachmentFile([FromBody] AccountEntryAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _accountHeadService.UpsertAccountHeadAttachmentFile(req, UserId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }
    }
}
