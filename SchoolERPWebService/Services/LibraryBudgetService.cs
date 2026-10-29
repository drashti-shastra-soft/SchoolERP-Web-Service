using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class LibraryBudgetService:ILibraryBudgetService
    {
        private readonly IConfiguration _configuration;
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryBudgetService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration instance.</param>
        public LibraryBudgetService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Creates or updates a library budget in the database.
        /// </summary>
        /// <param name="model">The library budget request details.</param>
        /// <returns>An API response indicating operation outcome.</returns>
        public async Task<ApiResponse> UpsertLibraryBudgetAsync(LibraryBudgetRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@BudgetId", model.BudgetId);
                parameters.Add("@COMPANYID", model.CompanyID);
                parameters.Add("@BudgetName", model.BudgetName);
                parameters.Add("@DisplayLabel", model.DisplayLabel);
                parameters.Add("@Acronym", model.Acronym);
                parameters.Add("@ISACTIVE", model.IsActive);
                parameters.Add("@USERID", model.UserID);
                parameters.Add("@IPADDRESS", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_TBL_MST_LibraryBudget_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure);

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
                response.Message = "Unable to save hostel type. Please try again.";
                response.TechnicalMessage = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Retrieves a library budget record by its identifier.
        /// </summary>
        /// <param name="budgetId">The library budget ID.</param>
        /// <returns>The library budget model details if found; otherwise null.</returns>
        public async Task<LibraryBudgetModel?> GetLibraryBudgetByIdAsync(int budgetId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@BudgetId", budgetId);

                return await conn.QueryFirstOrDefaultAsync<LibraryBudgetModel>(
                    "SP_TBL_MST_LibraryBudget_GETBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// Retrieves a paginated list of library budgets based on the search criteria.
        /// </summary>
        /// <param name="req">The search request criteria.</param>
        /// <returns>A paginated list of library budgets.</returns>
        public async Task<PagedResult<LibraryBudgetModel>> GetAllLibraryBudgetWithPage(SearchRequest req)
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
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);

                var result = (await conn.QueryAsync<LibraryBudgetModel>(
                "SP_TBL_MST_LibraryBudget_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<LibraryBudgetModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<LibraryBudgetModel>
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
        /// Deletes one or more library budgets by ID.
        /// </summary>
        /// <param name="ids">List of library budget identifiers to delete.</param>
        /// <param name="userId">The identifier of the user performing the deletion.</param>
        /// <returns>A tuple indicating success status and status message.</returns>
        public (bool success, string message) DeleteLibraryBudgety(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No library budget selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string budgetId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@BUDGETID", budgetId);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_LibraryBudget_DELETE",
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
        /// Toggles the active/inactive status of a library budget.
        /// </summary>
        /// <param name="request">Status update request details.</param>
        /// <returns>A tuple indicating success status and status message.</returns>
        public (bool success, string message) ToggleLibraryBudgetyStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@BUDGETID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_LibraryBudget_TOGGLESTATUS",
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
        /// Retrieves all budget for the company with optional includdeleted filtering.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="includdeleted">Optional includdeleted to filter budget.</param>
        /// <returns>A list of budget view models matching the criteria.</returns>
        public List<LibraryBudgetModel> GetAll(int companyId, bool? includdeleted)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<LibraryBudgetModel>(
                    "SP_TBL_MST_LibraryBudget_GETALL",
                    new
                    {
                        CompanyID = companyId,
                        INCLUDEDELETED = includdeleted
                    },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<(bool Success, string Message)> CopyLibraryBudget(CopyForCompanyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROMCOMPANYID", req.FromCompanyId);
                parameters.Add("@TOCOMPANYID", req.ToCompanyId);
                parameters.Add("@USERID", req.UserID);
                parameters.Add("@IPADDRESS", req.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_MST_LibraryBudget_COPYCOMPANY",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception)
            {

                throw;
            }


        }
    }
}
