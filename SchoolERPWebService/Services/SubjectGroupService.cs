using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;
using static System.Collections.Specialized.BitVector32;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing subject groups, such as saving, updating, or deleting group records and their mappings to classes and sections.
    /// </summary>
    public class SubjectGroupService: ISubjectGroupService
    {
        private readonly IConfiguration _configuration;
        public SubjectGroupService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all subject groups for the specified company and session.
        /// Optionally includes deleted records.
        /// </summary>
        public List<MstSubjectGroupViewModel> GetAll(int companyId, int sessionId, bool includeDeleted = false)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var result= conn.Query<MstSubjectGroupViewModel>(
                "sp_SubjectGroup_GetAll",
                new
                {
                    CompanyID = companyId,
                    SessionID = sessionId,
                    IncludeDeleted = includeDeleted
                },
                commandType: CommandType.StoredProcedure
            ).ToList();

            // If SP returned no rows at all
            if (!result.Any()) return null;

            // If SP returned rows but RESULT != 1 (failure case)
            if (result.First().Result != 1) return null;

            return result;
        }

        /// <summary>
        /// Looks up the details of a specific subject group using its unique ID.
        /// </summary>
        public MstSubjectGroupViewModel? GetByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstSubjectGroupViewModel>(
                "sp_SubjectGroup_GetByID",
                new
                {
                    SubjectGroupID = id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Saves or updates a subject group record in the database, including
        /// its links to classes, sections, and subjects.
        /// </summary>
        public (bool success, string message) Upsert(
            MstSubjectGroupUpsertRequest request,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                string sectionIdsStr = request.SectionIds != null
                    ? string.Join(",", request.SectionIds)
                    : "";

                string subjectIdsStr = request.SubjectIds != null
                    ? string.Join(",", request.SubjectIds)
                    : "";

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault(
                    "sp_SubjectGroup_Upsert",
                    new
                    {
                        SubjectGroupID = request.SubjectGroupID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        Name = request.Name,
                        ClassID = request.ClassID,
                        Description = request.Description,
                        IsActive = request.IsActive,
                        UserId = userId,
                        SectionIds = sectionIdsStr,
                        SubjectIds = subjectIdsStr
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? ""
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a subject group record from the database.
        /// </summary>
        /// <param name="id">Subject Group ID.</param>
        /// <param name="userId">User performing the delete operation.</param>
        /// <returns>
        /// Returns a tuple containing:
        /// - success: Indicates whether the delete operation was successful.
        /// - message: Status or error message returned by the stored procedure.
        /// </returns>
        public (bool success, string message) Delete(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", ids);                          


                var parameters = new DynamicParameters();
                parameters.Add("@SUBJECTGROUPID", id);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_SubjectGroup_Delete",
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
        /// Updates the active or inactive status of a subject group.
        /// </summary>
        /// <param name="id">Subject Group ID.</param>
        /// <param name="isActive">New status value.</param>
        /// <param name="userId">User performing the update operation.</param>
        /// <returns>
        /// Returns a tuple containing:
        /// - success: Indicates whether the status update was successful.
        /// - message: Status or error message returned by the stored procedure.
        /// </returns>
        public (bool success, string message) ToggleStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));


                var parameters = new DynamicParameters();
                parameters.Add("@SUBJECTGROUPID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_SubjectGroup_ToggleStatus",
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

        public List<DropdownModel> GetAllSubjectByClassandSectionId(int companyId, int sessionId, int classId, int sectionId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "SP_GET_SUBJECTS_BY_CLASS_SECTION",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        ClassID = classId,
                        SectionID = sectionId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<PagedResult<MstSubjectGroupViewModel>> GetAllSubjectGroupWithPage(SubjectGroupSearchRequest req)
        {
            try
            {
                if (req.PageNumber <= 0) req.PageNumber = 1;
                if (req.PageSize <= 0) req.PageSize = 10;

                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var param = new DynamicParameters();
                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@SUBJECTID", req.SubjectId);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                

                var result = (await conn.QueryAsync<MstSubjectGroupViewModel>(
                "SP_SUBJECTGROUP_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageNumber ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstSubjectGroupViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstSubjectGroupViewModel>
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
    }
}
