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
    /// This service handles the actual work of managing language settings, such as saving new languages and updating their details in the database.
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly IConfiguration _configuration;
        public LanguageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves a list of all languages from the database.
        /// </summary>
        public List<MstLanguageViewModel> GetAllLanguages(bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<MstLanguageViewModel>(
                    "sp_Languages_GetAll",
                    new { IncludeDeleted = includeDeleted },
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Looks up the details of a specific language using its unique ID.
        /// </summary>
        public MstLanguageViewModel? GetLanguageByID(int languageId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<MstLanguageViewModel>(
                    "sp_Languages_GetByID",
                    new { LanguageId = languageId },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Saves or updates language information in the database, including the name,
        /// code, icon, RTL status, base language, and active status.
        /// </summary>
        public (bool success, string message) UpsertLanguage(MstLanguageUpsertRequest request, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Languages_Upsert",
                    new
                    {
                        request.LanguageId,
                        request.LanguageTitle,
                        request.LanguageCode,
                        request.LanguageIcon,
                        request.LanguageIsRtl,
                        request.LanguageBase,
                        request.IsActive,
                        UserId = userId
                    },
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
        /// Deletes a language record from the database.
        /// </summary>
        public (bool success, string message) DeleteLanguage(List<int> languageId, int userId)
        {
            try
            {
                if (languageId == null || !languageId.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string languageIds = string.Join(",", languageId);
                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Languages_Delete",
                    new
                    {
                        LanguageId = languageIds,
                        UserId = userId
                    },
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

        /// <summary>
        /// Updates whether a language is currently enabled for use in the application.
        /// </summary>
        
        //public (bool success, string message) ToggleLanguageStatus(int languageId, bool isActive, int userId)
        //{
        //    try
        //    {
        //        using var conn = new SqlConnection(
        //            _configuration.GetConnectionString("DefaultConnection"));

        //        var result = conn.QueryFirstOrDefault<dynamic>(
        //            "sp_Languages_ToggleStatus",
        //            new
        //            {
        //                LanguageId = languageId,
        //                IsActive = isActive,
        //                UserId = userId
        //            },
        //            commandType: CommandType.StoredProcedure);

        //        return (
        //            Convert.ToInt32(result?.Result) == 1,
        //            Convert.ToString(result?.Message) ?? ""
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, ex.Message);
        //    }
        //}
        /// <summary>
        /// A helper tool that converts raw database data about languages into a format that the application can easily understand.
        /// </summary>


        public async Task<PagedResult<MstLanguageViewModel>> GetAllLanguageWithPage(SearchRequest req)
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

                var result = (await conn.QueryAsync<MstLanguageViewModel>(
                "sp_Languages_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstLanguageViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstLanguageViewModel>
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

        public (bool success, string message) ToggleLanguageStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@LANGUAGEID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Languages_ToggleStatus",
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
