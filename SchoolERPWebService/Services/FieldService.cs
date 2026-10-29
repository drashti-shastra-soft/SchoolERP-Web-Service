using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing custom form fields and settings for automatically generating IDs (like Student Admission Numbers).
    /// </summary>
    public class FieldService: IFieldService
    {
        private readonly IConfiguration _configuration;
        public FieldService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Retrieves a complete list of all custom fields for the current school and session from the database.
        /// </summary>
        public List<FieldModel> GetAllFields(int companyId, int sessionId, bool? isSystemField = null, string belongsTo = null)
        {
            using var conn = new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));

            var param = new DynamicParameters();
            // Step 1: Pack the search criteria (School, Session, System-only, or Category).
            param.Add("@CompanyId", companyId);
            param.Add("@SessionId", sessionId);
            param.Add("@IsSystemField", isSystemField);
            param.Add("@BelongsTo", belongsTo);

            // Step 2: Ask the database for all matching fields using the 'GetAll' recipe.
            return conn.Query<FieldModel>(
                "sp_Mst_Fields_GetAll",
                param,
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Retrieves a field record based on the specified field ID.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the field.
        /// </param>
        /// <returns>
        /// Returns the matching field record if found; otherwise null.
        /// </returns>
        public FieldModel? GetFieldByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var param = new DynamicParameters();
            param.Add("@FieldId", id);

            return conn.QueryFirstOrDefault<FieldModel>(
                "sp_Mst_Fields_GetByID",
                param,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Creates a new field or updates an existing field record.
        /// </summary>
        /// <param name="model">
        /// Contains field information such as name, type, display settings,
        /// required status, and company/session details.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) UpsertField(
            FieldViewModel model,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@FieldId", model.FieldId);
                param.Add("@BelongsTo", model.BelongsTo);
                param.Add("@FieldName", model.FieldName);
                param.Add("@FieldType", model.FieldType);
                param.Add("@FieldValues", model.FieldValues);
                param.Add("@IsSystemField", model.IsSystemField);
                param.Add("@IsRequired", model.IsRequired);
                param.Add("@IsActive", model.IsActive);
                param.Add("@DisplayOrder", model.DisplayOrder);
                param.Add("@GridColumn", model.GridColumn);
                param.Add("@OnTable", model.OnTable);
                param.Add("@CompanyId", model.CompanyID);
                param.Add("@SessionId", model.SessionID);
                param.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Fields_Upsert",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response received from database.");

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes the specified field record from the database.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the field to be deleted.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteField(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string fieldIds = string.Join(",", id);

                var param = new DynamicParameters();

                param.Add("@FieldId", fieldIds);
                param.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Fields_Delete",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response received from database.");

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates or deactivates the specified field record.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the field.
        /// </param>
        /// <param name="isActive">
        /// Indicates whether the field should be active or inactive.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the status change operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) ToggleFieldStatus(StatusUpdateRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@FIELDID", req.Ids);
                parameters.Add("@ISACTIVE", req.IsActive);
                parameters.Add("@USERID", req.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_FIELDS_TOGGLESTATUS",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all ID auto-generation settings for the specified
        /// company and session.
        /// </summary>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <returns>
        /// Returns a list of ID auto-generation configuration settings.
        /// </returns>
        public async Task<List<IDAutoGenSettings>> GetIDAutoGenSettings(
            int companyId,
            int sessionId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var param = new DynamicParameters();

            param.Add("@CompanyId", companyId);
            param.Add("@SessionId", sessionId);

            var result = await conn.QueryAsync<IDAutoGenSettings>(
                "sp_Settings_IDAutoGen_Get",
                param,
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        /// <summary>
        /// Creates or updates ID auto-generation settings for the specified entity.
        /// </summary>
        /// <param name="request">
        /// Contains ID generation configuration details such as prefix,
        /// digit count, start number, and fields to include.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the save operation.
        /// </param>
        /// <returns>
        /// Returns a tuple indicating whether the operation was successful
        /// along with a status message.
        /// </returns>
        public (bool Success, string Message) SaveIDAutoGenSettings(
            IDAutoGenRequest request,
            int userId)
        {
            try
            {
                // Convert selected fields into a comma-separated string.
                var fields = request.FieldsToInclude != null
                    ? string.Join(",", request.FieldsToInclude)
                    : string.Empty;

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@EntityType", request.EntityType);
                param.Add("@IsEnabled", request.IsEnabled);
                param.Add("@Prefix", request.Prefix);
                param.Add("@DigitCount", request.DigitCount);
                param.Add("@StartNo", request.StartNo);
                param.Add("@FieldsToInclude", fields);
                param.Add("@CompanyId", request.CompanyID);
                param.Add("@SessionID", request.SessionID);
                param.Add("@UserId", userId);

                conn.Execute(
                    "sp_Settings_IDAutoGen_Upsert",
                    param,
                    commandType: CommandType.StoredProcedure);

                return (true, "Settings saved successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Get selected field values for a specific student by Student ID.
        /// </summary>
        public List<FieldModel> GetByAllRecordStudetFiled(int companyId, int sessionId, string? belongsTo = null, int? recordId = null)
        {
            try
            {
                using var conn = new SqlConnection(
            _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                // Step 1: Pack the search criteria (School, Session, System-only, or Category).
                param.Add("@CompanyId", companyId);
                param.Add("@SessionId", sessionId);
                param.Add("@BELONGSTO", belongsTo);
                param.Add("@RECORDID", recordId);

                // Step 2: Ask the database for all matching fields using the 'GetAll' recipe.
                return conn.Query<FieldModel>(
                    "SP_MST_CUSTOMFIELDVALUES_GETBYRECORD",
                    param,
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<PagedResult<FieldModel>> GetAllFieldWithPage(FieldSearchRequestModel req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }


                param.Add("@COMPANYID", req.CompanyId);
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@ISSYSTEMFIELD", req.IsSystemField);
                param.Add("@BELONGSTO", req.BelongsTo);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<FieldModel>(
                "SP_MST_FIELDS_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<FieldModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<FieldModel>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return userModel;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<(bool Success, string Message)> CopyField(CopyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROMCOMPANYID", req.FromCompanyId);
                parameters.Add("@FROMSESSIONID", req.FromSessionId);
                parameters.Add("@TOCOMPANYID", req.ToCompanyId);
                parameters.Add("@TOSESSIONID", req.ToSessionId);
                parameters.Add("@USERID", req.UserID);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_MST_FIELDS_COPYSESSION",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
