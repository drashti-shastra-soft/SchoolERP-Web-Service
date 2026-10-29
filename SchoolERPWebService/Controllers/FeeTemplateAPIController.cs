using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeeTemplateAPIController : ControllerBase
    {
        private readonly IFeeTemplateService _feeTemplateService;
        private readonly IUserMenuPermissionService _menuPerm;
        public FeeTemplateAPIController(IFeeTemplateService feeTemplateService, IUserMenuPermissionService menuPerm)
        {
            _feeTemplateService = feeTemplateService;
            _menuPerm = menuPerm;
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
        [HttpPost("Save")]
        public async Task<IActionResult> Save(
       [FromBody] FeeTemplateSaveRequest request)
        {
            try
            {
                int userId = GetCurrentUserId();

                request.USERID = userId;

                // 1. Save Header
                var headerRequest = new FeeTemplateRequest
                {
                    FEETEMPLATEID = request.FEETEMPLATEID,
                    COMPANYID = request.COMPANYID,
                    SESSIONID = request.SESSIONID,
                    CLASSID = request.CLASSID,
                    SECTIONID = request.SECTIONID,
                    INSTALLMENTID = request.INSTALLMENTID,
                    ADMISSIONTYPE = request.ADMISSIONTYPE,
                    FEECATEGORYID = request.FEECATEGORYID,
                    CATEGORYID = request.CATEGORYID,
                    NOOFMONTHS = request.NOOFMONTHS,
                    LATEFEEDATE = request.LATEFEEDATE,
                    LATEFEEAMOUNT = request.LATEFEEAMOUNT,
                    REMARK = request.REMARK,
                    ISACTIVE = request.ISACTIVE,
                    USERID = userId,
                    IPADDRESS = request.IPADDRESS
                };

                var headerResult =
                    await _feeTemplateService.UpsertFeeTemplateAsync(
                        headerRequest);

                if (headerResult.RESULT != 1)
                    return Ok(headerResult);

                // For new template, get generated ID
                int templateId =
                    request.FEETEMPLATEID > 0
                        ? request.FEETEMPLATEID
                        : headerResult.FEETEMPLATEID ?? 0;

                // 2. Save Details
                var detailResult =
                    await _feeTemplateService.UpsertFeeTemplateDetailsAsync(
                        templateId,
                        request.COMPANYID,
                        request.SESSIONID,
                        request.DETAILS,
                        userId,
                        request.IPADDRESS);

                return Ok(detailResult);
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse
                {
                    Result = 0,
                    Message = "Unable to save fee template.",
                    TechnicalMessage = ex.Message
                });
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(
    int feeTemplateId,
    int companyId,
    int sessionId)
        {
            try
            {
                var result = await _feeTemplateService.GetFeeTemplateByIdAsync(
                    feeTemplateId,
                    companyId,
                    sessionId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new FeeTemplateGetByIdResponse
                {
                    RESULT = 0,
                    MESSAGE = "UNABLE TO FETCH FEE TEMPLATE.",
                    TECHNICALMESSAGE = ex.Message
                });
            }
        }

        [HttpPost("GetPageIndex")]
        public async Task<IActionResult> GetPageIndex(
    [FromBody] FeeTemplatePageRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Ok(new FeeTemplatePageApiResponse
                    {
                        RESULT = 0,
                        MESSAGE = "INVALID REQUEST."
                    });
                }

                var result =
                    await _feeTemplateService.GetFeeTemplatePageAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new FeeTemplatePageApiResponse
                {
                    RESULT = 0,
                    MESSAGE = "UNABLE TO FETCH FEE TEMPLATE LIST.",
                    TECHNICALMESSAGE = ex.Message
                });
            }
        }


        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            try
            {
                int userId = GetCurrentUserId();
                string IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var (success, message) = _feeTemplateService.DeleteFeeTemplet(ids, userId, IPAddress);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        [HttpPost("GetFeeTemplateByClassSection")]
        public async Task<IActionResult> GetFeeTemplateByClassSection(
    [FromBody] FeeTemplateGetByClassSectionRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Ok(new FeeTemplatePageApiResponse
                    {
                        RESULT = 0,
                        MESSAGE = "INVALID REQUEST."
                    });
                }

                var result =
                    await _feeTemplateService.GetFeeTemplateByClassSectionAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new FeeTemplatePageApiResponse
                {
                    RESULT = 0,
                    MESSAGE = "UNABLE TO FETCH FEE TEMPLATE LIST.",
                    TECHNICALMESSAGE = ex.Message
                });
            }
        }

        [HttpGet("GetFeeTemplateDetails/{feeTemplateId:int}")]
        public async Task<IActionResult> GetFeeTemplateDetails(int feeTemplateId)
        {
            try
            {
                var result = await _feeTemplateService
                    .GetFeeTemplateDetailsAsync(feeTemplateId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        result = 0,
                        message = "FEE TEMPLATE NOT FOUND",
                        data = (object?)null
                    });
                }
                return Ok(result);
                //return Ok(new
                //{
                //    result = 1,
                //    message = "SUCCESS",
                //    data = result
                //});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    result = 0,
                    message = "ERROR",
                    technicalMessage = ex.Message
                });
            }
        }

        [HttpGet("GetStudentInstallmentSummary")]
        public async Task<IActionResult> GetStudentInstallmentSummary(int studentId, int companyId, int sessionId)
        {
            try
            {
                

                var result =
                    await _feeTemplateService.GetStudentInstallmentSummary(studentId, companyId, sessionId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new StudentInstallmentSummary());
            }
        }


        [HttpPost("GetPaymentModeSummary")]
        public async Task<IActionResult> GetPaymentModeSummary(
    [FromBody] StudentFeeCollectionRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Ok(new PaymentModeSummary
                    {
                        RESULT = 0,
                        MESSAGE = "INVALID REQUEST."
                    });
                }

                var result =
                    await _feeTemplateService.GetPaymentModeSummary(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(new PaymentModeSummary
                {
                    RESULT = 0,
                    MESSAGE = "UNABLE TO FETCH FEE TEMPLATE LIST.",
                    TECHNICALMESSAGE = ex.Message
                });
            }
        }
    }
}
