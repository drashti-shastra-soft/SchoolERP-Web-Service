using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class IssueItemService: IIssueItemService
    {
        private readonly IConfiguration _configuration;
        public IssueItemService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================================================
        // UPSERT (CREATE / UPDATE)
        // ============================================================
        public async Task<ApiResponse> UpsertItemIssueAsync(ItemIssueRequest model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", model.RoleId);
                parameters.Add("@ISSUETO", model.IssueTo);
                parameters.Add("@ISSUEBY", model.IssueBy);
                parameters.Add("@ISSUEDATE", model.IssueDate);
                parameters.Add("@RETURNDATE", model.ReturnDate);
                parameters.Add("@NOTE", model.Note);
                parameters.Add("@ITEMCATEGORYID", model.ItemCategoryId);
                parameters.Add("@ITEMID", model.ItemId);
                parameters.Add("@QUANTITY", model.Quantity);
                //parameters.Add("@ItemIssueId", model.ItemIssueId);
                parameters.Add("@CompanyID", model.CompanyID);
                parameters.Add("@SessionID", model.SessionID);  
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@IPADDRESS", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "USP_INVENTORY_ISSUEITEM_SAVE",
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

        public async Task<PagedResult<ItemIssueModel>> GetAllItemIssueWithPage(SearchInventoryItemRequest req)
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
                param.Add("@STATUS", req.SearchKeyword);
                param.Add("@ITEMCATEGORYID", req.ItemCategoryId);
                param.Add("@ITEMID", req.ItemId);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<ItemIssueModel>(
                "USP_INVENTORY_ISSUEITEM_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<ItemIssueModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ItemIssueModel>
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

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<ItemIssueModel?> GetItemIssueByIdAsync(int itemcategoryId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ITEMISSUEID", itemcategoryId);

                return await conn.QueryFirstOrDefaultAsync<ItemIssueModel>(
                    "USP_INVENTORY_ISSUEITEM_GETBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeleteItemIssue(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item category selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string itemCategoryId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@IDS", itemCategoryId);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "USP_INVENTORY_ISSUEITEM_DELETE",
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

        public (bool success, string message) ToggleItemIssueStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@IDS", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "USP_INVENTORY_ISSUEITEM_TOGGLESTATUS",
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

        public async Task<ApiResponse> UpsertItemIssueReturnAsync(ItemIssueReturnModel model)
        {
            var response = new ApiResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@ITEMISSUEID", model.ItemIssueId);
                parameters.Add("@QUANTITYRETURNED", model.QuantityReturned);
                parameters.Add("@RETURNEDBY", model.UserId);
                parameters.Add("@ACTUALRETURNDATE", model.ActualReturnDate);
                parameters.Add("@USERID", model.UserId);
                parameters.Add("@IPADDRESS", model.IpAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "USP_INVENTORY_ISSUEITEM_CONFIRMRETURN",
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

    }
}
