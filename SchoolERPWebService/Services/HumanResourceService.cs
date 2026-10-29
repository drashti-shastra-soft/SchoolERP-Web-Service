using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Data;
using System.Globalization;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing human resources, such as saving designations, departments, leave types, and staff records in the database.
    /// </summary>
    public class HumanResourceService : IHumanResourceService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IFieldService _fieldService;
        public HumanResourceService(IConfiguration configuration, IUserService userService, IFieldService fieldService)
        {
            _configuration = configuration;
            _userService = userService;
            _fieldService = fieldService;
        }

        // --- Designation ---
        /// <summary>
        /// Retrieves all HR designations for the specified company and session.
        /// </summary>
        public List<HRDesignationViewModel> GetAllDesignations(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRDesignationViewModel>(
                    "sp_Mst_HRDesignation_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                return new List<HRDesignationViewModel>();
            }
        }

        public async Task<PagedResult<HRDesignationViewModel>> GetAllDesignationsWithPage(SearchRequest req)
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HRDesignationViewModel>(
                "sp_Mst_HRDesignation_GetAllWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageNumber ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HRDesignationViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HRDesignationViewModel>
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
        /// Retrieves the details of a specific HR designation by its unique identifier.
        /// </summary>
        public HRDesignationViewModel? GetDesignationByID(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<HRDesignationViewModel>(
                "sp_Mst_HRDesignation_GetByID",
                new
                {
                    HRDesignationID = id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Creates a new HR designation or updates an existing designation.
        /// </summary>
        public (bool Success, string Message) UpsertDesignation(
            HRDesignationUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Mst_HRDesignation_Upsert",
                    new
                    {
                        req.HRDesignationID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        req.DesignationName,
                        req.DisplayLabel,
                        req.IsActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure
                );

                return (
                    result?.RESULT == 1,
                    result?.MESSAGE ?? "Operation completed."
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Deletes an HR designation record.
        /// </summary>
        public (bool Success, string Message) DeleteDesignation(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string hRDesignationIDs = string.Join(",", id);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_HRDesignation_Delete",
                    new
                    {
                        HRDesignationID = hRDesignationIDs,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return ((int)result.Result == 1,
                        (string)result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Changes the active status of an HR designation.
        /// </summary>
        //public (bool Success, string Message) ToggleDesignationStatus(
        //    int id,
        //    bool isActive,
        //    int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault<dynamic>(
        //            "sp_Mst_HRDesignation_ToggleStatus",
        //            new
        //            {
        //                HRDesignationID = id,
        //                IsActive = isActive,
        //                UserID = userId
        //            },
        //            commandType: CommandType.StoredProcedure
        //        );

        //        return ((int)result.Result == 1, (string)result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}


        public (bool success, string message) ToggleDesignationStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@HRDesignationID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_HRDesignation_ToggleStatus",
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

        // --- Department ---
        /// <summary>
        /// Retrieves all departments for the specified company and session.
        /// </summary>
        public DepartmentListResponse GetAllDepartments(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var data = conn.Query<HRDepartmentViewModel>(
                    "sp_Mst_Department_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // No records found
                if (data.Count == 0)
                {
                    return new DepartmentListResponse
                    {
                        Success = data[0].Success,
                        Message = data[0].Message,
                        Data = null
                    };
                }

                // Records found
                return new DepartmentListResponse
                {
                    Success = true,
                    Message = data.FirstOrDefault()?.Message ?? "SUCCESS",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new DepartmentListResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<PagedResult<HRDepartmentViewModel>> GetAllDepartmentsWithPage(SearchRequest req)
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HRDepartmentViewModel>(
                "sp_Mst_Department_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageNumber ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HRDepartmentViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HRDepartmentViewModel>
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
        /// Retrieves department details by department ID.
        /// </summary>
        public HRDepartmentViewModel? GetDepartmentByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<HRDepartmentViewModel>(
                "sp_Mst_Department_GetByID",
                new
                {
                    DepartmentID = id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Creates or updates a department record.
        /// </summary>
        public (bool Success, string Message) UpsertDepartment(
            HRDepartmentUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Department_Upsert",
                    new
                    {
                        req.DepartmentID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        req.DepartmentName,
                        req.DisplayLabel,
                        req.IsActive,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return ((int)result.Result == 1,
                        (string)result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Deletes a department record.
        /// </summary>
        public (bool Success, string Message) DeleteDepartment(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string departmentIDs = string.Join(",", id);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Department_Delete",
                    new
                    {
                        DepartmentID = departmentIDs,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return ((int)result.Result == 1,
                        (string)result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Updates the active/inactive status of a department.
        /// </summary>
        //public (bool Success, string Message) ToggleDepartmentStatus(
        //    int id,
        //    bool isActive,
        //    int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault<dynamic>(
        //            "sp_Mst_Department_ToggleStatus",
        //            new
        //            {
        //                DepartmentID = id,
        //                IsActive = isActive,
        //                UserID = userId
        //            },
        //            commandType: CommandType.StoredProcedure);

        //        return ((int)result.Result == 1,
        //                (string)result.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}

        public (bool Success, string Message) ToggleDepartmentStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@DEPARTMENTID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_Department_ToggleStatus",
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

        // --- Leave Type ---
        /// <summary>
        /// Retrieves all leave types for the specified company and session.
        /// </summary>
        public List<HRLeaveTypeViewModel> GetAllLeaveTypes(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRLeaveTypeViewModel>(
                    "sp_Mst_LeaveType_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                return new List<HRLeaveTypeViewModel>();
            }
        }

        /// <summary>
        /// Retrieves leave type details by leave type ID.
        /// </summary>
        public HRLeaveTypeViewModel? GetLeaveTypeByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<HRLeaveTypeViewModel>(
                "sp_Mst_LeaveType_GetByID",
                new
                {
                    LeaveTypeID = id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<PagedResult<HRLeaveTypeViewModel>> GetAllLeaveTypesWithPage(SearchRequest req)
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HRLeaveTypeViewModel>(
                "sp_Mst_LeaveType_GetAllWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageNumber ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HRLeaveTypeViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HRLeaveTypeViewModel>
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
        /// Creates or updates a leave type record.
        /// </summary>
        public (bool Success, string Message) UpsertLeaveType(HRLeaveTypeUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                        _configuration.GetConnectionString("DefaultConnection"));
                var result = conn.QueryFirstOrDefault<SpResult>(
                        "sp_Mst_LeaveType_Upsert",
                        new
                        {
                            req.LeaveTypeID,
                            CompanyID = companyId,
                            SessionID = sessionId,
                            req.LeaveTypeName,
                            req.DisplayLabel,
                            req.LeaveCode,
                            req.LeaveType,
                            req.FromMonth,
                            req.ToMonth,
                            req.MaxLeavePerMonth,
                            req.IsPaid,
                            req.IsActive,
                            UserID = userId
                        },
                        commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response from stored procedure.");

                return (result.Result == 1, result.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Deletes a leave type record.
        /// </summary>
        public (bool Success, string Message) DeleteLeaveType(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string leaveTypeIDs = string.Join(",", id);

                var result = conn.QueryFirstOrDefault(
                    "sp_Mst_LeaveType_Delete",
                    new
                    {
                        LeaveTypeID = leaveTypeIDs,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves leave balance details for a staff member and leave type.
        /// </summary>
        public dynamic GetLeaveBalance(
            int staffId,
            int leaveTypeId,
            int companyId,
            int sessionId)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                {
                    var staff = GetStaffByID(staffId);

                    if (staff != null)
                    {
                        if (companyId <= 0)
                            companyId = staff.CompanyID;

                        if (sessionId <= 0)
                            sessionId = staff.SessionID;
                    }
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault(
                    "sp_HR_Leave_GetBalance",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        StaffID = staffId,
                        LeaveTypeID = leaveTypeId
                    },
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    return new
                    {
                        TotalQuota = Convert.ToDecimal(result.TotalQuota),
                        TotalUsed = Convert.ToDecimal(result.TotalUsed),
                        Balance = Convert.ToDecimal(result.Balance),
                        Message = Convert.ToString(result.Message)
                    };
                }
            }
            catch
            {
            }

            return new
            {
                TotalQuota = 0m,
                TotalUsed = 0m,
                Balance = 0m,
                Message = "Unable to fetch balance"
            };
        }

        /// <summary>
        /// Retrieves all leave balances for a staff member.
        /// </summary>
        public List<dynamic> GetStaffAllLeaveBalances(
            int staffId,
            int companyId,
            int sessionId)
        {
            var result = new List<dynamic>();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var balances = conn.Query(
                    "sp_HR_Leave_GetStaffBalances",
                    new
                    {
                        StaffID = staffId
                    },
                    commandType: CommandType.StoredProcedure).ToList();

                if (balances.Any())
                {
                    foreach (var r in balances)
                    {
                        result.Add(new
                        {
                            leaveTypeID = Convert.ToInt32(r.LeaveTypeID),
                            leaveTypeName = Convert.ToString(r.LeaveTypeName),
                            totalQuota = Convert.ToDecimal(r.TotalQuota),
                            totalUsed = Convert.ToDecimal(r.TotalUsed),
                            balance = Convert.ToDecimal(r.Balance)
                        });
                    }
                }
                else
                {
                    if (companyId <= 0 || sessionId <= 0)
                    {
                        var staff = GetStaffByID(staffId);

                        if (staff != null)
                        {
                            companyId = staff.CompanyID;
                            sessionId = staff.SessionID;
                        }
                    }

                    var leaveTypes = GetAllLeaveTypes(companyId, sessionId);

                    foreach (var lt in leaveTypes)
                    {
                        var bal = GetLeaveBalance(
                            staffId,
                            lt.LeaveTypeID,
                            companyId,
                            sessionId);

                        decimal quota = (decimal)bal.TotalQuota;

                        if (quota == 0)
                        {
                            quota = GetStaffLeaveQuota(
                                companyId,
                                sessionId,
                                staffId,
                                lt.LeaveTypeID);
                        }

                        result.Add(new
                        {
                            leaveTypeID = lt.LeaveTypeID,
                            leaveTypeName = lt.LeaveTypeName,
                            totalQuota = quota,
                            totalUsed = (decimal)bal.TotalUsed,
                            balance = quota - (decimal)bal.TotalUsed
                        });
                    }
                }
            }
            catch
            {
                try
                {
                    var leaveTypes = GetAllLeaveTypes(companyId, sessionId);

                    foreach (var lt in leaveTypes)
                    {
                        result.Add(new
                        {
                            leaveTypeID = lt.LeaveTypeID,
                            leaveTypeName = lt.LeaveTypeName,
                            totalQuota = 0m,
                            totalUsed = 0m,
                            balance = 0m
                        });
                    }
                }
                catch
                {
                }
            }

            return result;
        }


        /// <summary>
        /// Toggles the active/inactive status of a leave type.
        /// </summary>
        /// <param name="id">Leave Type ID.</param>
        /// <param name="isActive">Status to set (true = Active, false = Inactive).</param>
        /// <param name="userId">User performing the action.</param>
        /// <returns>
        /// Returns Success flag and status message from the database.
        /// </returns>
        public (bool Success, string Message) ToggleLeaveTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Mst_LeaveType_ToggleStatus",
                    new
                    {
                        LEAVETYPEIDS = request.Ids,
                        IsActive = request.IsActive,
                        UserID = request.DoneBy
                    },
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

        // --- Staff ---
        public List<HRStaffViewModel> GetAllStaff(int companyId, int sessionId, int? staffId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var data = conn.Query<HRStaffViewModel>(
                    "SP_HR_STAFF_GETALL",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        IncludeDeleted = false,
                        StaffID = staffId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();

                return data;

            }
            catch
            {
                return new List<HRStaffViewModel>();
            }
        }

        public async Task<PagedResult<HRStaffViewModel>> GetAllStaffWithPage(SearchHRStaffRequest req)
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@StaffID", req.StaffID);
                param.Add("@INCLUDEDELETED", false);
                param.Add("@SearchTerm", req.SearchKeyword);
                param.Add("@DepartmentID", req.DepartmentID);
                param.Add("@DesignationID", req.DesignationID);
                param.Add("@RoleID", req.RoleID);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HRStaffViewModel>(
                "SP_HR_STAFF_GETALLPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HRStaffViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HRStaffViewModel>
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
        public async Task<PagedResult<HRStaffViewModel>> GetAllDisableStaffWithPage(SearchHRStaffRequest req)
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@StaffID", req.StaffID);
                param.Add("@INCLUDEDELETED", false);
                param.Add("@SearchTerm", req.SearchKeyword);
                param.Add("@DepartmentID", req.DepartmentID);
                param.Add("@DesignationID", req.DesignationID);
                param.Add("@RoleID", req.RoleID);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HRStaffViewModel>(
                "SP_HR_STAFF_GETALLSTAFFDISABLEPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HRStaffViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HRStaffViewModel>
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
        /// Retrieves staff details, roles, companies, leave quotas, and custom fields by Staff ID.
        /// </summary>
        /// <param name="id">Staff ID.</param>
        /// <returns>Staff information if found; otherwise null.</returns>
        public HRStaffViewModel? GetStaffByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var multi = conn.QueryMultiple(
                "sp_HR_Staff_GetByID",
                new { StaffID = id },
                commandType: CommandType.StoredProcedure);

            var staff = multi.ReadFirstOrDefault<HRStaffViewModel>();

            if (staff == null)
                return null;

            // Roles (Second Result Set)
            staff.RoleIDs = multi.Read<int>().Distinct().ToList();

            // Companies (Third Result Set)
            staff.CompanyIDs = multi.Read<int>().Distinct().ToList();

            // Leave Quotas (Fourth Result Set)
            staff.LeaveQuotas = multi.Read<HRStaffLeaveQuotaViewModel>().ToList();

            // Ensure primary company exists
            if (staff.CompanyID > 0 && !staff.CompanyIDs.Contains(staff.CompanyID))
                staff.CompanyIDs.Add(staff.CompanyID);

            if (staff.UserID.HasValue && staff.UserID > 0)
            {
                try
                {
                    // User Roles
                    var userRoles = conn.Query<int>(
                        "sp_UserRoles_GetByUser",
                        new { UserID = staff.UserID.Value },
                        commandType: CommandType.StoredProcedure);

                    foreach (var roleId in userRoles)
                    {
                        if (!staff.RoleIDs.Contains(roleId))
                            staff.RoleIDs.Add(roleId);
                    }

                    // User Companies
                    var userCompanies = conn.Query<int>(
                        "sp_UserCompanies_GetByUser",
                        new { UserID = staff.UserID.Value },
                        commandType: CommandType.StoredProcedure);

                    foreach (var companyId in userCompanies)
                    {
                        if (!staff.CompanyIDs.Contains(companyId))
                            staff.CompanyIDs.Add(companyId);
                    }

                    var allRoles = _userService.GetRoles();

                    var roleMap = allRoles.ToDictionary(
                        x => x.RoleID,
                        x => x.RoleName);

                    foreach (var roleId in staff.RoleIDs)
                    {
                        if (roleMap.TryGetValue(roleId, out var roleName)
                            && !staff.DisplayRoles.Contains(roleName))
                        {
                            staff.DisplayRoles.Add(roleName);
                        }
                    }
                }
                catch
                {
                }
            }

            try
            {
                staff.CustomFieldValues = conn.Query<StudentCustomFieldValueViewModel>(
                    "sp_HR_Staff_CustomFields_GetByStaff",
                    new { StaffID = id },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
            }

            return staff;
        }
        /// <summary>
        /// Retrieves leave quota assigned to a staff member for a specific leave type.
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="staffId">Staff ID.</param>
        /// <param name="leaveTypeId">Leave Type ID.</param>
        /// <returns>Maximum allowed leave days.</returns>
        private decimal GetStaffLeaveQuota(
            int companyId,
            int sessionId,
            int staffId,
            int leaveTypeId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<decimal>(
                    "sp_HR_StaffLeaveQuota_Get",
                    new
                    {
                        StaffID = staffId,
                        LeaveTypeID = leaveTypeId
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Inserts or updates a staff record along with documents, leave quotas,
        /// user account details, and custom field values.
        /// </summary>
        /// <param name="req">Staff request details.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <param name="userId">Logged-in user identifier.</param>
        /// <returns>
        /// Returns Success status and response message from the stored procedure.
        /// </returns>
        public StaffUpsertDTO UpsertStaff(HRStaffUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Existing mapping logic remains unchanged...

                var photoBytes = string.IsNullOrEmpty(req.PhotoBase64)
                    ? null
                    : Convert.FromBase64String(req.PhotoBase64.Split(',').Last());

                var resumeBytes = string.IsNullOrEmpty(req.ResumeBase64)
                    ? null
                    : Convert.FromBase64String(req.ResumeBase64.Split(',').Last());

                var joiningBytes = string.IsNullOrEmpty(req.JoiningLetterBase64)
                    ? null
                    : Convert.FromBase64String(req.JoiningLetterBase64.Split(',').Last());

                var resignBytes = string.IsNullOrEmpty(req.ResignationLetterBase64)
                    ? null
                    : Convert.FromBase64String(req.ResignationLetterBase64.Split(',').Last());

                var otherBytes = string.IsNullOrEmpty(req.OtherDocBase64)
                    ? null
                    : Convert.FromBase64String(req.OtherDocBase64.Split(',').Last());

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@StaffID", req.StaffID);
                param.Add("@UserID", req.UserID);
                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@StaffCode", req.StaffCode);
                param.Add("@FirstName", req.FirstName);
                param.Add("@LastName", req.LastName);
                param.Add("@FatherName", req.FatherName);
                param.Add("@MotherName", req.MotherName);
                param.Add("@Email", req.Email);
                param.Add("@Gender", req.Gender);
                param.Add("@DOB", req.DOB);
                param.Add("@DOJ", req.DOJ);
                param.Add("@MobileNo", req.MobileNo);
                param.Add("@EmergencyMobileNo", req.EmergencyMobileNo);
                param.Add("@MaritalStatus", req.MaritalStatus);

                param.Add("@PhotoDoc", null);
                param.Add("@PhotoDocType", req.PhotoDocType);
                param.Add("@PhotoDocName", req.PhotoDocName);

                param.Add("@CurrentAddress", req.CurrentAddress);
                param.Add("@PermanentAddress", req.PermanentAddress);
                param.Add("@DesignationID", req.DesignationID);
                param.Add("@DepartmentID", req.DepartmentID);
                param.Add("@Qualification", req.Qualification);
                param.Add("@WorkExperience", req.WorkExperience);
                param.Add("@Note", req.Note);
                param.Add("@EPFNo", req.EPFNo);
                param.Add("@BasicSalary", req.BasicSalary);

                param.Add("@ContractType", req.ContractType);
                param.Add("@WorkShift", req.WorkShift);
                param.Add("@WorkLocation", req.WorkLocation);

                param.Add("@CasualLeave", 0);
                param.Add("@SickLeave", 0);
                param.Add("@ImpWorkLeave", 0);

                param.Add("@AccountTitle", req.AccountTitle);
                param.Add("@BankAccountNo", req.BankAccountNo);
                param.Add("@BankName", req.BankName);
                param.Add("@IFSCCode", req.IFSCCode);
                param.Add("@BankBranchName", req.BankBranchName);

                param.Add("@FacebookURL", req.FacebookURL);
                param.Add("@TwitterURL", req.TwitterURL);
                param.Add("@LinkedinURL", req.LinkedinURL);
                param.Add("@InstagramURL", req.InstagramURL);

                param.Add("@ResumeDoc", resumeBytes, DbType.Binary);
                param.Add("@ResumeDocType", req.ResumeDocType);
                param.Add("@ResumeDocName", req.ResumeDocName);

                param.Add("@JoiningLetterDoc", joiningBytes, DbType.Binary);
                param.Add("@JoiningLetterDocType", req.JoiningLetterDocType);
                param.Add("@JoiningLetterDocName", req.JoiningLetterDocName);

                param.Add("@ResignationLetterDoc", resignBytes, DbType.Binary);
                param.Add("@ResignationLetterDocType", req.ResignationLetterDocType);
                param.Add("@ResignationLetterDocName", req.ResignationLetterDocName);

                param.Add("@OtherDoc", otherBytes, DbType.Binary);
                param.Add("@OtherDocType", req.OtherDocType);
                param.Add("@OtherDocName", req.OtherDocName);

                param.Add("@IsActive", req.IsActive);
                param.Add("@DoneBy", userId);

                param.Add("@Username", req.Username);
                param.Add("@PasswordPlain", req.PasswordPlain);
                param.Add("@UserTypeID", req.UserTypeID);
                param.Add("@RoleIDs", string.Join(",", req.RoleIDs));
                param.Add("@CompanyIDs", string.Join(",", req.CompanyIDs));

                param.Add("@DefaultRoleID", string.Join(",", req.DefaultRoleID));
                param.Add("@DashboardID", string.Join(",", req.DashboardID));
                var result = conn.QueryFirstOrDefault<StaffUpsertDTO>(
                    "sp_HR_Staff_Upsert",
                    param,
                    commandType: CommandType.StoredProcedure);



                return result;
            }
            catch (Exception ex)
            {
                var staffres = new StaffUpsertDTO();
                staffres.Result = 0;
                staffres.Message = ex.Message;
                staffres.StaffID = 0;
                return staffres;
            }
        }

        /// <summary>
        /// Inserts or updates staff leave quota details.
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="staffId">Staff ID.</param>
        /// <param name="leaveTypeId">Leave type ID.</param>
        /// <param name="maxDays">Maximum allowed leave days.</param>
        /// <param name="userId">User performing the operation.</param>
        /// <returns>
        /// Returns success status and message.
        /// </returns>
        private (bool Success, string Message) UpsertStaffLeaveQuota(
            int companyId,
            int sessionId,
            int staffId,
            int leaveTypeId,
            decimal maxDays,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                conn.Execute(
                    "sp_HR_StaffLeaveQuota_Upsert",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        StaffID = staffId,
                        LeaveTypeID = leaveTypeId,
                        MaxDays = maxDays,
                        DoneBy = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (true, "Success");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a staff record.
        /// Calls sp_HR_Staff_Delete and returns the operation result and message.
        /// </summary>
        /// <param name="id">Staff ID.</param>
        /// <param name="userId">User performing the delete operation.</param>
        /// <returns>
        /// Tuple containing:
        /// Success - Indicates whether the operation succeeded.
        /// Message - Database response message.
        /// </returns>
        public (bool Success, string Message) DeleteStaff(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string staffIDs = string.Join(",", id);
                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_HR_Staff_Delete",
                    new
                    {
                        StaffID = staffIDs,
                        DoneBy = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%";
            var random = new Random();
            return "Staff@" + new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Generates and returns the next available staff code for the specified company and session.
        /// Falls back to a timestamp-based staff code if an error occurs.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <returns>Generated staff code.</returns>
        public string GetNewStaffCode(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<string>(
                    "sp_Settings_IDAutoGen_GetNext",
                    new
                    {
                        EntityType = "Staff",
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure)
                    ?? ("STF" + DateTime.Now.Ticks.ToString().Substring(10));
            }
            catch
            {
                return "STF" + DateTime.Now.Ticks.ToString().Substring(10);
            }
        }
        /// <summary>
        /// Retrieves a staff document based on the specified document type.
        /// </summary>
        /// <param name="staffId">Staff identifier.</param>
        /// <param name="docType">Document type (Resume, JoiningLetter, ResignationLetter, Other).</param>
        /// <returns>Document bytes, file name, and content type.</returns>
        public (byte[] Bytes, string FileName, string ContentType) GetStaffDocument(int staffId, string docType)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var row = conn.QueryFirstOrDefault(
                    "sp_HR_Staff_GetByID",
                    new { StaffID = staffId },
                    commandType: CommandType.StoredProcedure);

                if (row == null)
                    return (null!, null!, null!);

                byte[] bytes = null!;
                string fileName = "";
                string contentType = "application/octet-stream";

                switch (docType.ToLower())
                {
                    case "resume":
                        bytes = row.ResumeDoc;
                        fileName = row.ResumeDocName ?? "Resume.pdf";
                        contentType = row.ResumeDocType ?? "application/pdf";
                        break;

                    case "joiningletter":
                        bytes = row.JoiningLetterDoc;
                        fileName = row.JoiningLetterDocName ?? "JoiningLetter.pdf";
                        contentType = row.JoiningLetterDocType ?? "application/pdf";
                        break;

                    case "resignationletter":
                        bytes = row.ResignationLetterDoc;
                        fileName = row.ResignationLetterDocName ?? "ResignationLetter.pdf";
                        contentType = row.ResignationLetterDocType ?? "application/pdf";
                        break;

                    case "other":
                        bytes = row.OtherDoc;
                        fileName = row.OtherDocName ?? "Document.pdf";
                        contentType = row.OtherDocType ?? "application/pdf";
                        break;
                }

                return (bytes, fileName, contentType);
            }
            catch
            {
                return (null!, null!, null!);
            }
        }
        // --- Attendance ---

        /// <summary>
        /// Retrieves staff attendance records for the specified date.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <param name="date">Attendance date.</param>
        /// <param name="roleId">Optional role identifier.</param>
        /// <returns>List of staff attendance records.</returns>
        public List<HRStaffAttendanceViewModel> GetStaffAttendance(int companyId, int sessionId, DateTime date, int? roleId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRStaffAttendanceViewModel>(
                    "sp_HR_StaffAttendance_GetByDate",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        AttendanceDate = date,
                        RoleID = roleId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<HRStaffAttendanceViewModel>();
            }
        }

        public async Task<PagedResult<HRStaffAttendanceViewModel>> GetAllStaffAttendanceWithPage(HRStaffAttendanceSearchRequest req)
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
                param.Add("@AttendanceDate", req.AttendanceDate);
                param.Add("@RoleID", req.RoleID);
                param.Add("@SearchText", req.SearchText);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HRStaffAttendanceViewModel>(
                "sp_HR_StaffAttendance_GetByDateWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HRStaffAttendanceViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HRStaffAttendanceViewModel>
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
        /// Saves or updates staff attendance details.
        /// </summary>
        /// <param name="req">Attendance information.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <param name="userId">Logged-in user identifier.</param>
        /// <returns>Success status and message.</returns>
        public (bool Success, string Message) SaveStaffAttendance(HRStaffAttendanceUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_HR_StaffAttendance_Upsert",
                    new
                    {
                        req.StaffID,
                      //  req.AttendanceDate,
                        req.Attendance,
                        req.Source,
                        req.Note,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public BulkUpsertResult SaveStaffAttendanceBulk(
    HRStaffAttendanceUpsertRequestModel request,
    int userId,
    string ipAddress)
        {
            try
            {
                // Serialize only the fields the SP expects per row — StaffID, Attendance,
                // Source, Note. System.Text.Json's default naming keeps PascalCase, which
                // matches the JSON keys read by OPENJSON's WITH clause ($.StaffID etc).
                var json = JsonSerializer.Serialize(request.AttendanceData);

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                // Two result sets come back — summary, then per-row failures —
                // so QueryMultiple is used instead of QueryFirstOrDefault.
                using var multi = conn.QueryMultiple(
                    "sp_HR_StaffAttendance_UpsertBulk",
                    new
                    {
                        request.CompanyID,
                        SessionID = request.SessionId,
                        request.AttendanceDate,
                        JsonData = json,
                        UserID = userId,
                        IPAddress = ipAddress
                    },
                    commandType: CommandType.StoredProcedure);

                var summary = multi.ReadFirstOrDefault<dynamic>();
                var failures = multi.Read<dynamic>().ToList();

                var result = new BulkUpsertResult
                {
                    Success = Convert.ToInt32(summary?.Result) == 1,
                    Message = Convert.ToString(summary?.Message) ?? string.Empty,
                    TotalRequested = Convert.ToInt32(summary?.TotalRequested ?? 0),
                    TotalSaved = Convert.ToInt32(summary?.TotalSaved ?? 0),
                    TotalFailed = Convert.ToInt32(summary?.TotalFailed ?? 0)
                };

                foreach (var row in failures)
                {
                    result.Failures.Add((
                        Convert.ToInt32(row.StaffID),
                        Convert.ToString(row.Reason) ?? string.Empty
                    ));
                }

                return result;
            }
            catch (Exception ex)
            {
                return new BulkUpsertResult
                {
                    Success = false,
                    Message = ex.Message,
                    TotalRequested = request?.AttendanceData?.Count ?? 0,
                    TotalSaved = 0,
                    TotalFailed = request?.AttendanceData?.Count ?? 0
                };
            }
        }
        // --- Payroll ---

        /// <summary>
        /// Retrieves payroll records for the specified month and year.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <param name="month">Payroll month.</param>
        /// <param name="year">Payroll year.</param>
        /// <param name="roleId">Optional role identifier.</param>
        /// <returns>Payroll list.</returns>
        public List<HRPayrollViewModel> GetAllPayroll(int companyId, int sessionId, int month, int year, int? roleId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRPayrollViewModel>(
                    "sp_HR_Payroll_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId,
                        Month = month,
                        Year = year,
                        RoleID = roleId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<HRPayrollViewModel>();
            }
        }
        /// <summary>
        /// Generates payroll for the selected staff and month.
        /// </summary>
        /// <param name="req">Payroll generation request.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <param name="userId">Logged-in user identifier.</param>
        /// <returns>Success status and message.</returns>
        public (bool Success, string Message) GeneratePayroll(HRPayrollGenerateRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_HR_Payroll_Generate",
                    new
                    {
                        req.StaffID,
                        req.Month,
                        req.Year,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Retrieves payroll generation details including staff information
        /// and attendance summary for the selected period.
        /// </summary>
        /// <param name="staffId">Staff identifier.</param>
        /// <param name="month">Payroll month.</param>
        /// <param name="year">Payroll year.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="sessionId">Session identifier.</param>
        /// <returns>Payroll generation data.</returns>
        public HRPayrollGenerationViewModel GetPayrollGenerationData(int staffId, int month, int year, int companyId, int sessionId)
        {
            var model = new HRPayrollGenerationViewModel
            {
                Staff = GetStaffByID(staffId) ?? new HRStaffViewModel(),
                Month = month,
                Year = year,
                AttendanceHistory = new List<HRAttendanceSummary>()
            };

            model.BasicSalary = model.Staff.BasicSalary;

            int prevMonth = month == 1 ? 12 : month - 1;
            int prevYear = month == 1 ? year - 1 : year;

            model.AttendanceHistory.Add(
                FetchAttendanceSummary(staffId, prevMonth, prevYear, companyId));

            model.AttendanceHistory.Add(
                FetchAttendanceSummary(staffId, month, year, companyId));

            return model;
        }
        /// <summary>
        /// Retrieves attendance summary for a staff member for the specified month and year.
        /// </summary>
        private HRAttendanceSummary FetchAttendanceSummary(int staffId, int month, int year, int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<HRAttendanceSummary>(
                    "sp_HR_Attendance_GetSummary",
                    new
                    {
                        StaffID = staffId,
                        Month = month,
                        Year = year,
                        CompanyID = companyId
                    },
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    result.Month = month;
                    result.MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);
                    result.Year = year;

                    return result;
                }
            }
            catch
            {
            }

            return new HRAttendanceSummary
            {
                Month = month,
                MonthName = DateTimeFormatInfo.CurrentInfo.GetMonthName(month),
                Year = year
            };
        }
        /// <summary>
        /// Saves payroll master record along with payroll component details.
        /// </summary>
        public (bool Success, string Message) SaveDetailedPayroll(HRPayrollSaveRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@PayrollID", dbType: DbType.Int32,
                    direction: ParameterDirection.Output);

                param.Add("@StaffID", req.StaffID);
                param.Add("@Month", req.Month);
                param.Add("@Year", req.Year);
                param.Add("@BasicSalary", req.BasicSalary);
                param.Add("@TotalEarnings", req.TotalEarnings);
                param.Add("@TotalDeductions", req.TotalDeductions);
                param.Add("@NetSalary", req.NetSalary);
                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@UserID", userId);

                conn.Execute(
                    "sp_HR_Payroll_SaveDetailed",
                    param,
                    commandType: CommandType.StoredProcedure);

                int payrollId = param.Get<int>("@PayrollID");

                if (payrollId > 0)
                {
                    foreach (var detail in req.Details)
                    {
                        conn.Execute(
                            "sp_HR_PayrollDetail_Insert",
                            new
                            {
                                PayrollID = payrollId,
                                detail.ComponentName,
                                detail.ComponentType,
                                detail.Amount,
                                UserID = userId
                            },
                            commandType: CommandType.StoredProcedure);
                    }

                    return (true, "Payroll saved successfully.");
                }

                return (false, "Failed to save payroll.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Marks payroll as paid and records payment information.
        /// </summary>
        public (bool Success, string Message) MarkAsPaid(HRPayrollPaymentRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                conn.Execute(
                    "sp_HR_Payroll_MarkAsPaid",
                    new
                    {
                        req.PayrollID,
                        req.PaymentMode,
                        req.PaymentDate,
                        req.Note,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (true, "Payment recorded successfully.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Retrieves payroll details and automatically maps result to HRPayrollViewModel using Dapper.
        /// </summary>
        public List<HRPayrollViewModel> GetPayrollList(int companyId, int month, int year)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRPayrollViewModel>(
                    "sp_HR_Payroll_Get",
                    new
                    {
                        CompanyID = companyId,
                        Month = month,
                        Year = year
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                return new List<HRPayrollViewModel>();
            }
        }
        // --- Apply Leave ---

        /// <summary>
        /// Retrieves all leave applications for the specified company and session.
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="sessionId">Session ID.</param>
        /// <returns>List of leave applications.</returns>
        public List<HRApplyLeaveViewModel> GetAllApplyLeave(int companyId, int sessionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRApplyLeaveViewModel>(
                    "sp_HR_ApplyLeave_GetAll",
                    new
                    {
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                return new List<HRApplyLeaveViewModel>();
            }
        }

        /// <summary>
        /// Retrieves all leave applications for a specific staff member.
        /// </summary>
        /// <param name="staffId">Staff ID.</param>
        /// <returns>List of leave applications.</returns>
        public List<HRApplyLeaveViewModel> GetStaffLeaves(int staffId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRApplyLeaveViewModel>(
                    "sp_HR_ApplyLeave_GetByStaff",
                    new
                    {
                        StaffID = staffId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                return new List<HRApplyLeaveViewModel>();
            }
        }

        /// <summary>
        /// Retrieves attendance history and monthly attendance summaries for a staff member.
        /// </summary>
        /// <param name="staffId">Staff ID.</param>
        /// <param name="year">Year.</param>
        /// <param name="companyId">Company ID.</param>
        /// <returns>Attendance history with monthly summaries.</returns>
        public HRAttendanceHistoryViewModel GetStaffAttendanceHistory(int staffId, int year, int companyId)
        {
            var model = new HRAttendanceHistoryViewModel();

            try
            {
                for (int m = 1; m <= 12; m++)
                {
                    model.Summaries.Add(
                        FetchAttendanceSummary(staffId, m, year, companyId));
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                model.Days = conn.Query<HRAttendanceDayStatus>(
                    "sp_HR_Attendance_GetDailyHistory",
                    new
                    {
                        StaffID = staffId,
                        Year = year,
                        CompanyID = companyId
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
            }

            return model;
        }
        // --- Timeline ---

        /// <summary>
        /// Retrieves all timeline records for a specific staff member.
        /// </summary>
        /// <param name="staffId">Staff ID.</param>
        /// <returns>List of timeline records.</returns>
        public List<HRStaffTimelineViewModel> GetStaffTimeline(int staffId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<HRStaffTimelineViewModel>(
                    "sp_HR_Timeline_GetByStaff",
                    new { StaffID = staffId },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<HRStaffTimelineViewModel>();
            }
        }

        /// <summary>
        /// Retrieves a timeline record by its ID.
        /// </summary>
        /// <param name="id">Timeline ID.</param>
        /// <returns>Timeline details if found; otherwise null.</returns>
        public HRStaffTimelineViewModel? GetTimelineByID(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<HRStaffTimelineViewModel>(
                    "sp_HR_Timeline_GetByID",
                    new { TimelineID = id },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates or updates a staff timeline record.
        /// </summary>
        /// <param name="req">Timeline information.</param>
        /// <param name="companyId">Company ID.</param>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Success status and message.</returns>
        public SpStaffTimeline UpsertTimeline(HRStaffTimelineUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
               
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var param = new DynamicParameters();
                param.Add("@TimelineID", req.TimelineID);
                param.Add("@StaffID", req.StaffID);
                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@TimelineTitle", req.TimelineTitle);
                param.Add("@TimelineDate", req.TimelineDate);
                param.Add("@TimelineDescription", req.TimelineDescription);
                param.Add("@TimelineAttachDoc", req.AttachDocBase64);
                param.Add("@TimelineAttahDocName", req.AttachDocName);
                param.Add("@TimelineAttachDocType", req.AttachDocType);
                param.Add("@TimelineVisible", req.TimelineVisible);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpStaffTimeline>(
                    "sp_HR_Timeline_Upsert",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result;

            }
            catch (Exception ex)
            {
                var model = new SpStaffTimeline();
                model.Result = 0;
                model.Message = ex.Message;
                model.TimelineID = 0;
                return model;
            }
        }

        /// <summary>
        /// Deletes a timeline record.
        /// </summary>
        /// <param name="id">Timeline ID.</param>
        /// <param name="userId">User ID performing the delete operation.</param>
        /// <returns>Success status and message.</returns>
        public (bool Success, string Message) DeleteTimeline(int id, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_HR_Timeline_Delete",
                    new
                    {
                        TimelineID = id,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the timeline document by timeline ID.
        /// </summary>
        /// <param name="id">Timeline ID.</param>
        /// <returns>Document bytes, file name, and content type.</returns>
        public (byte[] Bytes, string FileName, string ContentType) GetTimelineDocument(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_HR_Timeline_GetByID",
                    new { TimelineID = id },
                    commandType: CommandType.StoredProcedure);

                if (result == null || result.TimelineAttachDoc == null)
                    return (null!, null!, null!);

                return (
                    (byte[])result.TimelineAttachDoc,
                    result.TimelineAttachDocName ?? "Document.pdf",
                    result.TimelineAttachDocType ?? "application/pdf"
                );
            }
            catch
            {
                return (null!, null!, null!);
            }
        }
        private static HRStaffTimelineViewModel MapTimeline(DataRow r) => new()
        {
            TimelineID = Convert.ToInt32(r["TimelineID"]),
            StaffID = Convert.ToInt32(r["StaffID"]),
            TimelineTitle = r["TimelineTitle"].ToString()!,
            TimelineDate = Convert.ToDateTime(r["TimelineDate"]),
            TimelineDescription = r["TimelineDescription"]?.ToString(),
            TimelineAttachDocName = r["TimelineAttachDocName"]?.ToString(),
            TimelineVisible = Convert.ToBoolean(r["TimelineVisible"]),
            CreatedOn = Convert.ToDateTime(r["CreatedOn"])
        };

        /// <summary>
        /// Activates or deactivates a staff member.
        /// </summary>
        /// <param name="staffId">Staff ID.</param>
        /// <param name="isActive">Status flag.</param>
        /// <param name="userId">User ID.</param>
        /// <param name="statusDate">Status effective date.</param>
        /// <returns>Operation result.</returns>
        public (bool Success, string Message) ToggleStaffStatus(int staffId, bool isActive, int userId, DateTime? statusDate)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_HR_Staff_ToggleStatus",
                    new
                    {
                        StaffID = staffId,
                        IsActive = isActive,
                        UserID = userId,
                        StatusDate = statusDate
                    },
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
        /// Retrieves leave application details by ID.
        /// </summary>
        /// <param name="id">Apply leave ID.</param>
        /// <returns>Leave application details.</returns>
        public HRApplyLeaveViewModel? GetApplyLeaveByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<HRApplyLeaveViewModel>(
                "sp_HR_ApplyLeave_GetByID",
                new { ApplyLeaveID = id },
                commandType: CommandType.StoredProcedure);
        }
        /// <summary>
        /// Creates or updates an employee leave application.
        /// Handles attachment conversion from Base64 and saves leave details.
        /// </summary>
        /// <param name="req">Leave application request.</param>
        /// <param name="companyId">Company ID.</param>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="userId">Current User ID.</param>
        /// <returns>Success status and message.</returns>
        public (bool Success, string Message) UpsertApplyLeave(HRApplyLeaveUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                if (companyId <= 0 || sessionId <= 0)
                {
                    var staff = GetStaffByID(req.StaffID);

                    if (staff != null)
                    {
                        if (companyId <= 0)
                            companyId = staff.CompanyID;

                        if (sessionId <= 0)
                            sessionId = staff.SessionID;
                    }
                }

                byte[]? attachmentBytes = null;

                if (!string.IsNullOrEmpty(req.AttachmentBase64))
                {
                    var base64Data = req.AttachmentBase64.Contains(",")
                        ? req.AttachmentBase64.Split(',')[1]
                        : req.AttachmentBase64;

                    attachmentBytes = Convert.FromBase64String(base64Data);
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_HR_ApplyLeave_Upsert",
                    new
                    {
                        req.ApplyLeaveID,
                        CompanyID = companyId,
                        SessionID = sessionId,
                        req.StaffID,
                        req.LeaveTypeID,
                        req.FromDate,
                        req.ToDate,
                        req.Reason,
                        Status = req.Status ?? "Pending",
                        req.Note,
                        AttachmentDoc = attachmentBytes,
                        req.AttachmentDocType,
                        req.AttachmentDocName,
                        UserID = userId,
                        IPAddress = (string?)null
                    },
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
        /// Retrieves attachment document for a leave application.
        /// </summary>
        /// <param name="id">Apply Leave ID.</param>
        /// <returns>File bytes, file name and content type.</returns>
        public (byte[] Bytes, string FileName, string ContentType)
            GetApplyLeaveDocument(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_HR_ApplyLeave_GetByID",
                    new
                    {
                        ApplyLeaveID = id
                    },
                    commandType: CommandType.StoredProcedure);

                if (result == null || result.AttachmentFile == null)
                    return (null!, null!, null!);

                return (
                    (byte[])result.AttachmentFile,
                    Convert.ToString(result.AttachmentDocName) ?? "Attachment.pdf",
                    Convert.ToString(result.AttachmentDocType) ?? "application/pdf"
                );
            }
            catch
            {
                return (null!, null!, null!);
            }
        }

        /// <summary>
        /// Deletes a leave application.
        /// </summary>
        /// <param name="id">Apply Leave ID.</param>
        /// <param name="userId">Current User ID.</param>
        /// <returns>Success status and message.</returns>
        public (bool Success, string Message) DeleteApplyLeave(List<int> id,  int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                string applyLeaveID = string.Join(",", id);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_HR_ApplyLeave_Delete",
                    new
                    {
                        ApplyLeaveIDs = applyLeaveID,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return ((int)result.Result == 1,
                        (string)result.Message);

                
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Updates the approval status of a leave application.
        /// </summary>
        /// <param name="req">Status update request.</param>
        /// <param name="userId">Current User ID.</param>
        /// <returns>Success status and message.</returns>
        public (bool Success, string Message) UpdateApplyLeaveStatus(HRApplyLeaveStatusUpdateRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_HR_ApplyLeave_StatusUpdate",
                    new
                    {
                        req.ApplyLeaveID,
                        req.Status,
                        req.Note,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        private static HRApplyLeaveViewModel MapApplyLeave(DataRow r) => new()
        {
            ApplyLeaveID = Convert.ToInt32(r["ApplyLeaveID"]),
            CompanyID = Convert.ToInt32(r["CompanyID"]),
            SessionID = Convert.ToInt32(r["SessionID"]),
            StaffID = Convert.ToInt32(r["StaffID"]),
            StaffName = r.Table.Columns.Contains("StaffName") ? r["StaffName"].ToString()! : "Unknown",
            StaffCode = r.Table.Columns.Contains("StaffCode") ? r["StaffCode"].ToString()! : "-",
            LeaveTypeID = Convert.ToInt32(r["LeaveTypeID"]),
            LeaveTypeName = r.Table.Columns.Contains("LeaveTypeName") ? r["LeaveTypeName"].ToString()! : "Unknown",
            ApplyDate = Convert.ToDateTime(r["ApplyDate"]),
            FromDate = Convert.ToDateTime(r["FromDate"]),
            ToDate = Convert.ToDateTime(r["ToDate"]),
            Reason = r.Table.Columns.Contains("Reason") && r["Reason"] != DBNull.Value ? r["Reason"].ToString() : null,
            Status = r["Status"].ToString()!,
            ApprovedBy = r.Table.Columns.Contains("ApprovedBy") && r["ApprovedBy"] != DBNull.Value ? Convert.ToInt32(r["ApprovedBy"]) : (int?)null,
            ApprovedByName = r.Table.Columns.Contains("ApprovedByName") ? (r["ApprovedByName"]?.ToString() ?? "-") : "-",
            AttachmentDocType = r.Table.Columns.Contains("AttachmentDocType") && r["AttachmentDocType"] != DBNull.Value ? r["AttachmentDocType"].ToString() : null,
            AttachmentDocName = r.Table.Columns.Contains("AttachmentDocName") && r["AttachmentDocName"] != DBNull.Value ? r["AttachmentDocName"].ToString() : null,
            Note = r.Table.Columns.Contains("Note") && r["Note"] != DBNull.Value ? r["Note"].ToString() : null
        };



        /// <summary>
        /// Retrieves payroll summary and payroll detail records for a specific payroll.
        /// </summary>
        /// <param name="payrollId">Payroll ID.</param>
        /// <returns>Payroll summary and detail information.</returns>
        public HRPayrollDetailsViewModel GetPayrollDetails(int payrollId)
        {
            var model = new HRPayrollDetailsViewModel();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                using var multi = conn.QueryMultiple(
                    "sp_HR_Payroll_GetByID",
                    new { PayrollID = payrollId },
                    commandType: CommandType.StoredProcedure);

                model.Summary = multi.ReadFirstOrDefault<HRPayrollViewModel>();

                model.Details = multi.Read<HRPayrollDetailViewModel>().ToList();
            }
            catch (Exception ex)
            {
                model.Summary ??= new HRPayrollViewModel();
                model.Summary.Note = "Error fetching details: " + ex.Message;
            }

            return model;
        }

        /// <summary>
        /// Retrieves payroll records for a specific staff member.
        /// </summary>
        /// <param name="staffId">Staff ID.</param>
        /// <returns>List of payroll records.</returns>
        public List<HRPayrollViewModel> GetStaffPayroll(int staffId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<HRPayrollViewModel>(
                "sp_HR_Payroll_GetByStaff",
                new { StaffID = staffId },
                commandType: CommandType.StoredProcedure)
                .ToList();
        }


        public (bool Success, string Message) UpdateProfile(int staffId, string PhotoDoc, int userId)
        {
            try
            {

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@STAFFID", staffId);
                parameters.Add("@PHOTODOC", PhotoDoc);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_HR_Staff_UpdateProfile",
                   parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<LeaveQuotaModel> GetLeaveBalance(int leaveTypeId, int staffId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@LeaveTypeID", leaveTypeId);
                parameters.Add("@StaffID", staffId);

                return await conn.QueryFirstOrDefaultAsync<LeaveQuotaModel>(
                    "usp_GetLeaveBalance",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        public async Task<PagedResult<HRApplyLeaveViewModel>> GetAllApplyLeaveWithPage(SearchRequest req)
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
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@StaffID", req.StaffId);
                param.Add("@SearchText", req.SearchKeyword);
                param.Add("@PageIndex", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<HRApplyLeaveViewModel>(
                "sp_HR_ApplyLeave_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageNumber ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<HRApplyLeaveViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<HRApplyLeaveViewModel>
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

        public async Task<StaffAttendanceSummaryModel?> GetStaffAttendanceSummary(
           int companyId, int sessionId, int staffId, int? year = null, int? status = null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@STAFFID", staffId);
                parameters.Add("@YEAR", year);
                parameters.Add("@STATUS", status);

                using var multi = await conn.QueryMultipleAsync(
                    "sp_HR_StaffAttendance_GetYearlyProfile",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // ── Result Set 1: Totals ──
                var totals = await multi.ReadFirstOrDefaultAsync<AttendanceSummaryTotalsModel>()
                             ?? new AttendanceSummaryTotalsModel();

                // ── Result Set 2: Grid rows ──
                var rows = (await multi.ReadAsync<AttendanceStaffGridRowModel>()).ToList();

                return new StaffAttendanceSummaryModel
                {
                    Totals = totals,
                    AttendanceRows = rows
                };
            }
            catch (Exception ex)
            {
                // Log the actual exception — swallowing it silently makes debugging hard later
                // _logger?.LogError(ex, "Failed to fetch attendance summary for StudentID={StudentId}", studentId);
                return new StaffAttendanceSummaryModel();
            }
        }

        public (bool success, string message) UpsertStaffTimelineAttachmentFile(
 StudentTimelineAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@TIMELINEID", req.TimelineID);
                parameters.Add("@TIMELINEATTACHDOC", req.Attachment);
                parameters.Add("@TIMELINEATTACHDOCNAME", req.FileName);
                parameters.Add("@TIMELINEATTACHDOCTYPE", req.FileType);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_HR_TIMELINE_UPSERT_ATTACHMENT",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
