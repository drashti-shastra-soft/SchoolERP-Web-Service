using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class CountryService: ICountryService
    {
        private readonly IConfiguration _configuration;
        public CountryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================================================
        // UPSERT (CREATE / UPDATE)
        // ============================================================
        public async Task<ApiResponse> UpsertCountryAsync(CountryRequestModel model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CountryId", model.CountryId);
                parameters.Add("@CountryName", model.CountryName);
                parameters.Add("@Description", model.Description);
                parameters.Add("@ISACTIVE", model.IsActive);
                parameters.Add("@USERID", model.UserID);
                parameters.Add("@IPADDRESS", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_TBL_MST_Country_UPSERT",
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
        public async Task<CountryModel?> GetCountryByIdAsync(int countryId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COUNTRYID", countryId);

                return await conn.QueryFirstOrDefaultAsync<CountryModel>(
                    "SP_TBL_MST_COUNTRY_GETBYID",
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
        public async Task<List<CountryModel>> GetAllCountryAsync(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@INCLUDEDELETED", includeDeleted);

                var result = conn.Query<CountryModel>(
                    "SP_TBL_MST_COUNTRY_GETALL",
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

        public async Task<PagedResult<CountryModel>> GetAllCountryWithPage(HostelTypeSearchRequest req)
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


                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);

                var result = (await conn.QueryAsync<CountryModel>(
                "SP_TBL_MST_COUNTRY_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<CountryModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<CountryModel>
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
        public (bool success, string message) DeleteCountry(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string countryID = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@COUNTRYID", countryID);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_COUNTRY_DELETE",
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
        public (bool success, string message) ToggleCountryStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COUNTRYID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_COUNTRY_TOGGLESTATUS",
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

        public async Task<(bool Success, string Message)> CopyCountryToSession(CopyRequest req)
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
                    "SP_MST_COUNTRY_COPYSESSION",
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
