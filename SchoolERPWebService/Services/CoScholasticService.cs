using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class CoScholasticService: ICoScholasticService
    {
        private readonly IConfiguration _configuration;
        public CoScholasticService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<PagedResult<CoScholasticViewModel>> GetAllCoScholasticWithPage(CoScholasticSearchRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                if (request.PageNumber == 0 && request.PageSize == 0)
                {
                    request.PageNumber = 1;
                    request.PageSize = 10;
                }
                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@PageIndex", request.PageNumber, DbType.Int32);
                parameters.Add("@PageSize", request.PageSize, DbType.Int32);
                parameters.Add("@SearchText", request.SearchTerm, DbType.String);
                parameters.Add("@ParentID", request.ParentID, DbType.Int32);
                parameters.Add("@IsActive", request.IsActive, DbType.Boolean);
                parameters.Add("@SortColumn", request.SortColumn, DbType.String);
                parameters.Add("@SortDirection", request.SortDirection, DbType.String);

                var result = (await conn.QueryAsync<CoScholasticViewModel>(
                "sp_CoScholastic_GetPageIndex",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<CoScholasticViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<CoScholasticViewModel>
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

        public (bool success, string message) UpsertExam(CoScholasticRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CoScholasticID", request.CoScholasticID, DbType.Int32);
                parameters.Add("@ParentID", request.ParentID, DbType.Int32);
                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@SrNo", request.SrNo, DbType.Int32);
                parameters.Add("@Title", request.Title, DbType.String);
                parameters.Add("@IsGroup", request.IsGroup, DbType.Boolean);
                parameters.Add("@IsActive", request.IsActive, DbType.Boolean);
                parameters.Add("@UserId", request.UserId, DbType.Int32);
                parameters.Add("@IPAddress", request.IPAddress, DbType.String);

                var result = conn.QueryFirstOrDefault<SpResult>(
               "sp_CoScholastic_Upsert",
               parameters,
               commandType: CommandType.StoredProcedure);



                return (
                    Convert.ToInt32(result?.Result) == 1,
                    Convert.ToString(result?.Message) ?? ""
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public CoScholasticViewModel? GetCoScholasticByID(int coScholasticID)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<CoScholasticViewModel>(
                    "sp_CoScholastic_GetById",
                    new
                    {
                        CoScholasticID = coScholasticID
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<List<CoScholasticViewModel>> GetAllCoScholasticAsync(int companyId, bool? isActive = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@IsActive", isActive);

                var result = conn.Query<CoScholasticViewModel>(
                    "sp_CoScholastic_GetAll",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).ToList();

                // If SP returned no rows at all
                if (!result.Any()) return null;

                // If SP returned rows but RESULT != 1 (failure case)
                if (result.First().Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public (bool success, string message) DeleteCoScholastic(List<int> coScholasticID, int userId, string ipAddress)
        {
            try
            {
                if (coScholasticID == null || !coScholasticID.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", coScholasticID);

                var parameters = new DynamicParameters();
                parameters.Add("@CoScholasticID", id);
                parameters.Add("@UserId", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_CoScholastic_MultiDelete",
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

        public (bool success, string message) ToggleCoScholasticStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CoScholasticID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);
                parameters.Add("@IPAddress", request.IpAddress);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_CoScholastic_MultiToggleStatus",
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
    }
}
