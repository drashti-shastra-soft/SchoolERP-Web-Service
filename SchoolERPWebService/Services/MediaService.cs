using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class MediaService: IMediaService
    {
        private readonly IConfiguration _configuration;
        public MediaService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse> Upsert(
           MediaRequest request,
           int userId)
        {
            var response = new ApiResponse();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@MediaJson", request.MediaJson);
                parameters.Add("@CreatedBy", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                   "USP_InsertMediaLibraryBulk",
                   parameters,
                   commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.Result;
                    response.Message = result.Message;
                    response.Data = result.MediaID;
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

        public async Task<PagedResult<MediaViewModel>> GetAllMediaWithPage(SearchRequest request)
        {
            var response = new PagedResult<MediaViewModel>();
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                if (request.PageNumber == 0 && request.PageSize == 0)
                {
                    request.PageNumber = 1;
                    request.PageSize = 10;
                }
                parameters.Add("@COMPANYID", request.CompanyID);
                parameters.Add("@SESSIONID", request.SessionID);
                parameters.Add("@PageIndex", request.PageNumber);
                parameters.Add("@PageSize", request.PageSize);
                parameters.Add("@SearchTerm", request.SearchKeyword);
                var result = (await conn.QueryAsync<MediaViewModel>(
                "USP_GetMediaLibraryPageIndex",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.RESULT ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PAGESIZE ?? 0;

                response = new PagedResult<MediaViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    response = new PagedResult<MediaViewModel>
                    {
                        Data = null,
                        TotalRecords = totalRecords,
                        PageNumber = pageIndex,
                        PageSize = pageSize
                    };
                }
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Unable to fetch lesson plan list. Please try again.";
                response.Data = new List<MediaViewModel>();
                return response;
            }
        }

    }
}
