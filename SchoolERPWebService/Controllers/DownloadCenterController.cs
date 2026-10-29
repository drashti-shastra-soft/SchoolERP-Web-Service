using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Security.Claims;
using SchoolERPWebService.Services;
using Microsoft.AspNetCore.Authorization;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DownloadCenterController : ControllerBase
    {
        private readonly IDownloadCenterService _downloadCenterService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IAttendanceService _attendanceService;

        public DownloadCenterController(IDownloadCenterService downloadCenterService,
            ICompanyService companyService,
            ISessionService sessionService,
            IAttendanceService attendanceService)
        {
            _downloadCenterService = downloadCenterService;
            _companyService = companyService;
            _sessionService = sessionService;
            _attendanceService = attendanceService;
        }

        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;

        /// <summary>
        /// Retrieves all content types for the current company.
        /// Optionally filters the results using the supplied search term.
        /// </summary>
        /// <param name="searchTerm">
        /// Optional text used to filter content types by name or description.
        /// </param>
        /// <returns>
        /// A collection of content type records.
        /// </returns>
        [HttpGet("GetContentTypeList")]
        public IActionResult GetContentTypeList(string? searchTerm,int companyId)
        {
            try
            {
                if(companyId==0 || companyId == null) 
                {
                    companyId = GetCompanyId();
                }
                var result = _downloadCenterService.GetContentTypeList(companyId, searchTerm);

                return Ok(new ApiResponse<List<ContentTypeViewModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        [HttpPost("GetAllContentTypeWithPage")]
        public async Task<IActionResult> GetAllContentTypeWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<ContentTypeViewModel>>.SuccessResponse(new List<ContentTypeViewModel>()));
                var data = await _downloadCenterService.GetAllContentTypeWithPage(request);
                return Ok(ApiResponse<PagedResult<ContentTypeViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("CopyContentType")]
        public async Task<IActionResult> CopyContentType([FromBody] CopyForCompanyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = GetUserId();
                }
                if (string.IsNullOrWhiteSpace(request.IPAddress))
                    request.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                // ── Execute copy ──────────────────────────────────────────
                var result = await _downloadCenterService.CopyContentType(request);

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

        /// <summary>
        /// Retrieves a specific content type by its identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the content type.
        /// </param>
        /// <returns>
        /// The requested content type details.
        /// </returns>
        [HttpGet("GetContentTypeById")]
        public IActionResult GetContentTypeById(int id)
        {
            var result = _downloadCenterService.GetContentTypeById(id);

            return Ok(new ApiResponse<ContentTypeViewModel>
            {
                Success = result != null,
                Data = result,
                Message = result != null ? "Record found." : "Record not found."
            });
        }

        /// <summary>
        /// Creates a new content type or updates an existing content type.
        /// This method is used to save content type information.
        /// </summary>
        /// <param name="request">
        /// The content type information to be saved.
        /// </param>
        /// <returns>
        /// A response indicating whether the save operation was successful.
        /// </returns>
        [HttpPost("UpsertContentType")]
        public IActionResult UpsertContentType(ContentTypeUpsertRequest request)
        {
            try
            {
                if(request.CompanyID==0 || request.CompanyID == null) 
                {
                    request.CompanyID = GetCompanyId();
                }
                var result = _downloadCenterService.UpsertContentType(
                request,
                request.CompanyID,
                GetUserId());

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        /// <summary>
        /// Deletes the specified content type.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the content type.
        /// </param>
        /// <returns>
        /// A response indicating whether the delete operation was successful.
        /// </returns>
        [HttpPost("DeleteContentType")]
        public IActionResult DeleteContentType(List<int> id,int? companyId=null)
        {
            if(companyId==0 || companyId == null) 
            {
                companyId = GetCompanyId();
            }
            var res = _downloadCenterService.DeleteContentType(
                id,
                companyId.Value,
                GetUserId());

            return Ok(new ApiResponse<bool>
            {
                Success = res.Success,
                Message = res.Message,
                Data = res.Success
            });
        }

        /// <summary>
        /// Changes the active status of a content type.
        /// If the content type is active, it will be deactivated.
        /// If it is inactive, it will be activated.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the content type.
        /// </param>
        /// <returns>
        /// A response indicating whether the status change was successful.
        /// </returns>
        [HttpPost("ToggleContentTypeStatus")]
        public IActionResult ToggleContentTypeStatus([FromBody] StatusUpdateRequest request)
        {
            request.DoneBy = GetUserId();
            if(request.CompanyId==0 || request.CompanyId == null) 
            {
                request.CompanyId = GetCompanyId();
            }
            var result = _downloadCenterService.ToggleContentTypeStatus(request);

            return Ok(new ApiResponse<bool>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Success
            });
        }
        /// <summary>
        /// Retrieves video tutorials filtered by class, section, and search term.
        /// </summary>
        /// <param name="classId">
        /// Optional class identifier.
        /// </param>
        /// <param name="sectionId">
        /// Optional section identifier.
        /// </param>
        /// <param name="searchTerm">
        /// Optional search text.
        /// </param>
        /// <returns>
        /// A collection of video tutorials.
        /// </returns>
        [HttpGet("GetVideoTutorialList")]
        public IActionResult GetVideoTutorialList(
            int? classId,
            int? sectionId,
            string? searchTerm)
        {
            var result = _downloadCenterService
                .GetVideoTutorialList(
                GetCompanyId(),
                classId,
                sectionId,
                searchTerm);

            return Ok(new ApiResponse<List<VideoTutorialViewModel>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPost("GetAllVideoTutoriaWithPage")]
        public async Task<IActionResult> GetAllVideoTutoriaWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<VideoTutorialViewModel>>.SuccessResponse(new List<VideoTutorialViewModel>()));
                var data = await _downloadCenterService.GetAllVideoTutoriaWithPage(request);
                return Ok(ApiResponse<PagedResult<VideoTutorialViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific video tutorial by its identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the video tutorial.
        /// </param>
        /// <returns>
        /// The video tutorial details.
        /// </returns>
        [HttpGet("GetVideoTutorialById")]
        public IActionResult GetVideoTutorialById(int id)
        {
            var result = _downloadCenterService.GetVideoTutorialById(id);

            return Ok(new ApiResponse<VideoTutorialViewModel>
            {
                Success = result != null,
                Data = result,
                Message = result != null ? "Record found." : "Record not found."
            });
        }
        /// <summary>
        /// Creates a new video tutorial or updates an existing video tutorial.
        /// </summary>
        /// <param name="request">
        /// The video tutorial information to save.
        /// </param>
        /// <returns>
        /// A response indicating whether the operation was successful.
        /// </returns>
        [HttpPost("UpsertVideoTutorial")]
        public IActionResult UpsertVideoTutorial(VideoTutorialUpsertRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.CompanyID == null)
                {
                    request.CompanyID = GetCompanyId();
                }
                var result = _downloadCenterService.UpsertVideoTutorial(
                request,
                request.CompanyID.Value,
                GetUserId());

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        /// <summary>
        /// Deletes a video tutorial.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the video tutorial.
        /// </param>
        /// <returns>
        /// A response indicating whether the delete operation was successful.
        /// </returns>
        [HttpPost("DeleteVideoTutorial")]
        public IActionResult DeleteVideoTutorial(List<int> id)
        {
            var result = _downloadCenterService.DeleteVideoTutorial(
                id,
                GetUserId());

            return Ok(new ApiResponse<bool>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Success
            });
        }

        /// <summary>
        /// Changes the active status of a video tutorial.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the video tutorial.
        /// </param>
        /// <returns>
        /// A response indicating whether the status change was successful.
        /// </returns>
        [HttpPost("ToggleVideoTutorialStatus")]
        public IActionResult ToggleVideoTutorialStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                request.DoneBy = GetUserId();
                var result = _downloadCenterService.ToggleVideoTutorialStatus(request);

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        /// <summary>
        /// Retrieves uploaded content records.
        /// </summary>
        /// <param name="searchTerm">
        /// Optional text used to filter content records.
        /// </param>
        /// <returns>
        /// A collection of uploaded content records.
        /// </returns>
        [HttpGet("GetContentList")]
        public IActionResult GetContentList(string? searchTerm)
        {
            var result = _downloadCenterService.GetContentList(
                GetCompanyId(),
                searchTerm);

            return Ok(new ApiResponse<List<ContentViewModel>>
            {
                Success = true,
                Data = result
            });
        }

        [HttpPost("GetAllContentWithPage")]
        public async Task<IActionResult> GetAllContentWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<ContentViewModel>>.SuccessResponse(new List<ContentViewModel>()));
                var data = await _downloadCenterService.GetAllContentWithPage(request);
                return Ok(ApiResponse<PagedResult<ContentViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Deletes a downloadable content record.
        /// </summary>
        /// <param name="id">
        /// The content identifier.
        /// </param>
        /// <returns>
        /// A response indicating whether the delete operation succeeded.
        /// </returns>
        [HttpPost("DeleteContent")]
        public IActionResult DeleteContent(int id)
        {
            var result = _downloadCenterService.DeleteContent(
                id,
                GetUserId());

            return Ok(new ApiResponse<bool>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Success
            });
        }

        /// <summary>
        /// Generates a shareable link for the selected downloadable content items.
        /// The generated link can be shared with users to access the selected content.
        /// </summary>
        /// <param name="request">
        /// The shared link information including title, dates, and content identifiers.
        /// </param>
        /// <returns>
        /// A response containing the generated share token.
        /// </returns>
        [HttpPost("GenerateSharedLink")]
        public IActionResult GenerateSharedLink(SharedLinkUpsertRequest request)
        {
            try
            {
                if (request.CompanyID == 0 || request.CompanyID == null) 
                {
                    request.CompanyID = GetCompanyId();
                }

                var result = _downloadCenterService.GenerateSharedLink(
                request,
                request.CompanyID.Value,
                GetUserId());

                return Ok(new ApiResponse<string>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Token
                });
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        [HttpPost("GetAllSharedLinkWithPage")]
        public async Task<IActionResult> GetAllSharedLinkWithPage([FromBody] SearchRequest request)
        {
            try
            {
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<SharedLinkViewModel>>.SuccessResponse(new List<SharedLinkViewModel>()));
                var data = await _downloadCenterService.GetAllSharedLinkWithPage(request);
                return Ok(ApiResponse<PagedResult<SharedLinkViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all shared links created for downloadable content.
        /// </summary>
        /// <returns>
        /// A collection of shared link records.
        /// </returns>
        [HttpGet("GetSharedLinkList")]
        public IActionResult GetSharedLinkList()
        {
            var result = _downloadCenterService.GetSharedLinkList(GetCompanyId());

            return Ok(new ApiResponse<List<SharedLinkViewModel>>
            {
                Success = true,
                Data = result
            });
        }

        /// <summary>
        /// Deletes an existing shared link.
        /// </summary>
        /// <param name="id">
        /// The shared link identifier.
        /// </param>
        /// <returns>
        /// A response indicating whether the delete operation succeeded.
        /// </returns>
        [HttpPost("DeleteSharedLink")]
        public IActionResult DeleteSharedLink(List<int> id)
        {
            var result = _downloadCenterService.DeleteSharedLink(id, GetCompanyId());

            return Ok(new ApiResponse<bool>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Success
            });
        }

        /// <summary>
        /// Saves uploaded download center content.
        /// </summary>
        /// <param name="title">Content title.</param>
        /// <param name="typeId">Content type identifier.</param>
        /// <param name="fileType">File type.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="filePath">File path.</param>
        /// <param name="fileSize">File size.</param>
        /// <returns>Operation result.</returns>
        [HttpPost("SaveUploadContent")]
        public IActionResult SaveUploadContent(
    [FromQuery] string title,
    [FromQuery] int typeId,
    [FromQuery] string? fileType,
    [FromQuery] string? fileName,
    [FromQuery] string? filePath,
    [FromQuery] string? fileSize,
    [FromQuery] int? companyId)
        {
            try
            {
                var resolvedCompanyId = (companyId is null or 0) ? GetCompanyId() : companyId.Value;

                int userId = GetUserId();
                var result = _downloadCenterService.SaveUploadContent(
                    title,
                    typeId,
                    fileType,
                    fileName,
                    filePath,
                    fileSize,
                    resolvedCompanyId,
                    userId);

                return Ok(new ApiResponse<object>
                {
                    Success = result.Success,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }


        [HttpPost("GenerateWithSendTypeSharedLinkWith")]
        public async Task<IActionResult> GenerateWithSendTypeSharedLinkWith([FromBody] GenerateSharedLinkRequest request)
        {
            try
            {
                
                switch (request.SendToType.ToUpperInvariant())
                {
                    case "GROUP":
                        if (request.RoleIds == null || !request.RoleIds.Any())
                            return BadRequest(new { success = false, message = "Please select at least one role." });
                        break;

                    case "CLASS":
                        if (request.ClassSections == null
                            || !request.ClassSections.Any(cs => cs.SectionIds != null && cs.SectionIds.Any()))
                            return BadRequest(new { success = false, message = "Please add at least one class with sections." });
                        break;

                    case "INDIVIDUAL":
                        if (request.Individuals == null || !request.Individuals.Any())
                            return BadRequest(new { success = false, message = "Please add at least one individual." });
                        break;

                    default:
                        return BadRequest(new { success = false, message = "Invalid Send To type." });
                }

                // ---- current user/company context (adjust to your auth setup) --
                if(request.CompanyID==0 || request.CompanyID == null) 
                {
                    request.CompanyID = GetCompanyId();
                }                
                int userId = GetUserId();
                string userName = "";

                // ---- call the SP -------------------------------------------------
                var result = await _downloadCenterService.ExecuteGenerateSharedLinkSpAsync(request, request.CompanyID.Value, userId, userName);

                if (result.Success)
                {
                    return Ok(new ApiResponse<GenerateSharedLinkResult>
                    {
                        Success = true,
                        Data = result
                    });
                    //return Ok(result);
                }

                return BadRequest(new { success = false, message = result.Message ?? "Failed to generate link." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An unexpected error occurred." });
            }
        }

    }
}
