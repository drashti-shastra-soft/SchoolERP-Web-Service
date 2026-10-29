using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class WeeklyHolidaysSettingService: IWeeklyHolidaysSettingService
    {
        private readonly IConfiguration _configuration;
        public WeeklyHolidaysSettingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================================================
        // UPSERT (CREATE / UPDATE)
        // ============================================================
        public async Task<ApiResponse> UpsertWeeklyHolidaysSettingAsync(WeeklyHolidayBatchRequest req)
        {
            var response = new ApiResponse();
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", req.CompanyID, DbType.Int32);

                parameters.Add("@IsSunday", req.IsSunday, DbType.Boolean);
                parameters.Add("@SundayNthWeek", req.SundayNthWeek, DbType.String, size: 20);

                parameters.Add("@IsMonday", req.IsMonday, DbType.Boolean);
                parameters.Add("@MondayNthWeek", req.MondayNthWeek, DbType.String, size: 20);

                parameters.Add("@IsTuesday", req.IsTuesday, DbType.Boolean);
                parameters.Add("@TuesdayNthWeek", req.TuesdayNthWeek, DbType.String, size: 20);

                parameters.Add("@IsWednesday", req.IsWednesday, DbType.Boolean);
                parameters.Add("@WednesdayNthWeek", req.WednesdayNthWeek, DbType.String, size: 20);

                parameters.Add("@IsThursday", req.IsThursday, DbType.Boolean);
                parameters.Add("@ThursdayNthWeek", req.ThursdayNthWeek, DbType.String, size: 20);

                parameters.Add("@IsFriday", req.IsFriday, DbType.Boolean);
                parameters.Add("@FridayNthWeek", req.FridayNthWeek, DbType.String, size: 20);

                parameters.Add("@IsSaturday", req.IsSaturday, DbType.Boolean);
                parameters.Add("@SaturdayNthWeek", req.SaturdayNthWeek, DbType.String, size: 20);

                parameters.Add("@EffectiveFrom", req.EffectiveFrom, DbType.DateTime);
                parameters.Add("@ISACTIVE", req.IsActive, DbType.Boolean);
                parameters.Add("@USERID", req.UserID, DbType.Int32);
                parameters.Add("@IPADDRESS", req.IPAddress, DbType.String, size: 50);

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_TBL_MST_Institute_Weekly_Holidays_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result != null)
                {
                    response.Result = result.Result;
                    response.Message = result.Message;
                    response.TechnicalMessage = result.TECHNICALMESSAGE;
                }
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save weekly holiday settings. Please try again.";
                response.TechnicalMessage = ex.Message;
            }
            return response;
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<WeeklyHolidaysSettingModel?> GetWeeklyHolidaysSettingByIdAsync(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ID", id);

                return await conn.QueryFirstOrDefaultAsync<WeeklyHolidaysSettingModel>(
                    "SP_TBL_MST_Institute_Weekly_Holidays_GETBYID",
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
        public async Task<List<WeeklyHolidaysSettingModel>> GetAllWeeklyHolidaysSettingAsync(int companyId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@IncludeDeleted", includeDeleted);

                var result = conn.Query<WeeklyHolidaysSettingModel>(
                    "SP_TBL_MST_Institute_Weekly_Holidays_GETALL",
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

        public async Task<PagedResult<WeeklyHolidaysSettingModel>> GetAllWeeklyHolidaysSettingWithPage(WeeklyHolidaysSettingSearchRequest req)
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
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<WeeklyHolidaysSettingModel>(
                "SP_TBL_MST_Institute_Weekly_Holidays_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<WeeklyHolidaysSettingModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<WeeklyHolidaysSettingModel>
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
        /// Deletes a Weekly Holidays Settingrecord by its unique ID.
        /// </summary>
        public (bool success, string message) DeleteWeeklyHolidaysSetting(List<int> ids, int userId)
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
                parameters.Add("@Id", id);
                parameters.Add("@UserId", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_Institute_Weekly_Holidays_DELETE",
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
        public (bool success, string message) ToggleWeeklyHolidaysSettingStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@Id", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_Institute_Weekly_Holidays_TOGGLESTATUS",
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

    }
}
