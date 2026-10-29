using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class MenuService:IMenuService
    {
        private readonly SqlHelper _sqlHelper;
        private readonly IConfiguration _configuration;
        public MenuService(SqlHelper sqlHelper, IConfiguration configuration)
        {
            _sqlHelper = sqlHelper;
            _configuration = configuration;
        }
        /// <summary>
        /// Retrieves a list of all navigation menus from the database.
        /// </summary>
        public async Task<List<MenuViewModel>> GetAllMenus()
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = await conn.QueryAsync<MenuViewModel>(
                    "sp_Menus_GetAll",
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error in GetAllMenusAsync : {ex.Message}");
            }
        }


        public async Task<PagedResult<MenuViewModel>> GetAllMenusWithPage(SearchRequest req)
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
                param.Add("@USERID", req.UserId);

                var result = (await conn.QueryAsync<MenuViewModel>(
                "sp_Menus_GETALLPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MenuViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MenuViewModel>
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
        /// Looks up the details of a specific menu item from the database.
        /// </summary>
        public async Task<MenuViewModel?> GetMenuById(int menuId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                param.Add("@MenuID", menuId);

                return await conn.QueryFirstOrDefaultAsync<MenuViewModel>(
                    "sp_Menus_GetByID",
                    param,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMenuByIdAsync: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Saves or updates a menu item's information, including its name, icon, and link.
        /// </summary>
        public async Task<(int Result, string Message)> UpsertMenu(
        MenuUpsertRequest request,
        int userId,
        int mainAccountId,
        int sessionId,
        string ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@MenuID", request.MenuID);
                param.Add("@MenuName", request.MenuName);
                param.Add("@DisplayLabel", request.DisplayLabel);
                param.Add("@MenuURL", request.MenuURL);
                param.Add("@ParentID", request.ParentID);
                param.Add("@DisplayOrder", request.DisplayOrder);
                param.Add("@MenuIcon", request.MenuIcon);
                param.Add("@MenuKey", request.MenuKey);
                param.Add("@IsActive", request.IsActive);
                param.Add("@UserID", userId);
                param.Add("@IPAddress", ipAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "sp_Menus_Upsert",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    return (result.Result, result.Message);
                }
            }
            catch (Exception ex)
            {
                return (-1, "Database Error: " + ex.Message);
            }

            return (-1, "No response from database");
        }

        /// <summary>
        /// Updates whether a menu item should be shown or hidden.
        /// </summary>
        public async Task<(int Result, string Message)> ToggleMenuStatus(
        StatusUpdateRequest request,
        int mainAccountId,
        int sessionId,
        string ipAddress)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@MenuID", request.Ids);
                param.Add("@IsActive", request.IsActive);
                param.Add("@UserID", request.DoneBy);
                param.Add("@MainAccountId", mainAccountId);
                param.Add("@SessionId", sessionId);
                param.Add("@IPAddress", ipAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "sp_Menus_ToggleStatus",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    return (result.Result, result.Message);
                }
            }
            catch (Exception ex)
            {
                return (-1, "Database Error: " + ex.Message);
            }

            return (-1, "No response from database");
        }

        /// <summary>
        /// Deletes a menu item from the database.
        /// </summary>
        public (int Result, string Message) DeleteMenu(List<int> menuId, int userId, string ipAddress)
        {
            try
            {
                if (menuId == null || !menuId.Any())
                {
                    return (0, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string menuIds = string.Join(",", menuId);


                var parameters = new DynamicParameters();
                parameters.Add("@MenuID", menuIds);
                parameters.Add("@UserID", userId);
                parameters.Add("@IPAddress", ipAddress);

                var response = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Menus_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (response != null)
                {
                    return (
                        Convert.ToInt32(response.Result),
                        Convert.ToString(response.Message) ?? "Unknown error"
                    );
                }
            }
            catch (Exception ex)
            {
                return (-1, "Database Error: " + ex.Message);
            }

            return (-1, "No response from database");
        }

        /// <summary>
        /// Updates the sequence in which menu items are displayed on the screen.
        /// </summary>
        public (int Result, string Message) UpdateMenuOrder(
        List<MenuOrderRequest> orders,
        int userId,
        string ipAddress)
        {
            int successCount = 0;

            try
            {
                using var connection = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                connection.Open();

                // Step 1: Go through each menu item in the list one by one.
                foreach (var order in orders)
                {
                    // Step 2: Prepare the specific ID and its new position (Order Number).
                    var parameters = new DynamicParameters();
                    parameters.Add("@MenuID", order.MenuID);
                    parameters.Add("@DisplayOrder", order.DisplayOrder);
                    parameters.Add("@UserID", userId);
                    parameters.Add("@IPAddress", ipAddress);

                    // Step 3: Tell the database to update the order for this specific item.
                    connection.Execute(
                        "sp_Menus_UpdateOrder",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    successCount++;
                }

                // Step 4: If every item was updated successfully, report success.
                if (successCount == orders.Count)
                {
                    return (1, "Menu order updated successfully");
                }
                else
                {
                    return (0, $"Updated {successCount} of {orders.Count} items");
                }
            }
            catch (Exception ex)
            {
                return (-1, $"Error updating menu order: {ex.Message}");
            }
        }
    }
}
