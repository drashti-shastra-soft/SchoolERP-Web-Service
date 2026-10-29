using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.Reflection;

namespace SchoolERPWebService.Services
{
    public class LessonPlanReportService:ILessonPlanReportService
    {
        private readonly IConfiguration _configuration;
        private readonly IFieldService _fieldService;
        public LessonPlanReportService(IConfiguration configuration, IFieldService fieldService)
        {
            _configuration = configuration;
            _fieldService = fieldService;
        }

        public async Task<PagedResult<StaffAttendanceDayWiseReportResponse>> GetStaffAttendanceDayWiseReport(SyllabusStatusRequest model)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                if (model.PageNumber == 0 && model.PageSize == 0)
                {
                    model.PageNumber = 1;
                    model.PageSize = 10;
                }


                parameters.Add("@CompanyID", model.CompanyID);
                parameters.Add("@SessionID", model.SessionID);
                parameters.Add("@ClassID", model.ClassID);
                parameters.Add("@SectionID", model.SectionID);
                parameters.Add("@SubjectGroupID", model.SubjectGroupID);
                parameters.Add("@SubjectID", model.SubjectID);
                parameters.Add("@IncludeDeleted", model.IncludeDeleted);
                parameters.Add("@UserID", model.UserID);
                parameters.Add("@IPAddress", model.IPAddress);
                parameters.Add("@SEARCHKEYWORD", model.SEARCHKEYWORD);

                var result = (await conn.QueryAsync<StaffAttendanceDayWiseReportResponse>(
                "sp_HR_StaffAttendance_GetByDateWithPageIndex",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageNumber ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<StaffAttendanceDayWiseReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StaffAttendanceDayWiseReportResponse>
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
