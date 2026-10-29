using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// This service handles the actual work of managing account heads, such as saving, updating, or deleting head records in the database.
    /// </summary>
    public class AccountHeadService: IAccountHeadService
    {
        private readonly IConfiguration _configuration;
        public AccountHeadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Retrieves all account heads based on company, session, head type, and optional deleted filter.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="headType">The type of account head.</param>
        /// <param name="includeDeleted">Whether to include deleted records. Default is false.</param>
        /// <returns>List of <see cref="AccountHeadViewModel"/> matching the criteria.</returns>
        public List<AccountHeadViewModel> GetAllAccountHeads(int companyId, int sessionId, string headType, bool includeDeleted = false)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SessionID", sessionId);
            parameters.Add("@HeadType", headType);
            parameters.Add("@IncludeDeleted", includeDeleted);

            return conn.Query<AccountHeadViewModel>(
                "sp_Mst_AccountHead_GetAll",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();
        }
        /// <summary>
        /// Retrieves a single account head record by its unique identifier.
        /// </summary>
        /// <param name="id">The unique AccountHeadID to look up.</param>
        /// <returns>
        /// An <see cref="AccountHeadViewModel"/> if found; otherwise <c>null</c>.
        /// </returns>
        public AccountHeadModel? GetAccountHeadByID(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<AccountHeadModel>(
                "SP_AccountHead_GetById",
                new { AccountID = id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <summary>
        /// Inserts or updates an account head record by executing the stored procedure
        /// <c>sp_Mst_AccountHead_Upsert</c>.
        /// </summary>
        /// <param name="req">The upsert request containing account head details.</param>
        /// <param name="companyId">The ID of the company.</param>
        /// <param name="sessionId">The current session ID.</param>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <returns>
        /// A tuple where <c>Success</c> is <see langword="true"/> if the upsert succeeded,
        /// and <c>Message</c> contains the result message from the stored procedure.
        /// </returns>
        public (bool Success, string Message) UpsertAccountHead(
            AccountHeadUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@AccountHeadID", req.AccountHeadID);
                p.Add("@CompanyID", companyId);
                p.Add("@SessionID", sessionId);
                p.Add("@HeadName", req.HeadName);
                p.Add("@HeadDescription", req.HeadDescription);   // Dapper handles null → DBNull automatically
                p.Add("@HeadType", req.HeadType);
                p.Add("@DisplayLabel", req.DisplayLabel);
                p.Add("@IsActive", req.IsActive);
                p.Add("@UserId", userId);

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Mst_AccountHead_Upsert", p,
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message!);
            }
            catch (Exception ex) { return (false, ex.Message); }
        }

        /// <summary>
        /// Deletes an account head record by executing the stored procedure and returning success status with message.
        /// </summary>
        /// <param name="id">The AccountHead ID to delete.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <returns>A tuple containing Success flag and Message from the stored procedure result.</returns>
        public (bool Success, string Message) DeleteAccountHead(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string accountHeadIDs = string.Join(",", id);

                var parameters = new DynamicParameters();
                parameters.Add("@AccountIDs", accountHeadIDs);
                parameters.Add("@UserId", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                  "SP_AccountHead_Delete",
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
        /// Toggles the active/inactive status of an account head record.
        /// </summary>
        /// <param name="id">The unique identifier of the account head.</param>
        /// <param name="isActive">True to activate, false to deactivate the account head.</param>
        /// <param name="userId">The ID of the user performing the status change.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        ///   <item><description><c>Success</c>: True if the operation succeeded, false otherwise.</description></item>
        ///   <item><description><c>Message</c>: A status message returned from the stored procedure.</description></item>
        /// </list>
        /// </returns>
        public (bool Success, string Message) ToggleAccountHeadStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@AccountID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);

                // Dapper maps the first row automatically — no foreach or DataTable needed
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_mst_ACC_AccountHead_TOGGLESTATUS",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result is not null
                    ? (result.Result == 1, result.Message ?? string.Empty)
                    : (false, "No response from stored procedure.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public async Task<PagedResult<AccountHeadViewModel>> GetAllAccountHeadWithPageIndex(AccountHeadSearchRequest req)
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
                param.Add("@HEADTYPE", req.HeadType);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<AccountHeadViewModel>(
                "SP_MST_ACCOUNTHEAD_GETALLPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<AccountHeadViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AccountHeadViewModel>
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

        public async Task<(bool Success, string Message)> CopyAccountHead(CopyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROMCOMPANYID", req.FromCompanyId);
                parameters.Add("@FROMSESSIONID", req.FromSessionId);
                parameters.Add("@TOCOMPANYID", req.ToCompanyId);
                parameters.Add("@TOSESSIONID", req.ToSessionId);
                parameters.Add("@USERID", req.UserID);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_MST_ACCOUNTHEAD_COPYSESSION",
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

        public List<DropdownModel> GetAccountParentDropdownList(int? excludeAccountID=null, int? companyId=null, int? sessionId=null)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "SP_AccountHead_GetParentDropdown",
                    new
                    {
                        ExcludeAccountID = excludeAccountID,
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public SpAccount UpsertAccountHead(AccountRequest request)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@AccountID", request.AccountID);
                parameters.Add("@AccountName", request.AccountName);
                parameters.Add("@AccountTypeID", request.AccountTypeID);
                parameters.Add("@ParentAccountID", request.ParentAccountID);
                parameters.Add("@AccountNo", request.AccountNo);
                parameters.Add("@CostCenterID", request.CostCenterID);

                parameters.Add("@PostalCode", request.PostalCode);
                parameters.Add("@CountryId", request.CountryId);
                parameters.Add("@StateId", request.StateId);
                parameters.Add("@CityId", request.CityId);
                parameters.Add("@AreaID", request.AreaID);

                parameters.Add("@Email", request.Email);
                parameters.Add("@RepresentedBy", request.RepresentedBy);
                parameters.Add("@PayeeName", request.PayeeName);
                parameters.Add("@Address", request.Address);

                parameters.Add("@PhoneOff", request.PhoneOff);
                parameters.Add("@PhoneRes", request.PhoneRes);
                parameters.Add("@MobileNo", request.MobileNo);
                parameters.Add("@Fax", request.Fax);

                parameters.Add("@CreditDays", request.CreditDays);
                parameters.Add("@CreditLimit", request.CreditLimit);
                parameters.Add("@OpeningBalance", request.OpeningBalance);

                parameters.Add("@OpeningBalType", request.OpeningBalType);
                parameters.Add("@OpeningDate", request.OpeningDate);

                parameters.Add("@TinNo", request.TinNo);
                parameters.Add("@PANNo", request.PANNo);
                parameters.Add("@STTNo", request.STTNo);
                parameters.Add("@ServiceTaxNo", request.ServiceTaxNo);
                parameters.Add("@CSTNo", request.CSTNo);
                parameters.Add("@ECCNo", request.ECCNo);

                parameters.Add("@IsBlocked", request.IsBlocked);
                parameters.Add("@IsActive", request.IsActive);
               // parameters.Add("@IsDelete", request.IsDelete);

             //   parameters.Add("@CreatedBy", request.CreatedBy);
               // parameters.Add("@ModifiedBy", request.ModifiedBy);

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@SessionID", request.SessionID);
                parameters.Add("@UserID", request.UserID);
                //parameters.Add("@UserName", request.UserName);
                //parameters.Add("@IPAddress", request.IPAddress);

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));


                var result = conn.QueryFirstOrDefault<SpAccount>(
                    "SP_AccountHead_Upsert",
                   parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;

            }
            catch (Exception ex)
            {
                var model = new SpAccount();
                model.Result = 0;
                model.Message = ex.Message;
                model.AccountID = 0;
                return model;
            }
        }

        public async Task<PagedResult<AccountHeadModel>> GetAllListAccountHeadWithPageIndex(SearchAccountHeadRequest req)
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
                param.Add("@COSTCENTERID", req.CostCenterID);
                param.Add("@OPENINGBALTYPE", req.OpeningBalType);
                param.Add("@INCLUDEDELETED", 0);
                param.Add("@SEARCHTEXT", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<AccountHeadModel>(
                "SP_AccountHead_GetAllPageIndex",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<AccountHeadModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AccountHeadModel>
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

        public (bool success, string message) UpsertAccountHeadAttachmentFile(
  AccountEntryAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@AccountID", req.AccountEntryID);
                parameters.Add("@ATTACHDOC", req.Attachment);
                parameters.Add("@ATTACHDOCNAME", req.FileName);
                parameters.Add("@ATTACHDOCTYPE", req.FileType);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_mst_ACC_AccountHead_UPSERT_ATTACHMENT",
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
