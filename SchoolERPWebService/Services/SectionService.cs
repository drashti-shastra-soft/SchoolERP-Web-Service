using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing class sections, such as saving, updating, or deleting section records in the database.
    /// </summary>
    public class SectionService : ISectionService
    {
        private readonly IConfiguration _configuration;
        public SectionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ─── SECTION ────────────────────────────────────────────

        /// <summary>
        /// Retrieves all sections for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="includeDeleted">Whether to include deleted records.</param>
        /// <returns>List of sections.</returns>
        public List<MstSectionViewModel> GetAllSections(
            int companyId,
            int sessionId,            
            bool includeDeleted = false, int? userId=null)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionID", sessionId);
            parameters.Add("@UserId", userId);
            parameters.Add("@IncludeDeleted", includeDeleted);

            var result= conn.Query<MstSectionViewModel>(
                "sp_Sections_GetAll",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            // If SP returned no rows at all
            if (!result.Any()) return null;

            // If SP returned rows but RESULT != 1 (failure case)
            if (result.First().Result != 1) return null;

            return result;
        }

        /// <summary>
        /// Retrieves all sections assigned to a specific class.
        /// </summary>
        /// <param name="classId">Class ID.</param>
        /// <returns>List of sections for the specified class.</returns>
        public List<MstSectionViewModel> GetSectionsByClass(int classId,int? staffID=null)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@ClassID", classId);
            parameters.Add("@StaffID", staffID);

            return conn.Query<MstSectionViewModel>(
                "sp_Section_GetByClass",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        /// <summary>
        /// Retrieves section details by its unique ID.
        /// </summary>
        /// <param name="sectionId">Section ID.</param>
        /// <returns>Section details if found; otherwise null.</returns>
        public MstSectionViewModel? GetSectionByID(int sectionId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@SectionID", sectionId);

            return conn.QueryFirstOrDefault<MstSectionViewModel>(
                "sp_Sections_GetByID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Creates a new section or updates an existing section.
        /// </summary>
        /// <param name="request">Section information.</param>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) UpsertSection(
            MstSectionUpsertRequest request,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@SectionID", request.SectionID);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@SectionName", request.SectionName);
                parameters.Add("@DisplayLabel", request.DisplayLabel);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Sections_Upsert",
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
        /// Deletes a section record by its unique ID.
        /// </summary>
        /// <param name="sectionId">Section ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) DeleteSection(
            List<int> sectionId,
            int userId)
        {
            try
            {
                if (sectionId == null || !sectionId.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", sectionId);

                var parameters = new DynamicParameters();
                parameters.Add("@SectionID", id);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Sections_Delete",
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
        /// Activates or deactivates a section.
        /// </summary>
        /// <param name="sectionId">Section ID.</param>
        /// <param name="isActive">Status to set.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) ToggleSectionStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@SectionID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Sections_ToggleStatus",
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
        /// Copies selected sections to a target session.
        /// Skips duplicates automatically.
        /// </summary>
        public (bool success, string message, int inserted, int skipped) CopySectionsToSession(
            string sectionIds,
            int targetSessionId,
            int companyId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@SECTIONIDS", sectionIds);
                parameters.Add("@TARGETSESSIONID", targetSessionId);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<CopyResult>(
                    "sp_Sections_CopyToSession",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed.",
                    result?.Inserted ?? 0,
                    result?.Skipped ?? 0
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0, 0);
            }
        }


        public async Task<PagedResult<MstSectionViewModel>> GetAllSectionWithPage(HostelTypeSearchRequest req)
        {
            try
            {
                // Sanitise pagination — same guard as the SP
                if (req.PageNumber <= 0) req.PageNumber = 1;
                if (req.PageSize <= 0) req.PageSize = 10;

                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var param = new DynamicParameters();
                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                using var multi = await conn.QueryMultipleAsync(
                    "sp_Sections_GetAllPAGEINDEX", param,
                    commandType: CommandType.StoredProcedure);

                // Result set 1 — pagination meta from SP
                var meta = await multi.ReadFirstOrDefaultAsync<PaginationMeta>();

                // Result set 2 — actual section rows
                var sections = (await multi.ReadAsync<MstSectionViewModel>()).ToList();

                return new PagedResult<MstSectionViewModel>
                {
                    Data = sections,
                    TotalRecords = meta?.TotalRecords ?? 0,   
                    PageNumber = meta?.PageNumber ?? req.PageNumber.Value,
                    PageSize = meta?.PageSize ?? req.PageSize,
                    //TotalPages = meta?.TotalPages ?? 0         
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
