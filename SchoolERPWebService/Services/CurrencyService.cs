using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using System.Data;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing currency information, like saving exchange rates and updating currency details in the database.
    /// </summary>
    public class CurrencyService: ICurrencyService
    {
        private readonly IConfiguration _configuration;
        public CurrencyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Retrieves all currencies, optionally including soft-deleted records.
        /// </summary>
        /// <param name="includeDeleted">If true, includes deleted currencies; otherwise only active ones.</param>
        /// <returns>List of <see cref="MstCurrencyViewModel"/> representing all matching currencies.</returns>
        public List<MstCurrencyViewModel> GetAllCurrencies(bool includeDeleted = false)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var result = conn.Query<MstCurrencyViewModel>(
                "sp_Currencies_GetAll",
                new { IncludeDeleted = includeDeleted },
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }

        /// <summary>
        /// Retrieves a currency record by its unique identifier.
        /// </summary>
        /// <param name="currencyId">The unique identifier of the currency.</param>
        /// <returns>A <see cref="MstCurrencyViewModel"/> if found; otherwise, <c>null</c>.</returns>
        public MstCurrencyViewModel? GetCurrencyByID(int currencyId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstCurrencyViewModel>(
                "sp_Currencies_GetByID",
                new { CurrencyId = currencyId },
                commandType: CommandType.StoredProcedure
            );
        }
        /// <summary>
        /// Inserts or updates a currency record in the database.
        /// </summary>
        /// <param name="request">Currency details (Id, Title, Code, Symbol, ConvRate, Base, IsActive).</param>
        /// <param name="userId">ID of the user performing the operation.</param>
        /// <returns>A tuple indicating success status and a result message.</returns>
        public (bool success, string message) UpsertCurrency(MstCurrencyUpsertRequest request, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "sp_Currencies_Upsert",
                    new
                    {
                        request.CurrencyId,
                        request.CurrencyTitle,
                        request.CurrencyCode,
                        request.CurrencySymbol,
                        request.CurrencyConvRate,
                        request.CurrencyBase,
                        request.IsActive,
                        UserId = userId
                    },
                    commandType: CommandType.StoredProcedure
                );

                return (
                    result?.Result == 1,
                    result?.Message ?? "Operation completed."
                );
            }
            catch (Exception ex) { return (false, ex.Message); }
        }
        /// <summary>
        /// Deletes a currency record by its ID using the specified stored procedure.
        /// </summary>
        /// <param name="currencyId">The unique identifier of the currency to delete.</param>
        /// <param name="userId">The ID of the user performing the delete operation.</param>
        /// <returns>
        /// A tuple containing:
        /// <c>success</c> — true if deletion succeeded (Result = 1), false otherwise.
        /// <c>message</c> — a descriptive message from the stored procedure.
        /// </returns>
        public (bool success, string message) DeleteCurrency(List<int> currencyId, int userId)
        {
            try
            {
                if (currencyId == null || !currencyId.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string currencyIds = string.Join(",", currencyId);

                var parameters = new DynamicParameters();
                parameters.Add("@CurrencyId", currencyIds);
                parameters.Add("@UserId", userId);

                // Dapper maps the first row directly — no DataTable, no foreach needed
                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Currencies_Delete",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return (result.Result == 1, result.Message ?? "");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Toggles the active/inactive status of a currency record.
        /// </summary>
        /// <param name="currencyId">The unique identifier of the currency to update.</param>
        /// <param name="isActive">True to activate, false to deactivate the currency.</param>
        /// <param name="userId">The ID of the user performing the status change.</param>
        /// <returns>
        /// A tuple containing:
        /// <c>success</c> — true if the operation succeeded (Result = 1 from SP); false otherwise.
        /// <c>message</c> — a descriptive message returned from the stored procedure or exception detail.
        /// </returns>
        public (bool success, string message) ToggleCurrencyStatus(StatusUpdateRequest request)
        {
            try
            {
                
                using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CURRENCYID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Currencies_ToggleStatus",
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

        public async Task<PagedResult<MstCurrencyViewModel>> GetAllCurrencyWithPage(SearchRequest req)
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

                var result = (await conn.QueryAsync<MstCurrencyViewModel>(
                "sp_Currencies_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstCurrencyViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstCurrencyViewModel>
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

        public CurrencyDetailsModel? GetCurrencyByCompanyId(int companyId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<CurrencyDetailsModel>(
                    "SP_MST_Company_Currency_GET",
                    new { COMPANYID = companyId },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}
