using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class HostelTypeService: IHostelTypeService
    {
        private readonly IConfiguration _configuration;
        public HostelTypeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================================================
        // UPSERT (CREATE / UPDATE)
        // ============================================================
        public async Task<ApiResponse> UpsertHostelTypeAsync(HostelTypeUpsertRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@HOSTELTYPEID", model.HostelTypeID);
                parameters.Add("@COMPANYID", model.CompanyID);
                parameters.Add("@SESSIONID", model.SessionID);
                parameters.Add("@HOSTELTYPENAME", model.HostelTypeName);
                parameters.Add("@DisplayLabel", model.DisplayLabel);
                parameters.Add("@GENDER", model.Gender);
                parameters.Add("@DESCRIPTION", model.Description);
                parameters.Add("@ISACTIVE", model.IsActive);
                parameters.Add("@USERID", model.UserID);
                parameters.Add("@IPADDRESS", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_MST_HOSTELTYPE_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.RESULT;
                    response.Message = result.MESSAGE;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                }
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save hostel type. Please try again.";
                response.TechnicalMessage = ex.Message;
            }

            return response;
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<HostelTypeModel?> GetHostelTypeByIdAsync(int hostelTypeId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@HOSTELTYPEID", hostelTypeId);

                return await conn.QueryFirstOrDefaultAsync<HostelTypeModel>(
                    "SP_TBL_MST_HOSTELTYPE_GETBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        // ============================================================
        // GET ALL (BY COMPANY & SESSION)
        // ============================================================
        public async Task<List<HostelTypeModel>> GetAllHostelTypesAsync(int companyId,int sessionId,bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@IncludeDeleted", includeDeleted);

                var result = conn.Query<HostelTypeModel>(
                    "SP_TBL_MST_HOSTELTYPE_GETALL",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        public async Task<PagedResult<HostelTypeModel>> GetAllHostelTypeWithPage(HostelTypeSearchRequest req)
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
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HostelTypeModel>(
                "SP_TBL_MST_HOSTELTYPE_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HostelTypeModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HostelTypeModel>
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

        /// <summary>
        /// Deletes a Hostel Type record by its unique ID.
        /// </summary>
        /// <param name="HostelTypeId">HostelType ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) DeleteHostelType(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string hostelTypeID = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@HOSTELTYPEID", hostelTypeID);
                parameters.Add("@UserId", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_HOSTELTYPE_DELETE",
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
        public (bool success, string message) ToggleHostelTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@HOSTELTYPEID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_HOSTELTYPE_TOGGLESTATUS",
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


        public async Task<(bool Success, string Message)> CopyHostelTypeToSession(CopyRequest req)
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
                    "SP_MST_HOSTELTYPE_COPYSESSION",
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
