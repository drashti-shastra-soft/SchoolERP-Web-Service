using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Collections.Generic;
using System.Data;
using static System.Collections.Specialized.BitVector32;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing school classes, such as saving, updating, or deleting class records in the database.
    /// </summary>
    public class ClassService: IClassService
    {
        private readonly IConfiguration _configuration;
        public ClassService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ─── CLASS ─────────────────────────────────────────────

        /// <summary>
        /// Retrieves all classes for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="includeDeleted">Whether to include deleted records.</param>
        /// <returns>List of classes.</returns>
        public List<MstClassViewModel> GetAllClasses(
            int companyId,
            int sessionId,
            bool includeDeleted = false,int? staffID=null)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionID", sessionId);            
            parameters.Add("@IncludeDeleted", includeDeleted);
            parameters.Add("@StaffID", staffID);
            var result= conn.Query<MstClassViewModel>(
                "sp_Class_GetAll",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            // If SP returned no rows at all
            if (!result.Any()) return null;

            // If SP returned rows but RESULT != 1 (failure case)
            if (result.First().Result != 1) return result;

            return result;
        }

        /// <summary>
        /// Retrieves class details by its unique ID.
        /// </summary>
        /// <param name="classId">Class ID.</param>
        /// <returns>Class details if found; otherwise null.</returns>
        public MstClassViewModel? GetClassByID(int classId)
        {
            using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@ClassID", classId);

            return conn.QueryFirstOrDefault<MstClassViewModel>(
                "sp_Class_GetByID",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Creates a new class or updates an existing class.
        /// </summary>
        /// <param name="request">Class information including assigned sections.</param>
        /// <param name="companyId">Company/School ID.</param>
        /// <param name="sessionId">Academic session ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) UpsertClass(
         MstClassUpsertRequest request,
         int companyId,
         int sessionId,
         int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                      _configuration.GetConnectionString("DefaultConnection"));

                string sectionIds = request.SectionIds != null
                    ? string.Join(",", request.SectionIds)
                    : string.Empty;

                var parameters = new DynamicParameters();
                parameters.Add("@ClassID", request.ClassID);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@ClassName", request.ClassName);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", userId);
                parameters.Add("@SectionIds", sectionIds);
                //parameters.Add("@SequenceOrder", request.SequenceOrder);
                parameters.Add("@DisplayLabel", request.DisplayLabel);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Class_Upsert",
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
        /// Deletes a class record by its unique ID.
        /// </summary>
        /// <param name="classId">Class ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) DeleteClass(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string classId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@ClassID", classId);
                parameters.Add("@UserId", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "sp_Class_Delete",
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
        /// Activates or deactivates a class.
        /// </summary>
        /// <param name="classId">Class ID.</param>
        /// <param name="isActive">Status to set.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) ToggleClassStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ClassID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Class_ToggleStatus",
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


        public async Task<PagedResult<MstClassViewModel>> GetAllClassWithPage(ClassSearchRequest req)
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


                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@SectionID", req.SectionID);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<MstClassViewModel>(
                "sp_Class_GetAllWithPagination",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstClassViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstClassViewModel>
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

        public async Task<(bool Success, string Message)> CopyClassToSession(CopyRequest req)
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
                    "SP_CLASS_COPY",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                SpResult model = new SpResult();
                model.Result = 0;
                model.Message = ex.Message;
                return (
                    Convert.ToInt32(model.Result) == 1,
                    Convert.ToString(model.Message) ?? string.Empty
                );
            }

        }

        public async Task<(bool Success, string Message)> CopyStudentHouseToSession(CopyRequest req)
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
                    "SP_MST_STUDENTHOUSE_COPY",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                SpResult model = new SpResult();
                model.Result = 0;
                model.Message = ex.Message;
                return (
                    Convert.ToInt32(model.Result) == 1,
                    Convert.ToString(model.Message) ?? string.Empty
                );
            }

        }
    }
}
