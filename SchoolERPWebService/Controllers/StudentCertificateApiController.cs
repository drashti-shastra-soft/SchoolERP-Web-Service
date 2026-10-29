using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SchoolERPWebService.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentCertificateApiController : ControllerBase
    {
        private readonly IStudentCertificateService _certificateService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IUserMenuPermissionService _menuPerm;

        private const string MenuPath = "/Certificate/StudentCertificate";
        public StudentCertificateApiController(IStudentCertificateService certificateService, ICompanyService companyService, ISessionService sessionService, IUserMenuPermissionService menuPerm)
        {
            _certificateService = certificateService;
            _companyService = companyService;
            _sessionService = sessionService;
            _menuPerm = menuPerm;
        }

        /// <summary>
        /// Retrieves all student certificates for the authenticated user's current company and session.
        /// </summary>
        /// <remarks>
        /// This endpoint automatically determines the current user's active company and academic session,
        /// then returns all associated student certificate records.
        /// </remarks>
        /// <returns>
        /// A standardized API response containing the list of student certificates or an error message.
        /// </returns>
        [HttpGet("GetAll")]
        public IActionResult GetAll(int companyId, int sessionId)
        {
            try
            {
                int userId = GetCurrentUserId();
                if (companyId == 0 || companyId == null)
                {
                    companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                var data = _certificateService.GetAll(companyId, sessionId);
                return Ok(ApiResponse<List<StudentCertificateViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<List<StudentCertificateViewModel>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("GetByID/{id}")]
        public IActionResult GetByID(int id)
        {
            try
            {
                var data = _certificateService.GetByID(id);
                return Ok(ApiResponse<StudentCertificateViewModel>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<StudentCertificateViewModel>.ErrorResponse(ex.Message));
            }
        }

        //[HttpPost("Upsert")]
        //public IActionResult Upsert([FromBody] StudentCertificateUpsertRequest request)
        //{
        //    try
        //    {
        //        int userId = GetCurrentUserId();
        //        int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
        //        int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

        //        var (result, message) = _certificateService.Upsert(request, userId, companyId, sessionId);
        //        return Ok(ApiResponse<int>.SuccessResponse(result, message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponse<int>.ErrorResponse(ex.Message));
        //    }
        //}

        //[HttpPost("Delete")]
        //public IActionResult Delete(List<int> id)
        //{
        //    try
        //    {
        //        int userId = GetCurrentUserId();
        //        var (result, message) = _certificateService.Delete(id, userId);
        //        return Ok(ApiResponse<int>.SuccessResponse(result, message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponse<int>.ErrorResponse(ex.Message));
        //    }
        //}

        [HttpPost("Upsert")]
        public async Task<IActionResult> Upsert([FromBody] StudentCertificateUpsertRequest request)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                int userId = GetCurrentUserId();
                var isCreate = request.CertificateID <= 0;
                if (isCreate && !await _menuPerm.Has(User, MenuPath, "Add"))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "You do not have permission to add student certificate."
                    });
                }

                if (!isCreate && !await _menuPerm.Has(User, MenuPath, "Edit"))
                {
                    return Ok(new
                    {
                        success = false,
                        message = "You do not have permission to edit student certificate."
                    });
                }

                if (request.CompanyId == null || request.CompanyId == 0)
                {
                    request.CompanyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }

                if (request.SessionId == null || request.SessionId == 0)
                {
                    request.SessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }

                if (request.CompanyId == 0 || request.SessionId == 0)
                {
                    return BadRequest(ApiResponse<dynamic>.ErrorResponse("Current company or session not set."));
                }
                var (success, message) = _certificateService.Upsert(request: request, userId: userId, companyId: request.CompanyId, sessionId: request.SessionId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }



        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(List<int> ids)
        {
            if (!await _menuPerm.Has(User, MenuPath, "Delete"))
                return Ok(new { success = false, message = "You do not have permission to delete student certificate." });

            int userId = GetCurrentUserId();

            var (success, message) = _certificateService.Delete(ids, userId);

            return Ok(new { success, message });
        }

        //[HttpPost("ToggleStatus")]
        //public IActionResult ToggleStatus(int id, bool isActive)
        //{
        //    try
        //    {
        //        int userId = GetCurrentUserId();
        //        var (result, message) = _certificateService.ToggleStatus(id, isActive, userId);
        //        return Ok(ApiResponse<int>.SuccessResponse(result, message));
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ApiResponse<int>.ErrorResponse(ex.Message));
        //    }
        //}

        [HttpPost("ToggleStatus")]
        public async Task<IActionResult> ToggleStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                if (!await _menuPerm.Has(User, MenuPath, "Edit"))
                    return Ok(new { success = false, message = "You do not have permission to change class status." });

                int userId = GetCurrentUserId();
                var (success, message) = _certificateService.ToggleStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("Generate")]
        public IActionResult Generate(int studentId, int certificateId)
        {
            try
            {
                int userId = GetCurrentUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;
                int sessionId = _sessionService.GetUserCurrentSession(userId) ?? 0;

                var data = _certificateService.GenerateCertificate(studentId, certificateId, companyId, sessionId);
                return Ok(ApiResponse<string>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.ErrorResponse(ex.Message));
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }
    }
}
