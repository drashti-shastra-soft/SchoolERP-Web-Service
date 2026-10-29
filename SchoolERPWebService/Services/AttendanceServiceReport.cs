using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class AttendanceServiceReport: IAttendanceServiceReport
    {
        private readonly IConfiguration _configuration;
        private readonly IFieldService _fieldService;
        public AttendanceServiceReport(IConfiguration configuration, IFieldService fieldService)
        {
            _configuration = configuration;
            _fieldService = fieldService;
        }

        // Fixed column names returned by result set 1 — everything else
        // starting with "D" + 2 digits (D01..D31) is a dynamic day cell.
        private static readonly HashSet<string> FixedColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            "RESULT", "MESSAGE", "TOTALRECORDS", "TOTALPAGES", "CURRENTPAGE", "PAGESIZE",
            "STUDENTID", "ROLLNO", "ADMISSIONNO", "STUDENTNAME",
            "P", "L", "A", "H", "ATTENDANCEPERCENT",
            "TECHNICALMESSAGE", "ERRORLINE"
        };

        public async Task<AttendanceMonthlyPagedResult<AttendanceMonthlyResponse>> GetAttendanceMonthlyReport(AttendanceMonthlyReportRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@MONTH", req.Month);
                param.Add("@YEAR", req.Year);
                param.Add("@COMPANYID", req.CompanyId);
                param.Add("@STUDENTID", req.StudentId);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber <= 0 ? 1 : req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize <= 0 ? 10 : req.PageSize);

                using var multi = await conn.QueryMultipleAsync(
                    "SP_ATTENDANCE_MONTHLY_REPORT",
                    param,
                    commandType: CommandType.StoredProcedure);

                // ---- Result set 1: read as dynamic rows so D01..D31 (whatever
                // ---- the month actually has) come through without a fixed model.
                var rawRows = (await multi.ReadAsync()).ToList();

                var students = new List<AttendanceMonthlyResponse>();
                int result = 0, totalRecords = 0, totalPages = 0;
                int currentPage = req.PageNumber <= 0 ? 1 : req.PageNumber;
                int pageSize = req.PageSize <= 0 ? 10 : req.PageSize;

                foreach (var row in rawRows)
                {
                    var dict = (IDictionary<string, object>)row;

                    result = ToInt(GetVal(dict, "RESULT"));
                    totalRecords = ToInt(GetVal(dict, "TOTALRECORDS"));
                    totalPages = ToInt(GetVal(dict, "TOTALPAGES"));
                    currentPage = ToInt(GetVal(dict, "CURRENTPAGE"), currentPage);
                    pageSize = ToInt(GetVal(dict, "PAGESIZE"), pageSize);

                    // Validation / "no students found" rows have no STUDENTID column
                    if (!dict.ContainsKey("STUDENTID")) continue;

                    var item = new AttendanceMonthlyResponse
                    {
                        RESULT = result,
                        TOTALRECORDS = totalRecords,
                        TOTALPAGES = totalPages,
                        CURRENTPAGE = currentPage,
                        PAGESIZE = pageSize,
                        STUDENTID = ToInt(GetVal(dict, "STUDENTID")),
                        ROLLNO = GetVal(dict, "ROLLNO") as string,
                        ADMISSIONNO = GetVal(dict, "ADMISSIONNO") as string,
                        STUDENTNAME = GetVal(dict, "STUDENTNAME") as string,
                        P = ToInt(GetVal(dict, "P")),
                        L = ToInt(GetVal(dict, "L")),
                        A = ToInt(GetVal(dict, "A")),
                        H = ToInt(GetVal(dict, "H")),
                        ATTENDANCEPERCENT = ToInt(GetVal(dict, "ATTENDANCEPERCENT")),
                    };

                    foreach (var kv in dict)
                    {
                        if (FixedColumns.Contains(kv.Key)) continue;
                        if (kv.Key.Length == 3 && kv.Key.StartsWith("D", StringComparison.OrdinalIgnoreCase))
                        {
                            item.DayCells[kv.Key.ToUpperInvariant()] = kv.Value?.ToString() ?? string.Empty;
                        }
                    }

                    students.Add(item);
                }

                // ---- Result set 2: holiday calendar. Only present when the SP
                // ---- actually reached that far (i.e. RESULT == 1 path).
                var days = new List<AttendanceHolidayResponse>();
                if (result == 1 && !multi.IsConsumed)
                {
                    days = (await multi.ReadAsync<AttendanceHolidayResponse>()).ToList();
                }

                return new AttendanceMonthlyPagedResult<AttendanceMonthlyResponse>
                {
                    Data = result == 1 ? students : new List<AttendanceMonthlyResponse>(),
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageNumber = currentPage,
                    PageSize = pageSize,
                    Days = days
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static int ToInt(object val, int fallback = 0)
        {
            if (val == null || val == DBNull.Value) return fallback;
            return Convert.ToInt32(val);
        }

        // GetValueOrDefault (System.Collections.Generic.CollectionExtensions) only
        // targets IReadOnlyDictionary<TKey,TValue>/Dictionary<TKey,TValue>, not the
        // plain IDictionary<string, object> that ExpandoObject rows get cast to
        // (CS1061). TryGetValue is a real member on IDictionary, so use that instead.
        private static object GetVal(IDictionary<string, object> dict, string key)
        {
            return dict.TryGetValue(key, out var val) ? val : null;
        }

        public async Task<PagedResult<AttendanceTypeReportResponse>> GetAttendanceTypeReport(StudentReportRequest req)
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
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@SEARCHTYPE", req.SearchType);
                param.Add("@ATTENDANCETYPE", req.AttendanceType);
                param.Add("@FROMDATE", req.DateFrom);
                param.Add("@TODATE", req.DateTo);
                param.Add("@SEARCHKEYWORD", req.SearchTerm);
                param.Add("@ONLYWITHRECORDS", 0);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<AttendanceTypeReportResponse>(
                "SP_ATTENDANCE_TYPE_REPORT",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<AttendanceTypeReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AttendanceTypeReportResponse>
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

        public async Task<PagedResult<AttendanceDailyReportResponse>> GetAttendanceDailyReport(StudentReportRequest req)
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
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@DATE", req.DateFrom);
                param.Add("@SEARCHKEYWORD", req.SearchTerm);
                param.Add("@ONLYWITHRECORDS", 0);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<AttendanceDailyReportResponse>(
                "SP_ATTENDANCE_DAILY_REPORT",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<AttendanceDailyReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AttendanceDailyReportResponse>
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

        public async Task<PagedResult<AttendanceDayWiseReportResponse>> GetAttendanceDailyWiseReport(StudentReportRequest req)
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
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@DATE", req.DateFrom);
                param.Add("@SOURCE", req.Source);
                param.Add("@SEARCHKEYWORD", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<AttendanceDayWiseReportResponse>(
                "SP_ATTENDANCE_DAYWISE_REPORT",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<AttendanceDayWiseReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AttendanceDayWiseReportResponse>
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

        public async Task<AttendanceMonthlyPagedResult<AttendanceStaffMonthlyResponse>> GetAttendanceStaffMonthlyReport(AttendanceMonthlyReportRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                param.Add("@COMPANYID", req.CompanyId);
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@MONTH", req.Month);
                param.Add("@YEAR", req.Year);                
                param.Add("@STAFFID", req.StaffId);
                param.Add("@ROLEID", req.RoleId);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber <= 0 ? 1 : req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize <= 0 ? 10 : req.PageSize);

                using var multi = await conn.QueryMultipleAsync(
                    "SP_HR_STAFFATTENDANCE_MONTHLY_REPORT",
                    param,
                    commandType: CommandType.StoredProcedure);

                // ---- Result set 1: read as dynamic rows so D01..D31 (whatever
                // ---- the month actually has) come through without a fixed model.
                var rawRows = (await multi.ReadAsync()).ToList();

                var students = new List<AttendanceStaffMonthlyResponse>();
                int result = 0, totalRecords = 0, totalPages = 0;
                int currentPage = req.PageNumber <= 0 ? 1 : req.PageNumber;
                int pageSize = req.PageSize <= 0 ? 10 : req.PageSize;

                foreach (var row in rawRows)
                {
                    var dict = (IDictionary<string, object>)row;

                    result = ToInt(GetVal(dict, "RESULT"));
                    totalRecords = ToInt(GetVal(dict, "TOTALRECORDS"));
                    totalPages = ToInt(GetVal(dict, "TOTALPAGES"));
                    currentPage = ToInt(GetVal(dict, "CURRENTPAGE"), currentPage);
                    pageSize = ToInt(GetVal(dict, "PAGESIZE"), pageSize);

                    // Validation / "no students found" rows have no STAFFID column
                    if (!dict.ContainsKey("STAFFID")) continue;

                    var item = new AttendanceStaffMonthlyResponse
                    {
                        RESULT = result,
                        TOTALRECORDS = totalRecords,
                        TOTALPAGES = totalPages,
                        CURRENTPAGE = currentPage,
                        PAGESIZE = pageSize,
                        STAFFID = ToInt(GetVal(dict, "STAFFID")),
                        STAFFCODE = GetVal(dict, "STAFFCODE") as string,
                        STAFFNAME = GetVal(dict, "STAFFNAME") as string,
                        ROLENAME = GetVal(dict, "ROLENAME") as string,
                        P = ToInt(GetVal(dict, "P")),
                        L = ToInt(GetVal(dict, "L")),
                        A = ToInt(GetVal(dict, "A")),
                        H = ToInt(GetVal(dict, "H")),
                        ATTENDANCEPERCENT = ToInt(GetVal(dict, "ATTENDANCEPERCENT")),
                    };

                    foreach (var kv in dict)
                    {
                        if (FixedColumns.Contains(kv.Key)) continue;
                        if (kv.Key.Length == 3 && kv.Key.StartsWith("D", StringComparison.OrdinalIgnoreCase))
                        {
                            item.DayCells[kv.Key.ToUpperInvariant()] = kv.Value?.ToString() ?? string.Empty;
                        }
                    }

                    students.Add(item);
                }

                // ---- Result set 2: holiday calendar. Only present when the SP
                // ---- actually reached that far (i.e. RESULT == 1 path).
                var days = new List<AttendanceHolidayResponse>();
                if (result == 1 && !multi.IsConsumed)
                {
                    days = (await multi.ReadAsync<AttendanceHolidayResponse>()).ToList();
                }

                return new AttendanceMonthlyPagedResult<AttendanceStaffMonthlyResponse>
                {
                    Data = result == 1 ? students : new List<AttendanceStaffMonthlyResponse>(),
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageNumber = currentPage,
                    PageSize = pageSize,
                    Days = days
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResult<StaffAttendanceDayWiseReportResponse>> GetStaffAttendanceDayWiseReport(StudentReportRequest req)
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
                param.Add("@SessionID", req.SessionId);
                param.Add("@AttendanceDate", req.DateFrom);
                param.Add("@RoleID", req.RoleID);
                param.Add("@Source", req.Source);
                param.Add("@SearchText", req.SearchTerm);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<StaffAttendanceDayWiseReportResponse>(
                "sp_HR_StaffAttendance_GetByDateWithPageIndex",
                param,
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
