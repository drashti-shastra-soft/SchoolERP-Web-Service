using Dapper;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Data;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using System.ComponentModel.Design;
using System.Data;
using SchoolERPWebService.Domain.Common;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    public class DownloadCenterService: IDownloadCenterService
    {
        private readonly IConfiguration _configuration;
        public DownloadCenterService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves a list of content types filtered by company and optional search term.
        /// </summary>
        /// <param name="companyId">The ID of the company.</param>
        /// <param name="searchTerm">Optional search term to filter content types by name.</param>
        /// <returns>A list of <see cref="ContentTypeViewModel"/> objects.</returns>
        public List<ContentTypeViewModel> GetContentTypeList(int companyId, string? searchTerm)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();
            parameters.Add("@Action", "LIST");
            parameters.Add("@CompanyID", companyId);
            parameters.Add("@SearchTerm", searchTerm);   // Dapper handles null automatically

            return conn
                .Query<ContentTypeViewModel>(
                    "sp_Download_ContentType_CRUD",
                    parameters,
                    commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public async Task<PagedResult<ContentTypeViewModel>> GetAllContentTypeWithPage(SearchRequest req)
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
                param.Add("@SEARCHTERM", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<ContentTypeViewModel>(
                "SP_DOWNLOAD_CONTENTTYPE_PAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<ContentTypeViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ContentTypeViewModel>
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

        public async Task<(bool Success, string Message)> CopyContentType(CopyForCompanyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@SOURCECOMPANYID", req.FromCompanyId);
                parameters.Add("@TARGETCOMPANYID", req.ToCompanyId);
                parameters.Add("@USERID", req.UserID);
                parameters.Add("@IPADDRESS", req.IPAddress);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_DOWNLOAD_CONTENTTYPE_COMPANYWISE_COPY",
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

        /// <summary>
        /// Retrieves a content type record by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the content type.</param>
        /// <returns>A <see cref="ContentTypeViewModel"/> if found; otherwise, <c>null</c>.</returns>
        public ContentTypeViewModel GetContentTypeById(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<ContentTypeViewModel>(
                "sp_Download_ContentType_CRUD",
                new { Action = "GETBYID", ContentTypeID = id },
                commandType: CommandType.StoredProcedure
            );
        }
        /// <summary>
        /// Creates or updates a content type.
        /// Calls sp_Download_ContentType_CRUD with SAVE action and returns
        /// the operation status and message from the stored procedure.
        /// </summary>
        /// <param name="req">Content type details.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="userId">Current user identifier.</param>
        /// <returns>
        /// Tuple containing:
        /// Success - Indicates whether the operation succeeded.
        /// Message - Result message returned from the stored procedure.
        /// </returns>
        public (bool Success, string Message) UpsertContentType(
            ContentTypeUpsertRequest req,
            int companyId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Download_ContentType_CRUD",
                    new
                    {
                        Action = "SAVE",
                        ContentTypeID = req.ContentTypeID,
                        TypeName = req.TypeName,
                        Description = req.Description,
                        DisplayLabel= req.DisplayLabel,
                        CompanyID = companyId,
                        UserID = userId
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
        /// Deletes a content type by ContentTypeID.
        /// Calls sp_Download_ContentType_CRUD with DELETE action.
        /// </summary>
        /// <param name="id">Content Type ID.</param>
        /// <param name="userId">User ID performing the delete operation.</param>
        /// <returns>
        /// Tuple containing:
        /// Success = true if deletion succeeded.
        /// Message = database response message.
        /// </returns>
        public (bool Success, string Message) DeleteContentType(List<int> id,int companyId, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string contentTypeIDs = string.Join(",", id);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_DOWNLOAD_CONTENTTYPE_DELETE_MULTI",
                    new
                    {
                        CONTENTTYPEID = contentTypeIDs,
                        COMPANYID = companyId,
                        UserID = userId
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
        /// Toggles the active/inactive status of a content type.
        /// Calls sp_Download_ContentType_CRUD with Action = TOGGLESTATUS
        /// and returns the operation result message.
        /// </summary>
        /// <param name="id">Content Type ID.</param>
        /// <param name="userId">User performing the action.</param>
        /// <param name="companyId">Company ID.</param>
        /// <returns>
        /// Tuple containing:
        /// Success - Indicates whether the operation succeeded.
        /// Message - Database response message.
        /// </returns>
        public (bool Success, string Message) ToggleContentTypeStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_DOWNLOAD_CONTENTTYPE_TOGGLESTATUS_MULTI",
                    new
                    {
                        CONTENTTYPEID = request.Ids,
                        USERID = request.DoneBy,
                        COMPANYID = request.CompanyId
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
        // Video Tutorials
        /// <summary>
        /// Retrieves the list of video tutorials based on company, class, section, and search criteria.
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="classId">Optional Class ID.</param>
        /// <param name="sectionId">Optional Section ID.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <returns>List of video tutorials.</returns>
        public List<VideoTutorialViewModel> GetVideoTutorialList(int companyId, int? classId, int? sectionId, string? searchTerm)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<VideoTutorialViewModel>(
                "sp_Download_VideoTutorial_CRUD",
                new
                {
                    Action = "LIST",
                    CompanyID = companyId,
                    ClassID = classId,
                    SectionID = sectionId,
                    SearchTerm = searchTerm
                },
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public async Task<PagedResult<VideoTutorialViewModel>> GetAllVideoTutoriaWithPage(SearchRequest req)
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
                param.Add("@CLASSID", req.ClassID);
                param.Add("@SECTIONID", req.SectionID);
                param.Add("@SEARCHTERM", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<VideoTutorialViewModel>(
                "SP_DOWNLOAD_VIDEOTUTORIAL_LIST_PAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<VideoTutorialViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<VideoTutorialViewModel>
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
        /// Retrieves a video tutorial by its unique ID.
        /// </summary>
        /// <param name="id">Video Tutorial ID.</param>
        /// <returns>
        /// Returns a <see cref="VideoTutorialViewModel"/> if found; otherwise null.
        /// </returns>
        public VideoTutorialViewModel GetVideoTutorialById(int id)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.QueryFirstOrDefault<VideoTutorialViewModel>(
                "sp_Download_VideoTutorial_CRUD",
                new
                {
                    Action = "GETBYID",
                    VideoID = id
                },
                commandType: CommandType.StoredProcedure);
        }
        /// <summary>
        /// Inserts or updates a video tutorial record.
        /// Calls sp_Download_VideoTutorial_CRUD with Action='SAVE'
        /// and returns the operation status and message.
        /// </summary>
        /// <param name="req">Video tutorial details.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="userId">Logged-in user identifier.</param>
        /// <returns>
        /// Tuple containing:
        /// Success - Indicates whether the operation succeeded.
        /// Message - Database response message.
        /// </returns>
        public (bool Success, string Message) UpsertVideoTutorial(
            VideoTutorialUpsertRequest req,
            int companyId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Download_VideoTutorial_CRUD",
                    new
                    {
                        Action = "SAVE",
                        VideoID = req.VideoID,
                        req.Title,
                        req.VideoLink,
                        req.Description,
                        req.ClassID,
                        req.SectionID,
                        CompanyID = companyId,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Deletes a video tutorial by Video ID.
        /// Calls sp_Download_VideoTutorial_CRUD with DELETE action
        /// and returns the operation status and message.
        /// </summary>
        /// <param name="id">Video tutorial ID.</param>
        /// <param name="userId">User performing the delete operation.</param>
        /// <returns>
        /// Tuple containing:
        /// Success - Indicates whether the delete operation succeeded.
        /// Message - Database response message.
        /// </returns>
        public (bool Success, string Message) DeleteVideoTutorial(List<int> id, int userId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string videoID = string.Join(",", id);

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "SP_DOWNLOAD_VIDEOTUTORIAL_MULTIDELETE",
                    new
                    {                        
                        VIDEOIDS = videoID,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        /// <summary>
        /// Toggles the active/inactive status of a video tutorial.
        /// Executes the TOGGLESTATUS action in sp_Download_VideoTutorial_CRUD
        /// and returns the operation result along with a message.
        /// </summary>
        /// <param name="id">Video tutorial ID.</param>
        /// <param name="userId">User performing the action.</param>
        /// <param name="companyId">Company ID.</param>
        /// <returns>
        /// Tuple containing:
        /// - Success: Indicates whether the operation succeeded.
        /// - Message: Database response message.
        /// </returns>
        public (bool Success, string Message) ToggleVideoTutorialStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_DOWNLOAD_VIDEOTUTORIAL_TOGGLESTATUS",
                    new
                    {
                        VIDEOIDS = request.Ids,
                        ISACTIVE = request.IsActive,
                        UserID = request.DoneBy
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
        // Upload Content
        /// <summary>
        /// Retrieves the content list based on company and search term.
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <param name="searchTerm">Optional search term.</param>
        /// <returns>List of content records.</returns>
        public List<ContentViewModel> GetContentList(int companyId, string? searchTerm)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<ContentViewModel>(
                "sp_Download_Content_CRUD",
                new
                {
                    Action = "LIST",
                    CompanyID = companyId,
                    SearchTerm = searchTerm
                },
                commandType: CommandType.StoredProcedure
            ).ToList();
        }
        public async Task<PagedResult<ContentViewModel>> GetAllContentWithPage(SearchRequest req)
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
                param.Add("@CONTENTTYPEID", req.ContenttypeId);
                param.Add("@SEARCHTERM", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<ContentViewModel>(
                "SP_DOWNLOAD_CONTENT_LISTWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<ContentViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<ContentViewModel>
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
        /// Saves or updates download center content details.
        /// Calls sp_Download_Content_CRUD with Action='SAVE'
        /// and returns operation status with message.
        /// </summary>
        /// <param name="content">Content details to save.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="userId">Logged-in user identifier.</param>
        /// <returns>
        /// Tuple containing:
        /// Success - Indicates whether operation succeeded.
        /// Message - Database response message.
        /// </returns>
        public (bool Success, string Message) SaveContent(ContentViewModel content, int companyId, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Download_Content_CRUD",
                    new
                    {
                        Action = "SAVE",
                        Title = content.Title,
                        ContentTypeID = 0,
                        FileType = content.FileType,
                        FileName = content.FileName,
                        FilePath = content.FilePath,
                        FileSize = content.FileSize,
                        CompanyID = companyId,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        // Overload or refined version for save
        /// <summary>
        /// Saves uploaded content details such as title, file information, and content type
        /// by executing the Download Content CRUD stored procedure.
        /// </summary>
        /// <param name="title">Content title.</param>
        /// <param name="typeId">Content type identifier.</param>
        /// <param name="fileType">File extension/type.</param>
        /// <param name="fileName">Uploaded file name.</param>
        /// <param name="filePath">Physical or virtual file path.</param>
        /// <param name="fileSize">File size.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <param name="userId">User identifier.</param>
        /// <returns>
        /// Returns a tuple containing:
        /// Success - Indicates whether the operation succeeded.
        /// Message - Result message returned from the stored procedure.
        /// </returns>
        public (bool Success, string Message) SaveUploadContent(
            string title,
            int typeId,
            string? fileType,
            string fileName,
            string? filePath,
            string? fileSize,
            int companyId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Download_Content_CRUD",
                    new
                    {
                        Action = "SAVE",
                        Title = title,
                        ContentTypeID = typeId,
                        FileType = fileType,
                        FileName = fileName,
                        FilePath = filePath,
                        FileSize = fileSize,
                        CompanyID = companyId,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message ?? string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes a content record by Content ID.
        /// </summary>
        /// <param name="id">Content ID.</param>
        /// <param name="userId">User performing the delete operation.</param>
        /// <returns>
        /// Returns a tuple containing:
        /// Success - Indicates whether the operation was successful.
        /// Message - Database response message.
        /// </returns>
        public (bool Success, string Message) DeleteContent(int id, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "sp_Download_Content_CRUD",
                    new
                    {
                        Action = "DELETE",
                        ContentID = id,
                        UserID = userId
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        // Shared Links
        /// <summary>
        /// Generates a shared download link and returns the generated token.
        /// </summary>
        /// <param name="req">Shared link request details.</param>
        /// <param name="companyId">Company ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>
        /// Success - Indicates whether the operation succeeded.
        /// Message - Status message returned from the database.
        /// Token - Generated share token.
        /// </returns>
        public (bool Success, string Message, string Token) GenerateSharedLink(
            SharedLinkUpsertRequest req,
            int companyId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Download_ShareLink_CRUD",
                    new
                    {
                        Action = "SAVE",
                        Title = req.Title,
                        ShareDate = req.ShareDate,
                        ValidUpto = req.ValidUpto,
                        ContentIDs = string.Join(",", req.ContentIDs!),
                        CompanyID = companyId,
                        UserID = userId,
                        SendToType = req.SendToType
                    },
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response received from database.", "");
                //return (
                //    result?.Result == 1,
                //    result?.Message ?? "Operation completed."
                //);
                bool success = Convert.ToInt32(result?.Result) == 1;
                string message = Convert.ToString(result.Message) ?? string.Empty;
                string token = success
                    ? Convert.ToString(result.Extra) ?? string.Empty
                    : string.Empty;

                return (success, message, token);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, "");
            }
        }

        /// <summary>
        /// Retrieves all shared download links for the specified company.
        /// </summary>
        /// <param name="companyId">Company ID.</param>
        /// <returns>List of shared links.</returns>
        public List<SharedLinkViewModel> GetSharedLinkList(int companyId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return conn.Query<SharedLinkViewModel>(
                "sp_Download_ShareLink_CRUD",
                new
                {
                    Action = "LIST",
                    CompanyID = companyId
                },
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public async Task<PagedResult<SharedLinkViewModel>> GetAllSharedLinkWithPage(SearchRequest req)
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
                param.Add("@SEARCHTERM", req.SearchKeyword);
                param.Add("@PAGEINDEX", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);

                var result = (await conn.QueryAsync<SharedLinkViewModel>(
                "SP_DOWNLOAD_SHARELINK_LISTPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<SharedLinkViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<SharedLinkViewModel>
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
        /// Deletes a shared link.
        /// </summary>
        /// <param name="id">Shared Link ID.</param>
        /// <returns>Success status and message.</returns>
        public (bool Success, string Message) DeleteSharedLink(List<int> id, int companyId)
        {
            try
            {
                if (id == null || !id.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string sharedLinkIDs = string.Join(",", id);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Download_ShareLink_CRUD",
                    new
                    {
                        Action = "DELETE",
                        SharedLinkID = sharedLinkIDs,
                        COMPANYID= companyId
                    },
                    commandType: CommandType.StoredProcedure);
                var dict = (IDictionary<string, object>)result;

                bool success = Convert.ToInt32(dict["RESULT"]) == 1;
                string message = Convert.ToString(dict["MESSAGE"]) ?? string.Empty;

                return (success, message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<GenerateSharedLinkResult> ExecuteGenerateSharedLinkSpAsync(
    GenerateSharedLinkRequest request, int companyId, int userId, string userName)
        {
            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("@Title", request.Title, DbType.String, size: 200);
                parameters.Add("@ShareDate", request.ShareDate, DbType.DateTime);
                parameters.Add("@ValidUpto", request.ValidUpto, DbType.DateTime);
                parameters.Add("@ContentIDsJson", JsonSerializer.Serialize(request.ContentIDs), DbType.String);
                parameters.Add("@SendToType", request.SendToType.ToUpperInvariant(), DbType.String, size: 20);

                string roleIdsJson = null;
                if (request.RoleIds != null && request.RoleIds.Count > 0)
                {
                    roleIdsJson = JsonSerializer.Serialize(request.RoleIds);
                }
                parameters.Add("@RoleIdsJson", roleIdsJson, DbType.String);

                string classSectionJson = null;
                if (request.ClassSections != null && request.ClassSections.Count > 0)
                {
                    classSectionJson = JsonSerializer.Serialize(request.ClassSections);
                }
                parameters.Add("@ClassSectionJson", classSectionJson, DbType.String);

                string individualJson = null;
                if (request.Individuals != null && request.Individuals.Count > 0)
                {
                    individualJson = JsonSerializer.Serialize(request.Individuals);
                }
                parameters.Add("@IndividualJson", individualJson, DbType.String);

                parameters.Add("@CompanyID", companyId, DbType.Int32);
                parameters.Add("@UserID", userId, DbType.Int32);
                parameters.Add("@IPAddress", (object)null, DbType.String, size: 50); // pass real IP if you have it

                parameters.Add("@SharedLinkID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@Token", dbType: DbType.Guid, direction: ParameterDirection.Output);   // Guid, no size

                using var connection = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var row = await connection.QueryFirstOrDefaultAsync<SpSharedLinkResultRow>(
                    "dbo.usp_Download_SharedLink_Create",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                var result = new GenerateSharedLinkResult();
                result.Success = row != null ? row.Success : false;
                result.SharedLinkID = parameters.Get<int?>("@SharedLinkID");
                result.Token = parameters.Get<Guid?>("@Token");   // Guid -> string, only here
                result.Message = row != null ? row.Message : null;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
    

