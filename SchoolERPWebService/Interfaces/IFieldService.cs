using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing custom fields and auto-generation settings for IDs (like Student IDs or Staff IDs).
    /// </summary>
    public interface IFieldService
    {
        /// <summary>
        /// Retrieves a list of custom fields based on school, session, and category (e.g., Student fields).
        /// </summary>
        List<FieldModel> GetAllFields(int companyId, int sessionId, bool? isSystemField = null, string belongsTo = null);

        /// <summary>
        /// Finds the details of a specific custom field using its unique ID.
        /// </summary>
        FieldModel GetFieldByID(int id);

        /// <summary>
        /// Adds a new custom field or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertField(FieldViewModel model, int userId);

        /// <summary>
        /// Removes a custom field from the system.
        /// </summary>
        (bool Success, string Message) DeleteField(List<int> id, int userId);

        /// <summary>
        /// Turns a custom field's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleFieldStatus(StatusUpdateRequest req);

        /// <summary>
        /// Retrieves the current settings for how IDs (like Student IDs) are automatically generated.
        /// </summary>
        Task<List<IDAutoGenSettings>> GetIDAutoGenSettings(int companyId, int sessionId);

        /// <summary>
        /// Saves or updates the settings for automatic ID generation.
        /// </summary>
        (bool Success, string Message) SaveIDAutoGenSettings(IDAutoGenRequest request, int userId);


        /// <summary>
        /// Get selected field values for a specific student by Student ID.
        /// </summary>
        List<FieldModel> GetByAllRecordStudetFiled(int companyId, int sessionId, string? belongsTo = null, int? recordId = null);

        Task<PagedResult<FieldModel>> GetAllFieldWithPage(FieldSearchRequestModel req);
        Task<(bool Success, string Message)> CopyField(CopyRequest req);
    }
}
