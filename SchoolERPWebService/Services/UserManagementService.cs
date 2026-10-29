using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// Service responsible for retrieving user permission information
    /// from the database using Dapper and stored procedures.
    /// </summary>
    public class UserManagementService: IUserManagementService
    {
        private readonly SqlHelper _sqlHelper;
        private readonly IConfiguration _configuration;
        public UserManagementService(SqlHelper sqlHelper, IConfiguration configuration)
        {
            _sqlHelper = sqlHelper;
            _configuration = configuration;
        }


        /// <summary>
        /// Retrieves all permissions assigned to the specified user.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user.
        /// </param>
        /// <returns>
        /// A list of user permissions associated with the specified user.
        /// </returns>
        public async Task<List<UserPermissionViewModel>> GetUserPermissionsAsync(int userId, int roleId)
        {
            try
            {
                using var conn = new SqlConnection(
               _configuration.GetConnectionString("DefaultConnection"));

                var list = await conn.QueryAsync<UserPermissionViewModel>(
                    "sp_User_GetPermissions",
                    new { UserID = userId , RoleID = roleId},
                    commandType: CommandType.StoredProcedure);
                return list.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        /// <summary>
        /// Retrieves user permissions with optional filtering by
        /// menu URL prefix and permission name.
        /// </summary>
        /// <returns>
        /// A filtered list of user permissions.
        /// </returns>
        public async Task<List<UserPermissionViewModel>> GetUserPermissions(int userId, string? menuUrlPrefix=null, string? permissionName = null,int? roleID=null)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var param = new DynamicParameters();
            if(roleID ==0) 
            {
                roleID = null;
            }
            param.Add("@USERID", userId);
            param.Add("@PermissionName", permissionName);
            param.Add("@MenuKey", menuUrlPrefix);
            param.Add("@RoleID", roleID);

            var result = await conn.QueryAsync<UserPermissionViewModel>(
            "sp_User_GetPermissions",
            param,
            commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        /// <summary>
        /// Retrieves a list of all user types from the database.
        /// </summary>
        /// <returns>A list of <see cref="MstUserTypeViewModel"/> containing user type details.</returns>
        public List<MstUserTypeViewModel> GetAllUserTypes()
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<MstUserTypeViewModel>(
                "sp_UserTypes_GetAll",
                commandType: CommandType.StoredProcedure
            ).ToList();
        }




        public async Task<PagedResult<MstUserTypeViewModel>> GetAllUserTypesWithPage(SearchRequest req)
        {
            try
            {
                // Sanitise pagination — same guard as the SP
                if (req.PageNumber <= 0) req.PageNumber = 1;
                if (req.PageSize <= 0) req.PageSize = 10;

                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var param = new DynamicParameters();
                param.Add("@UserId", req.UserId);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                using var multi = await conn.QueryMultipleAsync(
                    "sp_UserTypes_GetAllPAGEINDEX", param,
                    commandType: CommandType.StoredProcedure);

                // Result set 1 — pagination meta from SP
                var meta = await multi.ReadFirstOrDefaultAsync<PaginationMeta>();

                // Result set 2 — actual section rows
                var sections = (await multi.ReadAsync<MstUserTypeViewModel>()).ToList();

                return new PagedResult<MstUserTypeViewModel>
                {
                    Data = sections,
                    TotalRecords = meta?.TotalRecords ?? 0,
                    PageNumber = meta?.PageNumber ?? req.PageNumber.Value,
                    PageSize = meta?.PageSize ?? req.PageSize,
                    //TotalPages = meta?.TotalPages ?? 0         
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Looks up a specific user category's details from the database.
        /// </summary>
        /// <param name="userTypeID">The unique identifier of the user type.</param>
        /// <returns>
        /// A <see cref="MstUserTypeViewModel"/> containing the user type details if found; otherwise, <c>null</c>.
        /// </returns>
        public MstUserTypeViewModel? GetUserTypeByID(int userTypeID)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstUserTypeViewModel>(
                "sp_UserTypes_GetByID",
                new { UserTypeID = userTypeID },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Saves a new user type or updates an existing user type in the database.
        /// Executes the <c>sp_UserTypes_Upsert</c> stored procedure and returns the operation result.
        /// </summary>
        /// <param name="request">The user type details to insert or update.</param>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <param name="ipAddress">The IP address of the user performing the operation.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>success</c> - Indicates whether the operation was successful.</description></item>
        /// <item><description><c>message</c> - The result message returned by the stored procedure.</description></item>
        /// </list>
        /// </returns>
        public (bool success, string message) UpsertUserType(MstUserTypeUpsertRequest request, int userId, string ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_UserTypes_Upsert",
                    new
                    {
                        request.UserTypeID,
                        request.TypeCode,
                        request.TypeName,
                        request.IsActive,
                        UserID = userId,
                        IPAddress = ipAddress
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message ?? "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Updates whether a user category is active or inactive.
        /// </summary>
        /// <param name="userTypeID">The unique identifier of the user type.</param>
        /// <param name="isActive">The status to set (true = active, false = inactive).</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        /// <param name="ipAddress">The IP address from which the request originated.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>success</c> - Indicates whether the operation was successful.</description></item>
        /// <item><description><c>message</c> - The result message returned by the database.</description></item>
        /// </list>
        /// </returns>
        public (bool success, string message) ToggleUserTypeStatus(int userTypeID, bool isActive, int userId, string ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault(
                    "sp_UserTypes_ToggleStatus",
                    new
                    {
                        UserTypeID = userTypeID,
                        IsActive = isActive,
                        UserID = userId,
                        IPAddress = ipAddress
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result?.Result ?? 0) == 1,
                    result?.Message?.ToString() ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool success, string message) ToggleUserTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@USERTYPEID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPADDRESS", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_UserTypes_ToggleStatus",
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
        /// Deletes a section record by its unique ID.
        /// </summary>
        /// <param name="sectionId">Section ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) DeleteUserType(
            List<int> userTypeID,
            int userId)
        {
            try
            {
                if (userTypeID == null || !userTypeID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", userTypeID);

                var parameters = new DynamicParameters();
                parameters.Add("@UserTypeID", id);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_UserTypes_Delete",
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
        /// Retrieves a list of all roles from the database.
        /// </summary>
        /// <returns>A collection of role details.</returns>
        public List<MstRoleViewModel> GetAllRoles()
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<MstRoleViewModel>(
                    "sp_Roles_GetAll",
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Looks up a specific role's details from the database.
        /// </summary>
        /// <param name="roleID">The unique identifier of the role.</param>
        /// <returns>
        /// A <see cref="MstRoleViewModel"/> containing the role details if found; otherwise, <c>null</c>.
        /// </returns>
        public MstRoleViewModel? GetRoleByID(int roleID)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<MstRoleViewModel>(
                    "sp_Roles_GetByID",
                    new
                    {
                        RoleID = roleID
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Saves or updates a role in the database.
        /// </summary>
        public async Task<(bool success, string message, int roleId)> UpsertRole(MstRoleUpsertRequest request, int userId, string ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var result = await conn.QueryFirstOrDefaultAsync<RoleUpsertResult>(
                    "sp_Roles_Upsert",
                    new
                    {
                        request.RoleID,
                        request.RoleName,
                        request.RoleDesc,
                        request.IsActive,
                        UserID = userId,
                        IPAddress = ipAddress
                    },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No responsive mapping detected from database trace", 0);

                bool ok = Convert.ToInt32(result.Result) == 1;
                string msg = Convert.ToString(result.Message) ?? string.Empty;

                int savedRoleId = 0;
                try
                {
                    savedRoleId = result.RoleID != null
                        ? Convert.ToInt32(result.RoleID)
                        : (request.RoleID > 0 ? request.RoleID : 0);
                }
                catch
                {
                    savedRoleId = request.RoleID > 0 ? request.RoleID : 0;
                }

                return (ok, msg, savedRoleId);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0);
            }
        }

        /// <summary>
        /// Updates the active/inactive status of a role.
        /// </summary>
        /// <param name="roleID">The unique identifier of the role.</param>
        /// <param name="isActive">The status to set for the role.</param>
        /// <param name="userId">The identifier of the user performing the action.</param>
        /// <param name="ipAddress">The IP address of the user performing the action.</param>
        /// <returns>
        /// A tuple containing:
        /// - success: Indicates whether the operation was successful.
        /// - message: The response message returned by the stored procedure.
        /// </returns>
        public (bool success, string message) ToggleRoleStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Roles_ToggleStatus",
                    new
                    {
                        ROLEIDS = request.Ids,
                        IsActive = request.IsActive,
                        UserID = request.DoneBy,
                        IPAddress = request.IpAddress
                    },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No responsive mapping detected from database trace");

                return (result.Result == 1, result.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Fetches the full list of allowed actions for a role from the database.
        /// </summary>
        public List<RoleMenuPermissionViewModel> GetPermissionsMatrix(int roleId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var list = conn.Query<RoleMenuPermissionViewModel>(
                    "sp_Roles_GetPermissionsMatrix",
                    new { RoleID = roleId },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                return list ?? new List<RoleMenuPermissionViewModel>();
            }
            catch
            {
                return new List<RoleMenuPermissionViewModel>();
            }
        }

        /// <summary>
        /// Saves the mapping of actions to roles in the database.
        /// </summary>
        public (bool success, string message) SaveRolePermissions(MstRolePermissionSaveRequest request, int adminId, string ipAddress)
        {
            try
            {
                string pairs = string.Join(",",
                    request.SelectedPermissions.Select(p => $"{p.MenuID}:{p.PermissionID}"));

                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Roles_SavePermissions",
                    new
                    {
                        request.RoleID,
                        MenuPermissionPairs = pairs,
                        UserId = adminId,
                        IPAddress = ipAddress
                    },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response from database");

                return (result.Result == 1, result.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a role from the database.
        /// Executes the sp_Roles_Delete stored procedure and returns the operation result.
        /// </summary>
        /// <param name="roleID">The unique identifier of the role to delete.</param>
        /// <param name="userId">The ID of the user performing the delete operation.</param>
        /// <param name="ipAddress">The IP address of the user performing the operation.</param>
        /// <returns>
        /// A tuple containing:
        /// - success: Indicates whether the delete operation was successful.
        /// - message: The result message returned by the stored procedure.
        /// </returns>
        public (bool success, string message) DeleteRole(List<int> roleID, int userId, string ipAddress)
        {
            try
            {
                if (roleID == null || !roleID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string roleIDs = string.Join(",", roleID);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Roles_Delete",
                    new
                    {
                        ROLEID = roleIDs,
                        UserID = userId,
                        IPAddress = ipAddress
                    },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No responsive mapping detected from database trace");

                return (result.Result == 1, result.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }



        /// <summary>
        /// Retrieves a list of all possible actions/permissions from the database.
        /// </summary>
        public List<MstPermissionViewModel> GetAllPermissions()
        {
            var list = new List<MstPermissionViewModel>();
            var dt = _sqlHelper.ExecuteQuery("sp_Permissions_GetAll", null!);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MstPermissionViewModel
                {
                    PermissionID = Convert.ToInt32(row["PermissionID"]),
                    PermissionName = row["PermissionName"].ToString() ?? "",
                    DisplayLabel = row["DisplayLabel"]?.ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedOn = Convert.ToDateTime(row["CreatedOn"])
                });
            }
            return list;
        }

        /// <summary>
        /// Looks up a specific permission's details from the database.
        /// </summary>
        public MstPermissionViewModel? GetPermissionByID(int permissionID)
        {
            var dt = _sqlHelper.ExecuteQuery("sp_Permissions_GetByID", new[] { new SqlParameter("@PermissionID", permissionID) });
            if (dt.Rows.Count == 0) return null;
            var row = dt.Rows[0];
            return new MstPermissionViewModel
            {
                PermissionID = Convert.ToInt32(row["PermissionID"]),
                PermissionName = row["PermissionName"].ToString() ?? "",
                DisplayLabel = row["DisplayLabel"]?.ToString(),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedOn = Convert.ToDateTime(row["CreatedOn"])
            };
        }

        /// <summary>
        /// Saves or updates a permission in the database.
        /// </summary>
        public (bool success, string message) UpsertPermission(MstPermissionUpsertRequest request, int userId, string ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Permissions_Upsert",
                    new
                    {
                        PermissionID = request.PermissionID,
                        PermissionName = request.PermissionName,
                        DisplayLabel = (object?)request.DisplayLabel ?? DBNull.Value,
                        IsActive = request.IsActive,
                        UserID = userId,
                        IPAddress = ipAddress
                    },
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Updates whether a permission is currently usable.
        /// </summary>
        public (bool success, string message) TogglePermissionStatus(StatusUpdateRequest request)
        {
            try
            {
                

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@PermissionID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserID", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Permissions_ToggleStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );

            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes a permission from the database.
        /// </summary>
        public (bool success, string message) DeletePermission(List<int> permissionID, int userId, string ipAddress)
        {
            try
            {
                if (permissionID == null || !permissionID.Any())
                {
                    return (false, "No students selected for deletion.");
                }
                string permissionIDs = string.Join(",", permissionID);
                var parameters = new[]
                {
                    new SqlParameter("@PermissionID", permissionIDs),
                    new SqlParameter("@UserID", userId),
                    new SqlParameter("@IPAddress", ipAddress)
                };
                var dt = _sqlHelper.ExecuteQuery("sp_Permissions_Delete", parameters);
                return (Convert.ToInt32(dt.Rows[0]["Result"]) == 1, dt.Rows[0]["Message"].ToString() ?? "");
            }
            catch (Exception ex) { return (false, ex.Message); }
        }


        public List<UserRoleViewModel> GetAllRolesByUser(int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@USERID", userId);
                return conn.Query<UserRoleViewModel>(
                    "SP_USER_ROLES_GETBYUSER",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task<PagedResult<MstPermissionViewModel>> GetAllPermissionWithPage(SearchRequest req)
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
                param.Add("@ONLYACTIVE", 0);
                param.Add("@INCLUDEDELETED", 0);

                var result = (await conn.QueryAsync<MstPermissionViewModel>(
                "sp_Permissions_GetAllWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstPermissionViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstPermissionViewModel>
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

        public async Task<PagedResult<MstRoleViewModel>> GetAllRoleWithPage(SearchRequest req)
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


                param.Add("@ONLYACTIVE", 0);
                param.Add("@INCLUDEDELETED",0);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<MstRoleViewModel>(
                "sp_Roles_GetAllWithPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstRoleViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstRoleViewModel>
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

        public List<SearchByRoleModel> GetAllRolesBySearch(int roleID,string searchTerm,int companyID,int sessionID)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@RoleID", roleID);
                parameters.Add("@SearchTerm", searchTerm);
                parameters.Add("@CompanyID", companyID);
                parameters.Add("@SessionID", sessionID);
                return conn.Query<SearchByRoleModel>(
                    "SP_DOWNLOAD_SEARCHUSERSBYROLE",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch
            {
                throw;
            }
        }

    }
}
