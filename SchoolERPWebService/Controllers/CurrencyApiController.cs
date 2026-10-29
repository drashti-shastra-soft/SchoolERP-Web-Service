using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    /// <summary>
    /// This controller provides the technical endpoints for managing currency settings through the API.
    /// </summary>
    public class CurrencyApiController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyApiController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// Gets the full list of all available currencies from the system.
        /// </summary>
        [HttpGet("GetAll")]
        public IActionResult GetAll(bool includeDeleted = false)
        {
            var data = _currencyService.GetAllCurrencies(includeDeleted);
            return Ok(ApiResponse<List<MstCurrencyViewModel>>.SuccessResponse(data));
        }

        /// <summary>
        /// Gets the details of one specific currency using its unique ID number.
        /// </summary>
        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            var data = _currencyService.GetCurrencyByID(id);
            if (data == null) return NotFound(ApiResponse<MstCurrencyViewModel>.ErrorResponse("Denomination not found"));
            return Ok(ApiResponse<MstCurrencyViewModel>.SuccessResponse(data));
        }

        /// <summary>
        /// Saves a new currency or updates an existing one with the details provided.
        /// </summary>
        [HttpPost("Upsert")]
        public IActionResult Upsert([FromBody] MstCurrencyUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();
                var (success, message) = _currencyService.UpsertCurrency(request, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
           
        }

        /// <summary>
        /// Permanently removes a currency's record from the system.
        /// </summary>
        [HttpPost("Delete")]
        public IActionResult Delete(List<int> id)
        {
            int userId = GetCurrentUserId();
            var (success, message) = _currencyService.DeleteCurrency(id, userId);
            return Ok(new { success, message });
        }

        /// <summary>
        /// Turns a currency's active status on or off.
        /// </summary>
        [HttpPost("ToggleStatus")]
        public IActionResult ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();
                request.DoneBy = userId;
                var (success, message) = _currencyService.ToggleCurrencyStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        /// <summary>
        /// Contextually parses incoming bearer identities.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1; 
        }

        [HttpPost("GetAllCurrencyWithPage")]
        public async Task<IActionResult> GetAllCurrencyWithPage([FromBody] SearchRequest request)
        {
            try
            {
                request.UserId = GetCurrentUserId();
                var data = await _currencyService.GetAllCurrencyWithPage(request);
                return Ok(ApiResponse<PagedResult<MstCurrencyViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet("GetCurrencyByCompanyId")]
        public IActionResult GetCurrencyByCompanyId(int companyId)
        {
            try
            {
                var data = _currencyService.GetCurrencyByCompanyId(companyId);
                if (data == null) return NotFound(ApiResponse<CurrencyDetailsModel>.ErrorResponse("Denomination not found"));
                return Ok(ApiResponse<CurrencyDetailsModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }            
        }
    }
}
