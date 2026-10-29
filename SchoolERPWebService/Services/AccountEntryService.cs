using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.Design;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing account entries, such as saving, updating, or searching income and expense transactions in the database.
    /// </summary>
    public class AccountEntryService: IAccountEntryService
    {
        private readonly IConfiguration _configuration;
        public AccountEntryService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Retrieves all account entries for the specified company and session.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="entryType">The type of account entry.</param>
        /// <param name="includeDeleted">Whether to include deleted entries. Default is false.</param>
        /// <returns>A list of <see cref="AccountEntryViewModel"/> objects.</returns>
        public List<AccountEntryViewModel> GetAllAccountEntries(int companyId, int sessionId, string entryType, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@EntryType", entryType);
                parameters.Add("@IncludeDeleted", includeDeleted);

                return conn.Query<AccountEntryViewModel>(
                    "sp_Trn_AccountEntry_GetAll",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                return new List<AccountEntryViewModel>();
            }
        }

        public async Task<PagedResult<AccountEntryViewModel>> GetAllAccountEntriesdWithPageIndex(AccountHeadSearchRequest req)
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
                param.Add("@ENTRYTYPE", req.HeadType);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<AccountEntryViewModel>(
                "SP_TRN_ACCOUNTENTRY_GETALLPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<AccountEntryViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AccountEntryViewModel>
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

                return null;
            }
        }

        /// <summary>
        /// Searches account entries based on company, session, entry type, search type, and optional date range.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="entryType">The type of account entry.</param>
        /// <param name="searchType">The search type filter.</param>
        /// <param name="dateFrom">Optional start date in dd-MM-yyyy format.</param>
        /// <param name="dateTo">Optional end date in dd-MM-yyyy format.</param>
        /// <returns>A list of <see cref="AccountEntryViewModel"/> matching the search criteria.</returns>
       

        public async Task<PagedResult<AccountEntryViewModel>> SearchAccountEntries(AccountEntrySearchRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();
                if (req.PageIndex == 0 && req.PageSize == 0)
                {
                    req.PageIndex = 1;
                    req.PageSize = 10;
                }


                param.Add("@COMPANYID", req.CompanyId, DbType.Int32);
                param.Add("@SESSIONID", req.SessionId, DbType.Int32);
                param.Add("@ENTRYTYPE", req.EntryType, DbType.String);
                param.Add("@SEARCHTYPE", req.SearchType, DbType.String);
                param.Add("@DATEFROM", req.DateFrom , DbType.DateTime);
                param.Add("@DATETO", req.DateTo , DbType.DateTime);
                param.Add("@SEARCHKEYWORD", req.Searchkeyword, DbType.String);
                param.Add("@PAGEINDEX", req.PageIndex, DbType.Int32);
                param.Add("@PAGESIZE", req.PageSize, DbType.Int32);

                //var result = connection.Query(
                //    "sp_Trn_AccountEntry_Search",
                //    p,
                //    commandType: CommandType.StoredProcedure
                //);

                var result = (await conn.QueryAsync<AccountEntryViewModel>(
                "sp_Trn_AccountEntry_Search",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<AccountEntryViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AccountEntryViewModel>
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

                return null;
            }
        }

        /// <summary>
        /// Retrieves a single account entry by its ID.
        /// </summary>
        /// <param name="id">The AccountEntry ID to look up.</param>
        /// <returns>An <see cref="AccountEntryViewModel"/> if found; otherwise <c>null</c>.</returns>
        public AccountEntryViewModel? GetAccountEntryByID(int id)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
            return conn.QueryFirstOrDefault<AccountEntryViewModel>(
                "sp_Trn_AccountEntry_GetByID",
                new { AccountEntryID = id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Inserts or updates an account entry record using the stored procedure <c>sp_Trn_AccountEntry_Upsert</c>.
        /// </summary>
        /// <param name="req">The account entry request model containing all entry details.</param>
        /// <param name="companyId">The ID of the company performing the operation.</param>
        /// <param name="sessionId">The current session ID.</param>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <returns>
        /// A tuple where <c>Success</c> indicates whether the upsert was successful,
        /// and <c>Message</c> contains the result message from the stored procedure.
        /// </returns>
        public SpExpense UpsertAccountEntry(AccountEntryUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@AccountEntryID", req.AccountEntryID);
                p.Add("@CompanyID", companyId);
                p.Add("@SessionID", sessionId);
                p.Add("@AccountHeadID", req.AccountHeadID);
                p.Add("@EntryType", req.EntryType);
                p.Add("@Name", req.Name);
                p.Add("@InvoiceNo", req.InvoiceNo);
                p.Add("@Date", req.Date);
                p.Add("@Amount", req.Amount);
                p.Add("@AttachDoc", req.AttachDoc);
                p.Add("@AttachDocType", req.AttachDocType);
                p.Add("@AttachDocName", req.AttachDocName);
                p.Add("@Description", req.Description);
                p.Add("@IsActive", req.IsActive);
                p.Add("@UserId", userId);

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                //var result = conn.QueryFirstOrDefault<SpResult>(
                //    "sp_Trn_AccountEntry_Upsert",
                //    p,
                //    commandType: CommandType.StoredProcedure
                //);
                var result = conn.QueryFirstOrDefault<SpExpense>(
                    "sp_Trn_AccountEntry_Upsert",
                    p,
                    commandType: CommandType.StoredProcedure
                );
                return result;
                
            }
            catch (Exception ex)
            {
                var model = new SpExpense();
                model.Result = 0;
                model.Message = ex.Message;
                model.ACCOUNTENTRYID = 0;
                return model;
            }
        }


        /// <summary>
        /// Deletes an account entry by executing the stored procedure sp_Trn_AccountEntry_Delete.
        /// </summary>
        /// <param name="id">The ID of the account entry to delete.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <returns>A tuple indicating success status and a message from the stored procedure.</returns>
        public (bool Success, string Message) DeleteAccountEntry(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No Income selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string accountEntryID = string.Join(",", id);

                var parameters = new DynamicParameters();
                parameters.Add("@AccountEntryID", accountEntryID);
                parameters.Add("@UserId", userId);


                var result = conn.QueryFirstOrDefault<SpResult>(
                   "sp_Trn_AccountEntry_Delete",
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
        /// Toggles the active/inactive status of an account entry.
        /// </summary>
        /// <param name="id">The AccountEntry ID to toggle.</param>
        /// <param name="isActive">True to activate, false to deactivate.</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        /// <returns>
        /// A tuple containing:
        /// <c>Success</c> — true if the stored procedure returned Result = 1; otherwise false.
        /// <c>Message</c> — the message returned by the stored procedure, or the exception message on failure.
        /// </returns>
        public (bool Success, string Message) ToggleAccountEntryStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault(
                    "sp_Trn_AccountEntry_ToggleStatus",
                    new { AccountEntryID = request.Ids, IsActive = request.IsActive, UserId = request.DoneBy },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message?.ToString() ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public (bool success, string message) UpsertAccountEntryAttachmentFile(
  AccountEntryAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@ACCOUNTENTRYID", req.AccountEntryID);
                parameters.Add("@ATTACHDOC", req.Attachment);
                parameters.Add("@ATTACHDOCNAME", req.FileName);
                parameters.Add("@ATTACHDOCTYPE", req.FileType);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_TRN_ACCOUNTENTRY_UPSERT_ATTACHMENT",
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
