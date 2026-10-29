using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Reflection;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CityAPIController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly IUserMenuPermissionService _menuPerm;
        private const string MenuPath = "/City/Index";

        public CityAPIController(ICityService cityService, IUserMenuPermissionService menuPerm)
        {
            _cityService = cityService;
            _menuPerm = menuPerm;
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        /*****************************************************************************************
        CONTROLLER NAME : CityAPIController

        DESCRIPTION     : Exposes API endpoint for city insert/update operations.

        ENDPOINTS       :
        - POST api/city/upsert : Accepts city request model and calls CityService

        RESPONSIBILITY  :
        - Validates request body
        - Calls service layer
        - Returns HTTP 200 for success
        - Returns HTTP 400 for business errors
        *****************************************************************************************/
        [HttpPost("Save")]
        public async Task<IActionResult> Upsert([FromBody] CityUpsertRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                model.USERID = GetCurrentUserId();
                // Capture client IP if not provided
                if (string.IsNullOrWhiteSpace(model.IPADDRESS))
                    model.IPADDRESS = HttpContext.Connection.RemoteIpAddress?.ToString();

                var res = await _cityService.UpsertCityAsync(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        /*****************************************************************************************
        CONTROLLER NAME : CityController

        DESCRIPTION     : Provides API endpoint to fetch city details by CityId.

        ENDPOINTS       :
        - GET api/city/{cityId} : Returns one city record

        RESPONSIBILITY  :
        - Accepts city id from route
        - Calls service layer
        - Returns Ok, NotFound, or BadRequest based on procedure result
        *****************************************************************************************/
        [HttpGet("GetById/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _cityService.GetCityByIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ------------------------------------------------------------
        // GET : api/CityAPI/GetAll?companyId=1&sessionId=1
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

                var response = await _cityService.GetAllCityAsync(companyId, sessionId, false);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ------------------------------------------------------------
        // POST : api/CityAPI/GetAllCityWithPage
        // ------------------------------------------------------------
        [HttpPost("GetAllCityWithPage")]
        public async Task<IActionResult> GetAllCityWithPage([FromBody] CitySearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<CityModel>>.SuccessResponse(new List<CityModel>()));
                var data = await _cityService.GetAllCityWithPage(request);
                return Ok(ApiResponse<PagedResult<CityModel>>.SuccessResponse(data));
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
                var (success, message) = _cityService.DeleteCity(ids, userId);
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
                request.DoneBy = userId;
                if (string.IsNullOrWhiteSpace(request.IpAddress))
                    request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _cityService.ToggleCityStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetAllCityByStateIdWise/{id:int}")]
        public async Task<IActionResult> GetAllCityByStateIdWise(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new ApiResponse { Result = 0, Message = "Invalid ID." });

                var response = await _cityService.GetAllCityByStateIdAsync(id);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
