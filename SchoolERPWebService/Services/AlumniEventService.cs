using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using System.Data;
using System.Data.Common;
using SchoolERPWebService.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace SchoolERPWebService.Services
{
    public class AlumniEventService: IAlumniEventService
    {
        private readonly IConfiguration _configuration;
        public AlumniEventService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<AlumniEventViewModel> GetEvents(string? searchText, int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Action", "LIST");
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SearchText", searchText);

            using var conn = new SqlConnection(
                  _configuration.GetConnectionString("DefaultConnection")); // or however you get IDbConnection
            var list = conn.Query<AlumniEventViewModel>(
                "sp_AlumniEvents_CRUD",
                parameters,
                commandType: CommandType.StoredProcedure
            ).ToList();

            return list;
        }

        /// <summary>
        /// Inserts or updates an alumni event record in the database.
        /// Calls the stored procedure <c>sp_AlumniEvents_CRUD</c> with action "UPSERT".
        /// </summary>
        /// <param name="req">The request object containing all event details to upsert.</param>
        /// <param name="companyId">The ID of the company performing the operation.</param>
        /// <param name="userId">The ID of the user performing the operation.</param>
        /// <returns>
        /// A tuple where <c>Success</c> is <c>true</c> if the upsert succeeded (Result == 1),
        /// and <c>Message</c> contains the response message from the stored procedure.
        /// </returns>
        public SpEvent UpsertEvent(AlumniEventUpsertRequest req, int companyId, int userId)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Action", "UPSERT");
                parameters.Add("@EventID", req.EventID);
                parameters.Add("@EventTitle", req.EventTitle);
                parameters.Add("@EventDescription", req.EventDescription);
                parameters.Add("@FromDate", req.FromDate);
                parameters.Add("@ToDate", req.ToDate);
                parameters.Add("@Location", req.Location);
                parameters.Add("@EventPhoto", req.EventPhoto, DbType.Binary);
                parameters.Add("@EventPhotoType", req.EventPhotoType);
                parameters.Add("@EventPhotoName", req.EventPhotoName);
                parameters.Add("@EventFor", req.EventFor);
                parameters.Add("@SessionID", req.SessionID);
                parameters.Add("@ClassID", req.ClassID);
                parameters.Add("@SectionIDs", req.SectionIDs);
                parameters.Add("@CompanyID", companyId);
                parameters.Add("@UserID", userId);

                using var conn = new SqlConnection(
                      _configuration.GetConnectionString("DefaultConnection"));  // or however you get IDbConnection
                                                                                 //var result = conn.QueryFirstOrDefault(
                                                                                 //    "sp_AlumniEvents_CRUD",
                                                                                 //    parameters,
                                                                                 //    commandType: CommandType.StoredProcedure
                                                                                 //);
                var result = conn.QueryFirstOrDefault<SpEvent>(
                        "sp_AlumniEvents_CRUD",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                return result;
            }
            catch (Exception ex)
            {
                var model = new SpEvent();
                model.Result = 0;
                model.Message = ex.Message;
                model.EventID = 0;
                return model;
            }
            
        }

        /// <summary>
        /// Deletes an alumni event from the database for the specified company and user.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event to delete.</param>
        /// <param name="companyId">The unique identifier of the company associated with the event.</param>
        /// <param name="userId">The unique identifier of the user performing the deletion.</param>
        /// <returns>A tuple containing a Success flag (true if deleted successfully) and a Message describing the result.</returns>
        public (bool Success, string Message) DeleteEvent(List<int> id, int companyId, int userId)
        {
            if (id == null || !id.Any())
            {
                return (false, "No students selected for deletion.");
            }
            string eventID = string.Join(",", id);

            var parameters = new DynamicParameters();
            parameters.Add("@EVENTIDS", eventID);
            parameters.Add("@USERID", userId);
            parameters.Add("@COMPANYID", companyId);
            using var conn = new SqlConnection(
                  _configuration.GetConnectionString("DefaultConnection"));
            var result = conn.QueryFirstOrDefault<SpResult>(
                "SP_ALUMNIEVENTS_DELETE",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return (
                   result?.Result == 1,
                   result?.Message ?? "Operation completed."
               );
        }

        /// <summary>
        /// Toggles the active/inactive status of an alumni event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="isActive">The new active status to set for the event.</param>
        /// <param name="companyId">The ID of the company associated with the event.</param>
        /// <param name="userId">The ID of the user performing the status toggle.</param>
        /// <returns>A tuple containing Success (bool) and Message (string) indicating the result.</returns>
        public (bool Success, string Message) ToggleEventStatus(StatusUpdateRequest request, int companyId, int sessionId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@EVENTIDS", request.Ids);
            parameters.Add("@ISACTIVE", request.IsActive);
            //parameters.Add("@CompanyID", companyId);
            //parameters.Add("@SESSIONID", sessionId);
            parameters.Add("@UserID", request.DoneBy);
            using var conn = new SqlConnection(
                  _configuration.GetConnectionString("DefaultConnection"));
            
            var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_ALUMNIEVENTS_TOGGLESTATUS",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            return (
                result?.Result == 1,
                result?.Message ?? "Operation completed."
            );
            
        }

        /// <summary>
        /// Retrieves the photo associated with a specific alumni event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <returns>
        /// A tuple containing:
        /// - <c>Bytes</c>: The raw byte array of the event photo, or <c>null</c> if not found.
        /// - <c>FileName</c>: The file name of the photo, or <c>null</c> if not found.
        /// - <c>ContentType</c>: The MIME content type of the photo, or <c>null</c> if not found.
        /// </returns>
        public (byte[]? Bytes, string? FileName, string? ContentType) GetEventPhoto(int eventId, int companyId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Action", "GET_BY_ID");
            parameters.Add("@EventID", eventId);
            parameters.Add("@CompanyID", companyId);

            using var conn = new SqlConnection(
                  _configuration.GetConnectionString("DefaultConnection"));

            var row = conn.QueryFirstOrDefault(
                "sp_AlumniEvents_CRUD",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (row != null && row.EventPhoto != null)
            {
                return ((byte[])row.EventPhoto, (string?)row.EventPhotoName, (string?)row.EventPhotoType);
            }

            return (null, null, null);
        }


        public async Task<PagedResult<AlumniEventViewModel>> GetAllEventsWithPage(SearchRequest req)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }

                parameters.Add("@COMPANYID", req.CompanyID);
                parameters.Add("@SESSIONID", req.SessionID);
                parameters.Add("@SEARCHTEXT", req.SearchKeyword);
                parameters.Add("@PAGEINDEX", req.PageNumber);
                parameters.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<AlumniEventViewModel>(
                "SP_ALUMNIEVENTS_GETALL",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<AlumniEventViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<AlumniEventViewModel>
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

        public AlumniEventViewModel GetEventsById(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.QueryFirstOrDefault<AlumniEventViewModel>(
                    "SP_ALUMNIEVENTS_GETBYID",
                    new
                    {
                        EVENTID = id
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public (bool success, string message) AlumniEventAttachmentFile(
   AlumniEventAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@EVENTID", req.EventID);
                parameters.Add("@EVENTPHOTO", req.Attachment);
                parameters.Add("@EVENTPHOTONAME", req.FileName);
                parameters.Add("@EVENTPHOTOTYPE", req.FileType);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_ALUMNIEVENTS_Upsert_Attachment",
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
