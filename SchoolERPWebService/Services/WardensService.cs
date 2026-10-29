using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;

namespace SchoolERPWebService.Services
{
    public class WardensService: IWardensService
    {
        private readonly IConfiguration _configuration;
        public WardensService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // ============================================================
        // UPSERT (CREATE / UPDATE)
        // ============================================================
        public async Task<WardenSaveResponse> UpsertWardensAsync(WardensRequestModel model)
        {
            var response = new WardenSaveResponse();

            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@WardenId", model.WardenId);
                parameters.Add("@HostelID", model.HostelID);
                parameters.Add("@WardenName", model.WardenName);
                parameters.Add("@WardenContact", model.WardenContact);
                parameters.Add("@WardenEmail", model.WardenEmail);
                parameters.Add("@Address", model.Address);
                parameters.Add("@CountryId", model.CountryId);
                parameters.Add("@StateId", model.StateId);
                parameters.Add("@PinCode", model.PinCode);
                parameters.Add("@Gender", model.Gender);
                parameters.Add("@BirthDate", model.BirthDate);
                parameters.Add("@JoiningDate", model.JoiningDate);
                parameters.Add("@Qualification", model.Qualification);
                parameters.Add("@ExperienceYears", model.ExperienceYears);
                parameters.Add("@AadharNumber", model.AadharNumber);
                parameters.Add("@Photo", model.Photo);
                parameters.Add("@EmergencyContactNumber", model.EmergencyContactNumber);
                parameters.Add("@COMPANYID", model.CompanyID);
                parameters.Add("@SESSIONID", model.SessionID);
                parameters.Add("@ISACTIVE", model.IsActive);
                parameters.Add("@USERID", model.UserID);
                parameters.Add("@IPADDRESS", model.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<WardenSaveResponse>(
                    "SP_TBL_MST_HOSTEL_WARDEN_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    response.Result = result.Result;
                    response.Message = result.Message;
                    response.WardenId = result.WardenId;
                    response.TechnicalMessage = result.TechnicalMessage;
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

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<WardensModel?> GetWardensByIdAsync(int countryId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@WARDENID", countryId);

                return await conn.QueryFirstOrDefaultAsync<WardensModel>(
                    "SP_TBL_MST_HOSTELWARDEN_GETBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        // ============================================================
        // GET ALL (BY COMPANY & SESSION)
        // ============================================================
        public async Task<List<WardensModel>> GetAllWardensAsync(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@SessionID", sessionId);
                parameters.Add("@INCLUDEDELETED", includeDeleted);

                var result = conn.Query<WardensModel>(
                    "SP_TBL_MST_HOSTELWARDEN_GETALL",
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

        public async Task<PagedResult<WardensModel>> GetAllWardensWithPage(WardensSearchRequest req)
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
                param.Add("@HOSTELID", req.HostelID);
                param.Add("@SEARCHKEYWORD", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                param.Add("@INCLUDEDELETED", 0);

                var result = (await conn.QueryAsync<WardensModel>(
                "SP_TBL_MST_HOSTELWARDEN_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<WardensModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<WardensModel>
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
        /// Deletes a Wardens  record by its unique ID.
        /// </summary>
        /// <param name="WardensId">Wardens ID.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) DeleteWardens(List<int> ids, int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string wordenID = string.Join(",", ids);

                var parameters = new DynamicParameters();
                parameters.Add("@WARDENID", wordenID);
                parameters.Add("@USERID", userId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                   "SP_MST_HOSTELWARDEN_DELETE",
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
        /// Activates or deactivates a Wardens.
        /// </summary>
        /// <param name="WardensId">Wardens ID.</param>
        /// <param name="isActive">Status to set.</param>
        /// <param name="userId">Logged-in user ID.</param>
        /// <returns>Operation status and message.</returns>
        public (bool success, string message) ToggleWardensStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@WARDENID", request.Ids);
                parameters.Add("@IsActive", request.IsActive);
                parameters.Add("@UserId", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_MST_HOSTELWARDEN_TOGGLESTATUS",
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

        public (bool Success, string Message) UpdateWardenProfile(WardenProfileRequest req)
        {
            try
            {

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@WardenId", req.WardenId);
                parameters.Add("@Photo", req.Photo);
                parameters.Add("@USERID", req.UserId);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_TBL_MST_HOSTEL_WARDEN_UPDATEPROFILE",
                   parameters,
                    commandType: CommandType.StoredProcedure);

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
