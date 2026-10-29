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
    public class FeeDiscountController : ControllerBase
    {
        private readonly IFeeDiscountService _service;

        public FeeDiscountController(IFeeDiscountService service)
        {
            _service = service;
        }

        // ---------------------------------------------------------
        // Concession Dropdown
        // ---------------------------------------------------------
        [HttpGet("ConcessionDropdown")]
        public async Task<IActionResult> ConcessionDropdown(int? companyId)
        {
            try
            {
                var result = await _service
                    .GetFeeConcessionDropdownAsync(companyId);

                return Ok(new ApiResponse<List<DropdownModel>>
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
                    message = "Unable to get concession dropdown.",
                    technicalMessage = ex.Message
                });
            }
        }


        // ---------------------------------------------------------
        // Discount By User Dropdown
        // ---------------------------------------------------------
        [HttpGet("DiscountByDropdown")]
        public async Task<IActionResult> DiscountByDropdown(int? companyId)
        {
            try
            {
                var result = await _service
                    .GetFeeDiscountByUserDropdownAsync(companyId);

                return Ok(new ApiResponse<List<DropdownModel>>
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


        // ---------------------------------------------------------
        // Get Student Fee Discount Rows
        // ---------------------------------------------------------
        [HttpGet("GetRows")]
        public async Task<IActionResult> GetRows(int studentId, int companyId, int sessionId)
        {
            try
            {
                var request = new StudentFeeDiscountRowsRequest
                {
                    STUDENTID= studentId,
                    COMPANYID= companyId,
                    SESSIONID = sessionId
                };
                var result = await _service
                    .GetStudentFeeDiscountRowsAsync(request);

                return Ok(new ApiResponse<List<StudentFeeDiscountRowResponse>>
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
                    message = "Unable to get student fee discount rows.",
                    technicalMessage = ex.Message
                });
            }
        }


        // ---------------------------------------------------------
        // Get Student Fee Discount Header
        // ---------------------------------------------------------
        [HttpPost("GetHeader")]
        public async Task<IActionResult> GetHeader(
            [FromBody] StudentFeeDiscountHeaderRequest request)
        {
            try
            {
                var result = await _service
                    .GetStudentFeeDiscountHeaderAsync(request);

                return Ok(new ApiResponse<StudentFeeDiscountHeaderResponse>
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
                    message = "Unable to get student fee discount header.",
                    technicalMessage = ex.Message
                });
            }
        }


        // ---------------------------------------------------------
        // Save Student Fee Discount
        // ---------------------------------------------------------
        [HttpPost("Save")]
        public async Task<IActionResult> Save(
            [FromBody] StudentFeeDiscountSaveRequest request)
        {
            try
            {
                var result = await _service
                    .SaveStudentFeeDiscountAsync(request);

                return Ok(new ApiResponse<StudentFeeDiscountSaveResponse>
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
                    message = "Unable to save student fee discount.",
                    technicalMessage = ex.Message
                });
            }
        }
    }
}
