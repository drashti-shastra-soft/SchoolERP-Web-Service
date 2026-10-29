using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeOpeningAPIController : ControllerBase
    {
        private readonly IFeeOpeningBalanceService _service;

        public FeeOpeningAPIController(
            IFeeOpeningBalanceService service)
        {
            _service = service;
        }

        // =========================================================
        // GET
        // =========================================================
        [HttpPost("GetOpeningBalance")]
        public async Task<IActionResult> GetOpeningBalance(
            [FromBody] StudentFeeOpeningBalanceGetRequest request)
        {
            try
            {
                var result = await _service
                    .GetOpeningBalanceAsync(request);

                return Ok(new
                {
                    result = 1,
                    message = "SUCCESS",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    result = 0,
                    message = "Unable to get opening balance.",
                    technicalMessage = ex.Message
                });
            }
        }


        // =========================================================
        // SAVE
        // =========================================================
        [HttpPost("Save")]
        public async Task<IActionResult> Save(
            [FromBody] StudentFeeOpeningBalanceSaveRequest request)
        {
            try
            {
                var result = await _service
                    .SaveOpeningBalanceAsync(request);

                return Ok(new
                {
                    result = 1,
                    message = "SUCCESS",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    result = 0,
                    message = "Unable to save opening balance.",
                    technicalMessage = ex.Message
                });
            }
        }


        // =========================================================
        // DELETE
        // =========================================================
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(
            [FromBody] StudentFeeOpeningBalanceDeleteRequest request)
        {
            try
            {
                var result = await _service
                    .DeleteOpeningBalanceAsync(request);

                return Ok(new
                {
                    result = 1,
                    message = "SUCCESS",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    result = 0,
                    message = "Unable to delete opening balance.",
                    technicalMessage = ex.Message
                });
            }
        }


        // ---------------------------------------------------------
        // Discount By User Dropdown
        // ---------------------------------------------------------
        [HttpGet("GetInstallmentDropdown")]
        public async Task<IActionResult> GetInstallmentDropdown(int companyId,int sessionId)
        {
            try
            {
                var result = await _service
                    .GetInstallmentDropdownAsync(companyId, sessionId);

                return Ok(new ApiResponse<List<FeeInstallmentDropdownResponse>>
                {
                    Success = true,
                    Message = "SUCCESS",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    result = 0,
                    message = "Unable to get discount user dropdown.",
                    technicalMessage = ex.Message
                });
            }
        }

        [HttpPost("GetPositionOfStudentFee")]
        public async Task<IActionResult> GetPositionOfStudentFee(
            [FromBody] PositionOfStudentFeeRequest request)
        {
            try
            {
                var result = await _service
                    .GetPositionOfStudentFeeAsync(request);

                return Ok(new ApiResponse<List<PositionOfStudentFeeResponse>>
                {
                    Success = true,
                    Message = "SUCCESS",
                    Data = result
                });
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    result = 0,
                    message = "Unable to save opening balance.",
                    technicalMessage = ex.Message
                });
            }
        }

    }
}
