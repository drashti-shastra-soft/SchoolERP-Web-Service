using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Interfaces;
using System.Data;
using System.Net;

namespace SchoolERPWebService.Services
{
    public class SubjectTypeService: ISubjectTypeService
    {
        private readonly IConfiguration _configuration;
        public SubjectTypeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves all subjects for the specified company and session.
        /// Optionally includes deleted subjects.
        /// </summary>

        public async Task<PagedResult<SubjectTypeModel>> GetAllSubjectTypeWithPage(SubjectSearchRequest req)
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


                param.Add("@CompanyID", req.CompanyID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@SearchKeyword", req.SearchKeyword);
                param.Add("@PageNumber", req.PageNumber);
                param.Add("@PageSize", req.PageSize);

                var result = (await conn.QueryAsync<SubjectTypeModel>(
                "sp_SubjectType_GetAllWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<SubjectTypeModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<SubjectTypeModel>
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


        public async Task<List<SubjectTypeModel>> GetAllSubjectTypeAsync(int companyId, int sessionId, bool includeDeleted = false)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@INCLUDEDELETED", includeDeleted);

                var result = conn.Query<SubjectTypeModel>(
                    "sp_SubjectType_GetAll",
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
        /// <summary>
        /// Retrieves a subject by its unique identifier.
        /// </summary>
        public SubjectTypeModel? GetSubjectTypeByID(int subjectTypeID)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<SubjectTypeModel>(
                    "sp_SubjectType_GetByID",
                    new
                    {
                        SubjectTypeID = subjectTypeID
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Inserts or updates a subject record.
        /// </summary>
        public (bool success, string message) UpsertSubject(SubjectTypeRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();

                parameters.Add("@SubjectTypeID", request.SubjectTypeID, DbType.Int32);
                parameters.Add("@CompanyID", request.CompanyID, DbType.Int32);
                parameters.Add("@SessionID", request.SessionID, DbType.Int32);
                parameters.Add("@SubjectTypeName", request.SubjectTypeName, DbType.String);
                parameters.Add("@DisplayLabel", request.DisplayLabel, DbType.String);
                parameters.Add("@IsActive", request.IsActive, DbType.Boolean);
                parameters.Add("@UserId", request.UserId, DbType.Int32);
                parameters.Add("@IPAddress", request.IPAddress, DbType.String);


                var result =  conn.QueryFirstOrDefault<SpResult>(
               "sp_SubjectType_Upsert",
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

        /// <summary>
        /// Deletes a subject record.
        /// </summary>
        public (bool success, string message) DeleteSubjectType(List<int> subjectId, int userId,string ipAddress)
        {
            try
            {
                if (subjectId == null || !subjectId.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", subjectId);

                var parameters = new DynamicParameters();
                parameters.Add("@SubjectTypeID", id);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPAddress", ipAddress);
                var result = conn.QueryFirstOrDefault<SpResult>(
                  "sp_SubjectType_Delete",
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
        /// Changes the active status of a subject.
        /// </summary>
        public (bool success, string message) ToggleSubjectTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@SubjectTypeID", request.Ids);
                parameters.Add("@ISACTIVE", request.IsActive);
                parameters.Add("@USERID", request.DoneBy);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_SubjectType_ToggleStatus",
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

        public async Task<(bool Success, string Message)> CopySubjectTypeToSession(CopyRequest req)
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
                    "SP_SubjectType_Copy",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (
                    Convert.ToInt32(result.Result) == 1,
                    Convert.ToString(result.Message) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                SpResult model = new SpResult();
                model.Result = 0;
                model.Message = ex.Message;
                return (
                    Convert.ToInt32(model.Result) == 1,
                    Convert.ToString(model.Message) ?? string.Empty
                );
            }

        }

        public List<DropdownModel> GetSubjectsTypeDropdownList(int companyID,int sessionID,int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "sp_tbl_Mst_SubjectsType_GETAllDROPDOWNBINDING",
                new
                {
                    CompanyID = companyID,
                    SessionID = sessionID,
                    UserId = userId,
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
    }
}
