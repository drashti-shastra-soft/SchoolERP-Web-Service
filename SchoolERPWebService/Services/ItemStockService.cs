using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class ItemStockService: IItemStockService
    {
        private readonly IConfiguration _configuration;
        public ItemStockService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // ============================================================
        // UPSERT (CREATE / UPDATE)
        // ============================================================
        public SpItemStockReceive UpsertItemStockAsync(ItemStockModelRequest model)
        {
            var response = new SpItemStockReceive();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@ItemStockId", model.ItemStockId);
                parameters.Add("@ItemCategoryId", model.ItemCategoryId);
                parameters.Add("@ItemId", model.ItemId);
                parameters.Add("@ItemStoreId", model.ItemStoreId);
                parameters.Add("@ItemSupplierId", model.ItemSupplierId);
                parameters.Add("@CompanyID", model.CompanyID);
                parameters.Add("@SessionID", model.SessionID);
                parameters.Add("@Quantity", model.Quantity);
                parameters.Add("@PurchasePrice", model.PurchasePrice);
                parameters.Add("@PurchaseDate", model.PurchaseDate);
                parameters.Add("@Description", model.Description);
                parameters.Add("@IsActive", model.IsActive);
                parameters.Add("@UserId", model.UserId);
                parameters.Add("@IPADDRESS", model.IPAddress);

                var result = conn.QueryFirstOrDefault<SpItemStockReceive>(
                    "usp_Inventory_ItemStock_Save",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                response.Result = 0;
                response.Message = "Unable to save hostel type. Please try again.";
                response.TECHNICALMESSAGE = ex.Message;
            }

            return response;
        }

        public async Task<PagedResult<ItemStockModel>> GetAllItemStockWithPage(SearchItemStockRequest req)
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
                param.Add("@ITEMCATEGORYID", req.ItemCategoryId);
                param.Add("@ITEMID", req.ItemId);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", false);

                var result = (await conn.QueryAsync<ItemStockModel>(
                "usp_Inventory_ItemStock_GetAll",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<ItemStockModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ItemStockModel>
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
        public async Task<ItemStockModel?> GetItemStockByIdAsync(int itemStockId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@ItemStockId", itemStockId);

                return await conn.QueryFirstOrDefaultAsync<ItemStockModel>(
                    "usp_Inventory_ItemStock_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public (bool success, string message) DeleteItemStock(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No item category selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string itemStockId = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@Ids", itemStockId);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "usp_Inventory_ItemStock_Delete",
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

        public (bool success, string message) ToggleItemStockStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@Ids", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "usp_Inventory_ItemStock_ToggleStatus",
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

        public (bool success, string message) UpsertItemStockAttachmentFile(
   ItemStockAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@ITEMSTOCKID", req.ItemStockId);
                parameters.Add("@ATTACHMENT", req.Attachment);
                parameters.Add("@FILENAME", req.FileName);
                parameters.Add("@FILETYPE", req.FileType);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "usp_Inventory_ItemStock_UpsertAttachment",
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
