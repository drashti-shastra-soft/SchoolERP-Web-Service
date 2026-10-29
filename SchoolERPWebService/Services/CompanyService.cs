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
    /// This service performs the actual work of managing company information, such as saving, updating, or deleting school records from the database.
    /// </summary>
    public class CompanyService: ICompanyService
    {
        private readonly IConfiguration _configuration;
        public CompanyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves a complete list of all companies from the database.
        /// </summary>
        /// <param name="includeDeleted">Indicates whether deleted companies should be included.</param>
        /// <returns>A list of company records.</returns>
        public List<MstCompanyViewModel> GetAllCompanies(bool includeDeleted = false,int? userId=null)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@USERID", userId);

            return conn.Query<MstCompanyViewModel>(
                "sp_Companies_GetAll",
                parameters,
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        /// <summary>
        /// Retrieves all companies assigned to the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of companies assigned to the user.</returns>
        public List<MstCompanyViewModel> GetCompaniesByUserId(int userId)
        {

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var rows = conn.Query<CurrentCompanyResult>(
             "sp_UserCompanies_GetByUser",
             new { UserID = userId },
             commandType: CommandType.StoredProcedure);

                //var assignedCompanyIds = rows
                //    .Where(r => r.Result == 1)          // drop failure/status-only rows if any
                //    .Select(r => r.CompanyId)
                //    .Where(id => id.HasValue)
                //    .Select(id => id!.Value)
                //    .ToHashSet();                        // O(1) lookups instead of O(n) per Contains call

                return GetAllCompanies(false, userId)
                    //.Where(c => assignedCompanyIds.Contains(c.CompanyId))
                    .ToList();
            }
            catch
            {
                return null;
            }

            
        }

        /// <summary>
        /// Retrieves company details by company ID.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <returns>The company details if found; otherwise, null.</returns>
        public MstCompanyViewModel? GetCompanyByID(int companyId)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<MstCompanyViewModel>(
                "sp_Companies_GetByID",
                new { CompanyId = companyId },
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Creates a new company or updates an existing company.
        /// </summary>
        /// <param name="request">Company information to save.</param>
        /// <param name="userId">Current user identifier.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) UpsertCompany(MstCompanyUpsertRequest request, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                     _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Companies_Upsert",
                    new
                    {
                        request.CompanyId,
                        request.SchoolName,
                        request.SchoolCode,
                        request.ParentCompanyId,
                        request.Address,
                        request.HeadOfficeAddress,
                        request.Phone,
                        request.Email,
                        request.SessionId,
                        request.AdmissionNo,
                        request.BoardName,
                        request.SchoolStatus,
                        request.LateFinePerDay,
                        request.LibraryFinePerDay,
                        request.LateFineMaxPerBook,
                        request.CurrencyId,
                        request.SessionStartMonth,
                        request.DateFormat,
                        request.Timezone,
                        request.StartDayOfWeek,
                        request.CurrencyFormat,
                        request.IsActive,
                        UserId = userId,
                        PostalCode= request.PostalCode,
                        request.CountryId,
                        request.StateId,
                        request.CityId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a company from the system.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="userId">Current user identifier.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) DeleteCompany(List<int> companyId, int userId)
        {
            try
            {

                if (companyId == null || !companyId.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string companyIds = string.Join(",", companyId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Companies_Delete",
                    new
                    {
                        CompanyId = companyIds,
                        UserId = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Activates or deactivates a company.
        /// </summary>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="isActive">Status to apply.</param>
        /// <param name="userId">Current user identifier.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) ToggleStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

               
                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Companies_ToggleStatus",
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
        /// Updates the currently selected company for a user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) UpdateUserCurrentCompany(int userId, int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Users_UpdateCurrentCompany",
                    new
                    {
                        UserID = userId,
                        DoneBy = userId,
                        CurrentCompanyId = companyId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result?.Result == 1, result?.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the currently selected company for a user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Company identifier if found; otherwise, null.</returns>
        public int? GetUserCurrentCompany(int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var row = conn.QueryFirstOrDefault<CurrentCompanyResult>(
                    "sp_Users_GetCurrentCompany",
                    new { UserID = userId },
                    commandType: CommandType.StoredProcedure);

                return row?.Result == 1 ? row.CompanyId : null;
            }
            catch
            {
                return null;
            }
        }


        public async Task<PagedResult<MstCompanyViewModel>> GetAllCompanyWithPage(SearchRequest req)
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
                param.Add("@USERID", req.UserId);

                var result = (await conn.QueryAsync<MstCompanyViewModel>(
                "sp_Companies_GetAllPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TotalRecords ?? 0;
                int pageIndex = result.FirstOrDefault()?.PageNumber ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<MstCompanyViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<MstCompanyViewModel>
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

        public async Task<(bool Success, string Message, string FileName)> UpdateCompanyImageAsync(int companyId, string imageType, IFormFile file, int userId)
        {
            var company = GetCompanyByID(companyId);
            if (company == null)
                return (false, "Company not found.", null);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(wwwrootPath, "Uploads", "Company", imageType);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{companyId}_{imageType}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@Attachment", fileName);
            parameters.Add("@AttachmentType", imageType);
            parameters.Add("@AttachementName", file.FileName);
            parameters.Add("@UserId", userId);

            var logoId = await conn.QuerySingleOrDefaultAsync<int?>(
                "USP_MST_Company_Logo_Upsert",
                parameters,
                commandType: CommandType.StoredProcedure);

            if (logoId == null || logoId <= 0)
                return (false, "Could not save logo record.", null);

            return (true, "Image uploaded successfully.", fileName);
        }

        public async Task<string> GetCompanyLogoFileNameAsync(int companyId, string imageType)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyId", companyId);
            parameters.Add("@AttachmentType", imageType);

            var result = await conn.QuerySingleOrDefaultAsync<dynamic>(
                "USP_MST_Company_Logo_GetByType",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result?.Attachment;
        }

        public (bool success, string message) SaveCompanyLogoAsync(CompanyLogoRequest request)
        {
            try
            {
                
                using var conn = new SqlConnection(
          _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@CompanyID", request.CompanyID);
                parameters.Add("@UserID", request.UserID);

                parameters.Add("@PrintLogo", request.PrintLogo);
                parameters.Add("@PrintLogoType", request.PrintLogoType);
                parameters.Add("@PrintLogoName", request.PrintLogoName);

                parameters.Add("@AdminLogo", request.AdminLogo);
                parameters.Add("@AdminLogoType", request.AdminLogoType);
                parameters.Add("@AdminLogoName", request.AdminLogoName);

                parameters.Add("@AdminSmallLogo", request.AdminSmallLogo);
                parameters.Add("@AdminSmallLogoType", request.AdminSmallLogoType);
                parameters.Add("@AdminSmallLogoName", request.AdminSmallLogoName);

                parameters.Add("@WaterMarkLogo", request.WaterMarkLogo);
                parameters.Add("@WaterMarkLogoType", request.WaterMarkLogoType);
                parameters.Add("@WaterMarkLogoName", request.WaterMarkLogoName);

                parameters.Add("@BackgroundLogo", request.BackgroundLogo);
                parameters.Add("@BackgroundLogoType", request.BackgroundLogoType);
                parameters.Add("@BackgroundLogoName", request.BackgroundLogoName);

                var result =  conn.QueryFirstOrDefault<SpResult>(
                    "sp_CompanyLogo_Save",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                

                return (result?.Result == 1, result?.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public List<CompanyLogoResponse> GetAllCompanyLogoById(int? companyID = null)
        {
            using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@CompanyID", companyID);

            return conn.Query<CompanyLogoResponse>(
                "sp_CompanyLogo_GetByCompany",
                parameters,
                commandType: CommandType.StoredProcedure)
                .ToList();
        }
    }
}
