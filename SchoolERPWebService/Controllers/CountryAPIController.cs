using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CountryAPIController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Country/Index";

        public CountryAPIController(ICountryService countryService, IUserMenuPermissionService menuPerm)
        {
            _countryService = countryService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
        // ------------------------------------------------------------
        // POST : api/Country/Save
        // INSERT OR UPDATE
        // ------------------------------------------------------------
        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromBody] CountryRequestModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _countryService.UpsertCountryAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // ------------------------------------------------------------
        // GET : api/Country/GetById/{id}
        // ------------------------------------------------------------
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _countryService.GetCountryByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // ------------------------------------------------------------
        // GET : api/Country/GetAll?companyId=1&sessionId=1
        // ------------------------------------------------------------
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int companyId, [FromQuery] int sessionId)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid Company ID and Session ID are required."
                    });

                var response = await _countryService.GetAllCountryAsync(companyId, sessionId, false);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ------------------------------------------------------------
        // POST : api/Country/GetAllCountryWithPage
        // ------------------------------------------------------------

        [HttpPost("GetAllCountryWithPage")]
        public async Task<IActionResult> GetAllCountryWithPage([FromBody] HostelTypeSearchRequest request)
        {
            if (request.SessionID == 0)
                      return Ok(ApiResponse<List<CountryModel>>.SuccessResponse(new List<CountryModel>()));
                var data = await _countryService.GetAllCountryWithPage(request);
            return Ok(ApiResponse<PagedResult<CountryModel>>.SuccessResponse(data));
        }
        

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete classes." });

                int userId = GetCurrentUserId();
                var (success, message) = _countryService.DeleteCountry(ids, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetCurrentUserId();
                var (success, message) = _countryService.ToggleCountryStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

    }
}
