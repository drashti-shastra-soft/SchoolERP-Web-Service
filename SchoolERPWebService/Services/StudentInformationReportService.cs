using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.Reflection;
using Microsoft.Extensions.Hosting;

namespace SchoolERPWebService.Services
{
    public class StudentInformationReportService: IStudentInformationReportService
    {
        private readonly IConfiguration _configuration;
        private readonly IFieldService _fieldService;
        public StudentInformationReportService(IConfiguration configuration, IFieldService fieldService)
        {
            _configuration = configuration;
            _fieldService = fieldService;
        }

        public async Task<PagedResult<StudentInformationReportModel>> GetStudentReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@STUDENTID", req.StudentId);
                param.Add("@SEARCHTERM", req.SearchTerm);

                param.Add("@GENDER", req.Gender);
                param.Add("@CATEGORYID", req.CategoryId);
                param.Add("@RTE", req.RTE);

                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                param.Add("@USERID", req.UserId);
                param.Add("@IPADDRESS", req.IpAddress);

                var result = (await conn.QueryAsync<StudentInformationReportModel>(
                "SP_STUDENT_LIST_GETStudentReport",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<StudentInformationReportModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentInformationReportModel>
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

        public async Task<PagedResult<StudentCountReportResponse>> GETStudentCountReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@SEARCHTERM", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@USERID", req.UserId);
                param.Add("@IPADDRESS", req.IpAddress);

                var result = (await conn.QueryAsync<StudentCountReportResponse>(
                "SP_STUDENT_LIST_GETStudentCountReport",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.CurrentPage ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<StudentCountReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentCountReportResponse>
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

        public async Task<PagedResult<GuardianReportResponse>> GETStudentGuardianReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@SEARCHTERM", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@USERID", req.UserId);
                param.Add("@IPADDRESS", req.IpAddress);

                var result = (await conn.QueryAsync<GuardianReportResponse>(
                "SP_STUDENT_LIST_GETGuardianReport",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.CurrentPage ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<GuardianReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<GuardianReportResponse>
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

        public async Task<PagedResult<LoginCredentialReportResponse>> GETLoginCredentialReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@USERTYPE", req.UserType);
                param.Add("@SEARCHTERM", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@USERID", req.UserId);
                param.Add("@IPADDRESS", req.IpAddress);

                var result = (await conn.QueryAsync<LoginCredentialReportResponse>(
                "SP_STUDENT_LIST_GETLoginCredentialReport",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.CurrentPage ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<LoginCredentialReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<LoginCredentialReportResponse>
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

        public async Task<PagedResult<AdmissionReportResponse>> GETAdmissionReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@SEARCHTYPE", req.SearchType);
                param.Add("@DATEFROM", req.DateFrom);
                param.Add("@DATETO", req.DateTo);
                param.Add("@SEARCHTERM", req.SearchTerm);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@USERID", req.UserId);
                param.Add("@IPADDRESS", req.IpAddress);

                var result = (await conn.QueryAsync<AdmissionReportResponse>(
                "SP_STUDENT_LIST_GET_Admission_Report",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageIndex ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<AdmissionReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AdmissionReportResponse>
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

        public async Task<PagedResult<StudentGenderRatioReportResponse>> GetStudentGenderRatioReportReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@SEARCHTERM", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@USERID", req.UserId);
                param.Add("@IPADDRESS", req.IpAddress);

                var result = (await conn.QueryAsync<StudentGenderRatioReportResponse>(
                "SP_STUDENT_LIST_GetStudentGenderRatioReport",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<StudentGenderRatioReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentGenderRatioReportResponse>
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

        public async Task<PagedResult<TimeTableClassSubjectReportResponse>> GetClassSubjectReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@SEARCHKEYWORD", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<TimeTableClassSubjectReportResponse>(
                "SP_ACD_TIMETABLE_GETBYCLASSSUBJECTREPORT",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<TimeTableClassSubjectReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<TimeTableClassSubjectReportResponse>
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

        public async Task<PagedResult<StudentTeacherRatioReportResponse>> GetStudentTeacherRatioReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@SEARCHKEYWORD", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<StudentTeacherRatioReportResponse>(
                "SP_ACD_STUDENTTEACHERRATIO_GETREPORT",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<StudentTeacherRatioReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentTeacherRatioReportResponse>
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

        public async Task<PagedResult<StudentHostelReportResponse>> GetStudentHostelReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@CLASSID", req.ClassId);
                param.Add("@SECTIONID", req.SectionId);
                param.Add("@HOSTELID", req.HostelID);
                param.Add("@SEARCHKEYWORD", req.SearchTerm);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<StudentHostelReportResponse>(
                "SP_ACD_STUDENTHOSTEL_GETREPORT",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<StudentHostelReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentHostelReportResponse>
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

        public async Task<PagedResult<StaffReportResponse>> GetStaffReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@DepartmentID", req.DepartmentID);
                param.Add("@DesignationID", req.DesignationID);
                param.Add("@RoleID", req.RoleID);
                param.Add("@SEARCHTYPE", req.SearchType);
                param.Add("@DATEFROM", req.DateFrom);
                param.Add("@DATETO", req.DateTo);
                param.Add("@SearchTerm", req.SearchTerm);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<StaffReportResponse>(
                "SP_HR_STAFF_GETALLSTAFFREPORTPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<StaffReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StaffReportResponse>
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

        public async Task<PagedResult<ItemStockReportResponse>> GetStockReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@ItemID", req.ItemID);
                param.Add("@ItemCategoryID", req.ItemCategoryID);
                param.Add("@ItemStoreID", req.ItemStoreID);
                param.Add("@ItemSupplierID", req.ItemSupplierID);
                param.Add("@SEARCHTYPE", req.SearchType);
                param.Add("@DATEFROM", req.DateFrom);
                param.Add("@DATETO", req.DateTo);
                param.Add("@SearchTerm", req.SearchTerm);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<ItemStockReportResponse>(
                "SP_INVENTORY_GETSTOCKREPORTPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<ItemStockReportResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ItemStockReportResponse>
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

        public async Task<PagedResult<AddItemResponse>> GetAddItemReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@ItemID", req.ItemID);
                param.Add("@ItemCategoryID", req.ItemCategoryID);
                param.Add("@ItemStoreID", req.ItemStoreID);
                param.Add("@ItemSupplierID", req.ItemSupplierID);
                param.Add("@SEARCHTYPE", req.SearchType);
                param.Add("@DATEFROM", req.DateFrom);
                param.Add("@DATETO", req.DateTo);
                param.Add("@SearchTerm", req.SearchTerm);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<AddItemResponse>(
                "SP_INVENTORY_GETADDITEMREPORTPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<AddItemResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AddItemResponse>
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

        public async Task<PagedResult<ItemIssueResponse>> GetIssueItemReport(StudentReportRequest req)
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
                param.Add("@SESSIONID", req.SessionId);
                param.Add("@ItemID", req.ItemID);
                param.Add("@ItemCategoryID", req.ItemCategoryID);
                param.Add("@SEARCHTYPE", req.SearchType);
                param.Add("@DATEFROM", req.DateFrom);
                param.Add("@DATETO", req.DateTo);
                param.Add("@SearchTerm", req.SearchTerm);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<ItemIssueResponse>(
                "SP_INVENTORY_GETISSUEITEMREPORTPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                var userModel = new PagedResult<ItemIssueResponse>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ItemIssueResponse>
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
