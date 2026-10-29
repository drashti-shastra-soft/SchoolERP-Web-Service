using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostalCodeAPIController : ControllerBase
    {
        private readonly IPostalCodeService _postalCodeService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/Country/Index";
        public PostalCodeAPIController(IUserMenuPermissionService menuPerm,IPostalCodeService postalCodeService)
        {
            _postalCodeService = postalCodeService;
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
        public async Task<IActionResult> Save([FromBody] PostalCodeEditModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.UserID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPAddress))
                    model.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                var response = await _postalCodeService.UpsertPostalCodeAsync(model);
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

                var response = await _postalCodeService.GetPostalCodeByIdAsync(id);
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

                var response = await _postalCodeService.GetAllPostalCodeAsync(companyId, sessionId, false);
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
        [HttpPost("GetAllPostalCodeWithPage")]
        public async Task<IActionResult> GetAllPostalCodeWithPage([FromBody] SerachPostalCode request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<PostalCodeListViewModel>>.SuccessResponse(new List<PostalCodeListViewModel>()));
                var data = await _postalCodeService.GetAllPostalCodeyWithPage(request);
                return Ok(ApiResponse<PagedResult<PostalCodeListViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                    return Ok(new { success = false, message = "You do not have permission to delete classes." });

                int userId = GetCurrentUserId();
                var (success, message) = _postalCodeService.DeletePostalCodey(ids, userId);
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
                var (success, message) = _postalCodeService.TogglePostalCodeyStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("SearchPostalCode")]
        public async Task<IActionResult> SearchPostalCode([FromQuery] string term)
        {
            try
            {
                if (term==null)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid search postal code are required."
                    });

                var response = await _postalCodeService.SearchPostalCode(term);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
