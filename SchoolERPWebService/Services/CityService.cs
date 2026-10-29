using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class CityService: ICityService
    {
        private readonly IConfiguration _configuration;
        public CityService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /*****************************************************************************************
        SERVICE NAME    : CityService

        DESCRIPTION     : Calls SP_TBL_MST_City_UPSERT using Dapper and returns the procedure
                          result to the controller.

        FUNCTIONS       :
        - UpsertCityAsync : Inserts or updates city master data
        - Passes all parameters to the stored procedure
        - Reads RESULT and MESSAGE returned by SQL
        - Returns success or failure response
        *****************************************************************************************/
        public async Task<CityApiResponse> UpsertCityAsync(CityUpsertRequest model)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CityId", model.CityId, DbType.Int32);
                parameters.Add("@CityName", model.CityName, DbType.String);
                parameters.Add("@DisplayLabel", model.DisplayLabel, DbType.String);
                parameters.Add("@StateId", model.StateId, DbType.Int32);
                parameters.Add("@CountryId", model.CountryId, DbType.Int32);
                parameters.Add("@Description", model.Description, DbType.String);
                parameters.Add("@ISACTIVE", model.ISACTIVE, DbType.Boolean);
                parameters.Add("@USERID", model.USERID, DbType.Int32);
                parameters.Add("@IPADDRESS", model.IPADDRESS, DbType.String);

                var result = await connection.QueryFirstOrDefaultAsync<CityApiResponse>(
                    "SP_TBL_MST_City_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result ?? new CityApiResponse
                {
                    RESULT = 0,
                    MESSAGE = "No response returned."
                };
            }
            catch (Exception ex)
            {
                return new CityApiResponse
                {
                    RESULT = 0,
                    MESSAGE = ex.Message
                };
            }            
        }

        /*****************************************************************************************
        SERVICE NAME    : CityService

        DESCRIPTION     : Calls SP_TBL_MST_CITY_GETBYID using Dapper and returns city details
                          for the given CityId.

        FUNCTIONS       :
        - GetCityByIdAsync : Fetches a single city record by id
        - Passes CITYID to stored procedure
        - Maps procedure response into a strongly typed model
        *****************************************************************************************/
        public async Task<CityModel?> GetCityByIdAsync(int cityID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CITYID", cityID);

                return await conn.QueryFirstOrDefaultAsync<CityModel>(
                    "SP_TBL_MST_CITY_GETBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                var model= new CityModel();
                model.Result=0;
                model.Message=ex.Message;
                return model;                   
            }
        }

        /*****************************************************************************************
            METHOD NAME      : GetAllCityAsync

            DESCRIPTION      : Fetches all city records by company and session using Dapper.

            FUNCTIONS        :
            - Calls stored procedure SP_TBL_MST_CITY_GETALL
            - Passes CompanyID, SessionID, and IncludeDeleted
            - Returns city list in a standard response wrapper
            - Handles no-data, failure, and exception cases safely
        *****************************************************************************************/
        public async Task<CityListResponse> GetAllCityAsync(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@INCLUDEDELETED", includeDeleted);

                var result = (await conn.QueryAsync<CityModel>(
                    "SP_TBL_MST_CITY_GETALL",
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (!result.Any())
                {
                    return new CityListResponse
                    {
                        Result = 0,
                        Message = "No records found",
                        Data = new List<CityModel>()
                    };
                }

                if (result.First().Result != 1)
                {
                    return new CityListResponse
                    {
                        Result = 0,
                        Message = result.First().Message ?? "Failed",
                        Data = result
                    };
                }

                return new CityListResponse
                {
                    Result = 1,
                    Message = "Success",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new CityListResponse
                {
                    Result = 0,
                    Message = ex.Message,
                    Data = new List<CityModel>()
                };
            }
        }

        public async Task<PagedResult<CityModel>> GetAllCityWithPage(CitySearchRequest req)
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


                param.Add("@COUNTRYID", req.CountryId);
                param.Add("@STATEID", req.StateId);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);

                var result = (await conn.QueryAsync<CityModel>(
                "SP_TBL_MST_CITY_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<CityModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<CityModel>
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
        /// Deletes a City record by its unique ID.
        /// </summary>
        public (bool success, string message) DeleteCity(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string cityID = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@CITYID", cityID);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_CITY_DELETE",
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

        public (bool success, string message) ToggleCityStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CITYID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_CITY_TOGGLESTATUS",
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

        public async Task<List<CityModel>> GetAllCityByStateIdAsync(int stateId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@StateId", stateId);

                var result = conn.Query<CityModel>(
                    "SP_TBL_MST_CITY_GETALLBYSTATEWISE",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
