using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Data;
using static System.Collections.Specialized.BitVector32;

namespace SchoolERPWebService.Services
{
    public class BackDateSettingService:IBackDateSettingService
    {
        private readonly IConfiguration _configuration;
        public BackDateSettingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<BackDateSettingViewModel?> GetAttendanceBackDateSettingByIdAsync(int settingID)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@SettingID", settingID);

                return await conn.QueryFirstOrDefaultAsync<BackDateSettingViewModel>(
                    "SP_MST_AttendanceBackDateSetting_GETBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<ApiResponse> UpsertAttendanceBackDateSettingAsync(SaveBackDateSettingRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                //parameters.Add("@SettingID", model.SettingID);
                parameters.Add("@CompanyID", model.CompanyID);
                //parameters.Add("@RoleID", model.RoleID);
                parameters.Add("@StaffID", model.StaffID);
                parameters.Add("@ModuleType", model.ModuleType);
                parameters.Add("@RoleSettingsJson", model.RoleSettingsJson);
                //parameters.Add("@BackDaysAllowed", model.BackDaysAllowed);
                //parameters.Add("@IsUnlimited", model.IsUnlimited);
                //parameters.Add("@ISACTIVE", model.IsActive);
                parameters.Add("@USERID", model.UserID);
                parameters.Add("@IPADDRESS", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_MST_AttendanceBackDateSetting_UPSERT",
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

        public async Task<PagedResult<BackDateSettingViewModel>> GetAllAttendanceBackDateSettingWithPage(BackDateSettingSearchRequest req)
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


                param.Add("@COMPANYID", req.CompanyID);
                param.Add("@ROLEID", req.RoleId);
                param.Add("@STAFFID", req.StaffId);
                param.Add("@MODULETYPE", req.ModuleType);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);

                var result = (await conn.QueryAsync<BackDateSettingViewModel>(
                "SP_MST_AttendanceBackDateSetting_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<BackDateSettingViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<BackDateSettingViewModel>
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

        public async Task<int> GetBackDateCount(int companyId,int staffID,int roleId) 
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyId", companyId);
                parameters.Add("@StaffID", staffID);
                parameters.Add("@RoleId", roleId);

                return await conn.QueryFirstOrDefaultAsync<int>(
                    "usp_GetBackDateSetting_ByStaffRole",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BackDateSettingViewModel?> GetAttendanceBackDateSettingByStaffIdAsync(int staffId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@StaffID", staffId);

                return await conn.QueryFirstOrDefaultAsync<BackDateSettingViewModel>(
                    "SP_MST_AttendanceBackDateSetting_GETBYSTAFFID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
