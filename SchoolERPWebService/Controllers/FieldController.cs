using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;

        public FieldController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        /// <summary>
        /// Retrieves all custom fields for the specified company and session.
        /// Optional filters can be applied for system fields and entity type.
        /// </summary>
        /// <param name="companyId">
        /// The unique identifier of the company.
        /// </param>
        /// <param name="sessionId">
        /// The unique identifier of the academic session.
        /// </param>
        /// <param name="isSystemField">
        /// Optional filter to retrieve only system fields or non-system fields.
        /// </param>
        /// <param name="belongsTo">
        /// Optional entity/category name to filter fields.
        /// </param>
        /// <returns>
        /// Returns a list of matching field records.
        /// </returns>
        [HttpGet("GetAllFields")]
        public IActionResult GetAllFields(
            int companyId,
            int sessionId,
            bool? isSystemField = null,
            string? belongsTo = null)
        {
            var result = _fieldService.GetAllFields(
                companyId,
                sessionId,
                isSystemField,
                belongsTo);

            return Ok(new ApiResponse<List<FieldModel>>
            {
                Success = true,
                Message = "Fields retrieved successfully.",
                Data = result
            });
        }

        /// <summary>
        /// Retrieves field details for the specified field identifier.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the field.
        /// </param>
        /// <returns>
        /// Returns the field record if found; otherwise returns null.
        /// </returns>
        [HttpGet("GetFieldByID/{id}")]
        public IActionResult GetFieldByID(int id)
        {
            var result = _fieldService.GetFieldByID(id);

            if (result == null)
            {
                return Ok(new ApiResponse<FieldModel?>
                {
                    Success = false,
                    Message = "Field not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<FieldModel?>
            {
                Success = true,
                Message = "Field retrieved successfully.",
                Data = result
            });
        }

        /// <summary>
        /// Retrieves all ID auto-generation settings for the specified company and session.
        /// </summary>
        /// <param name="companyId">
        /// The unique identifier of the company.
        /// </param>
        /// <param name="sessionId">
        /// The unique identifier of the academic session.
        /// </param>
        /// <returns>
        /// Returns a list of ID auto-generation configuration settings.
        /// </returns>
        [HttpGet("GetIDAutoGenSettings")]
        public async Task<IActionResult> GetIDAutoGenSettings(
            int companyId,
            int sessionId)
        {
            var result = await _fieldService.GetIDAutoGenSettings(
                companyId,
                sessionId);

            return Ok(new ApiResponse<List<IDAutoGenSettings>>
            {
                Success = true,
                Message = "ID auto generation settings retrieved successfully.",
                Data = result
            });
        }


        /// <summary>
        /// Retrieves all custom fields for the specified company and session.
        /// Optional filters can be applied for system fields and entity type.
        /// </summary>
        /// <param name="companyId">
        /// The unique identifier of the company.
        /// </param>
        /// <param name="sessionId">
        /// The unique identifier of the academic session.
        /// </param>
        /// <param name="recordId">
        /// Optional filter to retrieve only student wise fields .
        /// </param>
        /// <param name="belongsTo">
        /// Optional entity/category name to filter fields.
        /// </param>
        /// <returns>
        /// Returns a list of matching field records.
        /// </returns>
        [HttpGet("GetByAllRecordStudetFiled")]
        public IActionResult GetByAllRecordStudetFiled(
            int companyId, int sessionId, string? belongsTo = null, int? recordId = null)
        {
            try
            {
                var result = _fieldService.GetByAllRecordStudetFiled(
                companyId,
                sessionId,
                belongsTo,
                recordId);

                return Ok(new ApiResponse<List<FieldModel>>
                {
                    Success = true,
                    Message = "Fields retrieved successfully.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}
