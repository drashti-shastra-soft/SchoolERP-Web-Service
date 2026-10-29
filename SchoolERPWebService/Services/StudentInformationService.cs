using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Data;
using System.Text.Json;

namespace SchoolERPWebService.Services
{
    /// <summary>
    /// Service responsible for managing student information
    /// operations such as student profile management,
    /// academic details, and related student data processing.
    /// </summary>
    public class StudentInformationService : IStudentInformationService
    {
        private readonly IConfiguration _configuration;
        private readonly IFieldService _fieldService;
        public StudentInformationService(IConfiguration configuration, IFieldService fieldService)
        {
            _configuration = configuration;
            _fieldService = fieldService;
        }

        /// <summary>
        /// Retrieves all active and inactive student disable reasons
        /// for the specified company and session.
        /// These reasons are typically used when disabling a student
        /// profile (e.g., Graduated, Transfer, Left School).
        /// </summary>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <returns>
        /// Returns a list of student disable reasons.
        /// </returns>
        public List<StudentDisableReasonViewModel> GetAllDisableReasons(
            int companyId,
            int sessionId, bool? IncludeIsActive = null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@INCLUDEDELETED", IncludeIsActive);

                return conn.Query<StudentDisableReasonViewModel>(
                    "sp_Student_DisableReason_GetAll",
                    param,
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<StudentDisableReasonViewModel>();
            }
        }

        public async Task<PagedResult<StudentDisableReasonViewModel>> GetAllDisableReasonsWithPage(StudentCategorySearch req)
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

                var result = (await conn.QueryAsync<StudentDisableReasonViewModel>(
                "SP_STUDENT_DISABLEREASON_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<StudentDisableReasonViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentDisableReasonViewModel>
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
        /// Creates a new student disable reason or updates an existing one.
        /// </summary>
        /// <param name="req">
        /// Contains disable reason information to be saved.
        /// </param>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) UpsertDisableReason(
            StudentDisableReasonUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@DisableReasonID", req.DisableReasonID);
                param.Add("@SessionID", sessionId);
                param.Add("@CompanyID", companyId);
                param.Add("@DisableReasonTitle", req.DisableReasonTitle);
                param.Add("@DisplayLabel", req.DisplayLabel);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Student_DisableReason_Upsert",
                    param,
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

        /// <summary>
        /// Deletes the specified student disable reason.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the disable reason.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteDisableReason(
            List<int> ids,
            int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", ids);

                var param = new DynamicParameters();

                param.Add("@DisableReasonID", id);
                param.Add("@UserID", userId);

                //var result = conn.QueryFirstOrDefault<dynamic>(
                //    "sp_Student_DisableReason_Delete",
                //    param,
                //    commandType: CommandType.StoredProcedure);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Student_DisableReason_Delete",
                    param,
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

        /// <summary>
        /// Retrieves all student houses configured for the specified
        /// company and session.
        /// Examples: Red House, Blue House, Green House.
        /// </summary>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <returns>
        /// Returns a list of student houses.
        /// </returns>
        public List<StudentHouseViewModel> GetAllStudentHouses(
            int companyId,
            int sessionId,bool? IncludeIsActive=null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@IncludeIsActive", IncludeIsActive);

                return conn.Query<StudentHouseViewModel>(
                    "sp_MST_StudentHouse_GetAll",
                    param,
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<StudentHouseViewModel>();
            }
        }

        /// <summary>
        /// Creates a new student house or updates an existing student house
        /// for the specified company and session.
        /// Examples: Red House, Blue House, Green House.
        /// </summary>
        /// <param name="req">
        /// Contains student house information to be saved.
        /// </param>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) UpsertStudentHouse(
            StudentHouseUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@StudentHouseID", req.StudentHouseID);
                param.Add("@SessionID", sessionId);
                param.Add("@CompanyID", companyId);
                param.Add("@StudentHouseName", req.StudentHouseName);
                param.Add("@StudentHouseDescription", req.StudentHouseDescription);
                param.Add("@DisplayLabel", req.DisplayLabel);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_MST_StudentHouse_Upsert",
                    param,
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

        /// <summary>
        /// Deletes the specified student house from the system.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the student house.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteStudentHouse(
            List<int> ids,
            int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", ids);
                var param = new DynamicParameters();

                param.Add("@StudentHouseID", id);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_MST_StudentHouse_Delete",
                    param,
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


        /// <summary>
        /// Retrieves all student categories configured for the specified
        /// company and session.
        /// Examples: General, OBC, SC, ST, EWS.
        /// </summary>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <returns>
        /// Returns a list of student categories.
        /// </returns>
        public List<StudentCategoryViewModel>  GetAllStudentCategories(
            int companyId,
            int sessionId,bool? includeDeleted=null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@IncludeIsActive", includeDeleted);
                return conn.Query<StudentCategoryViewModel>(
                    "sp_MST_StudentCategory_GetAll",
                    param,
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<StudentCategoryViewModel>();
            }
        }

        public async Task<PagedResult<StudentCategoryViewModel>> GetAllStudentCategoriesWithPage(StudentCategorySearch req)
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

                var result = (await conn.QueryAsync<StudentCategoryViewModel>(
                "SP_MST_STUDENTCATEGORY_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();


                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;

                var userModel = new PagedResult<StudentCategoryViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };

                if (res == 0)
                {
                    userModel = new PagedResult<StudentCategoryViewModel>
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
        /// Creates a new student category or updates an existing category.
        /// Examples: General, OBC, SC, ST.
        /// </summary>
        /// <param name="req">
        /// Contains student category information to be saved.
        /// </param>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) UpsertStudentCategory(
            StudentCategoryUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@StudentCategoryID", req.StudentCategoryID);
                param.Add("@SessionID", sessionId);
                param.Add("@CompanyID", companyId);
                param.Add("@StudentCategoryName", req.StudentCategoryName);
                param.Add("@DisplayLabel", req.DisplayLabel);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_MST_StudentCategory_Upsert",
                    param,
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

        /// <summary>
        /// Deletes the specified student category from the system.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the student category.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteStudentCategory(
            List<int> ids,
            int userId)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string id = string.Join(",", ids);

                var param = new DynamicParameters();

                param.Add("@StudentCategoryID", id);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_MST_StudentCategory_Delete",
                    param,
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

        /// <summary>
        /// Generates the next student roll number based on the
        /// configured auto-generation rules and dynamic field values.
        /// If generation fails, a fallback roll number is returned.
        /// </summary>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The session identifier.
        /// </param>
        /// <param name="dynamicValues">
        /// Optional dynamic field values used in roll number generation.
        /// </param>
        /// <returns>
        /// Returns the next available student roll number.
        /// </returns>
        public string GetNewStudentRollNo(
            int companyId,
            int sessionId,
            Dictionary<string, string>? dynamicValues = null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string? jsonValues = dynamicValues != null
                    ? JsonSerializer.Serialize(dynamicValues)
                    : null;

                var param = new DynamicParameters();

                param.Add("@EntityType", "Student");
                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@FieldValues", jsonValues);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Settings_IDAutoGen_GetNext",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result?.NextID?.ToString()
                    ?? $"STU{DateTime.Now.Ticks.ToString().Substring(10)}";
            }
            catch
            {
                return $"STU{DateTime.Now.Ticks.ToString().Substring(10)}";
            }
        }

        /// <summary>
        /// Retrieves the next available admission number for a student
        /// based on the last admission number used in the company.
        /// </summary>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <returns>
        /// Returns the next admission number.
        /// </returns>
        public string GetNextSimpleAdmissionNo(int companyId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@CompanyID", companyId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Student_GetNextAdmissionNo",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result?.NEXTADMISSIONNO?.ToString() ?? "1";
            }
            catch
            {
                return "1";
            }
        }

        /// <summary>
        /// Generates a random password using alphanumeric characters
        /// excluding visually similar characters for better readability.
        /// </summary>
        /// <param name="length">
        /// The desired password length. Default is 6 characters.
        /// </param>
        /// <returns>
        /// Returns a randomly generated password.
        /// </returns>
        private string GenerateRandomPassword(int length = 6)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";

            var random = new Random();

            return new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
        }

        /// <summary>
        /// This is a complex method that handles the entire student admission process.
        /// It saves basic info, parent details, and any custom fields.
        /// It also automatically creates usernames and passwords for the student and parent.
        /// </summary>
        public (bool Success, string Message, int StudentID) UpsertStudentAdmission(
            StudentAdmissionUpsertRequest req, int companyId, int sessionId, int userId)
        {
            try
            {
                // Fetch ALL fields for this company/session to ensure we find Misc fields too
                var allFields = _fieldService.GetAllFields(companyId, sessionId, belongsTo: null);

                // Auto-generate credentials
                string rollNo = req.RollNo;
                if (string.IsNullOrEmpty(rollNo) && req.FieldValues != null)
                    rollNo = req.FieldValues.ContainsKey("Roll No") ? req.FieldValues["Roll No"] : "";

                if (!string.IsNullOrEmpty(rollNo) && req.FieldValues != null)
                {
                    if (!req.FieldValues.ContainsKey("Student Username") || string.IsNullOrEmpty(req.FieldValues["Student Username"]))
                        req.FieldValues["Student Username"] = rollNo;
                    if (!req.FieldValues.ContainsKey("Student Password") || string.IsNullOrEmpty(req.FieldValues["Student Password"]))
                        req.FieldValues["Student Password"] = GenerateRandomPassword(6);
                    if (!req.FieldValues.ContainsKey("Parent Username") || string.IsNullOrEmpty(req.FieldValues["Parent Username"]))
                        req.FieldValues["Parent Username"] = "P_" + rollNo;
                    if (!req.FieldValues.ContainsKey("Parent Password") || string.IsNullOrEmpty(req.FieldValues["Parent Password"]))
                        req.FieldValues["Parent Password"] = GenerateRandomPassword(6);
                }

                // Auto-generate Admission No for new admissions if empty
                if (req.StudentID == 0)
                {
                    var admissionKey = req.FieldValues.Keys.FirstOrDefault(k =>
                        string.Equals(k, "Admission No", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(k, "AdmissionNo", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(k, "Admission Number", StringComparison.OrdinalIgnoreCase));

                    if (admissionKey == null)
                        req.FieldValues["Admission No"] = GetNextSimpleAdmissionNo(companyId);
                    else if (string.IsNullOrEmpty(req.FieldValues[admissionKey]))
                        req.FieldValues[admissionKey] = GetNextSimpleAdmissionNo(companyId);
                }
                
                    // Field-to-SP-parameter mapping
                    var mapping = new Dictionary<string, (string ParamName, string Type)>(StringComparer.OrdinalIgnoreCase) {

                    { "Roll No", ("@ROLLNO", "string") },
                    { "Admission No", ("@ADMISSIONNO", "string") },

                    { "Admission Number", ("@ADMISSIONNO", "string") },
                    { "Admission Date", ("@ADMISSIONDATE", "date") },
                    { "Class", ("@CLASSID", "int") },
                    { "Section", ("@SECTIONID", "int") },
                    { "First Name", ("@FIRSTNAME", "string") },
                    { "Middle Name", ("@MIDDLENAME", "string") },
                    { "Last Name", ("@LASTNAME", "string") },
                    { "Gender", ("@GENDER", "string") },
                    { "Date of Birth", ("@DOB", "date") },
                    { "Category", ("@CATEGORYID", "int") },
                    { "Religion", ("@RELIGION", "string") },
                    { "Caste", ("@CASTE", "string") },

                    { "Mobile Number", ("@MOBILENO", "string") },
                    { "Email", ("@EMAIL", "string") },
                    { "Blood Group", ("@BLOODGROUP", "string") },
                    { "House", ("@HOUSEID", "int") },
                    { "Height", ("@HEIGHT", "string") },
                    { "Weight", ("@WEIGHT", "string") },
                    { "Route List", ("@VEHICLEID", "int") },
                    { "TransportRouteID", ("@ROUTEID", "int") },
                    { "Pickup Point", ("@PICKUPPOINTID", "int") },
                    { "Fees Month", ("@TRANSPORTMONTH", "string") },
                    { "Hostel", ("@HOSTELID", "int") },
                    { "Room No", ("@ROOMID", "int") },
                    { "Father Name", ("@FATHERNAME", "string") },
                    { "Father Phone", ("@FATHERPHONE", "string") },
                    { "Father Mobile", ("@FATHERPHONE", "string") },
                    { "Father Occupation", ("@FATHEROCCUPATION", "string") },
                    { "Mother Name", ("@MOTHERNAME", "string") },
                    { "Mother Phone", ("@MOTHERPHONE", "string") },
                    { "Mother Mobile", ("@MOTHERPHONE", "string") },
                    { "Mother Occupation", ("@MOTHEROCCUPATION", "string") },
                    { "If Guardian Is", ("@IFGUARDIANIS", "string") },
                    { "Guardian Name", ("@GUARDIANNAME", "string") },
                    { "Guardian Phone", ("@GUARDIANPHONE", "string") },
                    { "Guardian Mobile", ("@GUARDIANPHONE", "string") },
                    { "Guardian Occupation", ("@GUARDIANOCCUPATION", "string") },

                    { "Guardian Relation", ("@GUARDIANRELATION", "string") },

                    { "Guardian Email", ("@GUARDIANEMAIL", "string") },
                    { "CurrentAddress", ("@CURRENTADDRESS", "string") },
                    { "CurrentPinCode",     ("@CURRENTPOSTALCODE", "string") },
                    { "CurrentCountryId",   ("@CURRENTCOUNTRYID", "int") },
                    { "CurrentStateId",     ("@CURRENTSTATEID", "int") },
                    { "CurrentCityId",      ("@CURRENTCITYID", "int") },
                    { "PermanentAddress", ("@PERMANENTADDRESS", "string") },
                    { "PermanentPinCode",   ("@PERMANENTPOSTALCODE", "string") },
                    { "PermanentCountryId", ("@PERMANENTCOUNTRYID", "int") },
                    { "PermanentStateId",   ("@PERMANENTSTATEID", "int") },
                    { "PermanentCityId",    ("@PERMANENTCITYID", "int") },
                    { "Student Username", ("@STUDENTUSERNAME", "string") },
                    { "Student Password", ("@STUDENTPASSWORD", "string") },
                    { "Parent Username", ("@PARENTUSERNAME", "string") },
                    { "Parent Password", ("@PARENTPASSWORD", "string") },
                    { "Student Photo", ("@STUDENTPHOTO", "string") },
                    { "Student Photo Name", ("@STUDENTPHOTONAME", "string") },
                    { "Student Photo Type", ("@STUDENTPHOTOTYPE", "string") },
                    { "Father Photo", ("@FATHERPHOTO", "string") },
                    { "Father Photo Name", ("@FATHERPHOTONAME", "string") },
                    { "Father Photo Type", ("@FATHERPHOTOTYPE", "string") },
                    { "Mother Photo", ("@MOTHERPHOTO", "string") },
                    { "Mother Photo Name", ("@MOTHERPHOTONAME", "string") },
                    { "Mother Photo Type", ("@MOTHERPHOTOTYPE", "string") },
                    { "Guardian Photo", ("@GUARDIANPHOTO", "string") },
                    { "Guardian Photo Name", ("@GUARDIANPHOTONAME", "string") },
                    { "Guardian Photo Type", ("@GUARDIANPHOTOTYPE", "string") },

                };

                // Build the DynamicParameters object — start with the fixed SP params
                var dp = new DynamicParameters();
                dp.Add("STUDENTID", req.StudentID);
                dp.Add("ParentUserID", req.ParentUserID);
                dp.Add("COMPANYID", companyId);
                dp.Add("SESSIONID", sessionId);
                dp.Add("FEETEMPLATEIDS", req.FeeTemplateId);
                dp.Add("USERID", userId);

                // Initialise every mapped target param with null so the SP always sees them
                foreach (var item in mapping.Values.Distinct())
                {
                    DbType? dbType = item.Type switch
                    {
                        "int" => DbType.Int32,
                        "date" => DbType.DateTime,
                        "datetime" => DbType.DateTime,
                        "byte[]" => DbType.Binary,
                        _ => DbType.String
                    };

                    dp.Add(item.ParamName, null, dbType);
                }

                // Custom fields — collected into a DataTable for TVP
                var customFieldsDt = new DataTable();
                customFieldsDt.Columns.Add("FIELDID", typeof(int));
                customFieldsDt.Columns.Add("FIELDVALUE", typeof(string));

                // Process incoming FieldValues
                if (req.FieldValues != null)
                {
                    foreach (var field in req.FieldValues)
                    {
                        string trimmedKey = field.Key?.Trim() ?? "";

                        if (mapping.ContainsKey(trimmedKey))
                        {
                            var map = mapping[trimmedKey];
                            try
                            {
                                object val;
                                if (string.IsNullOrEmpty(field.Value))
                                {
                                    val = null;
                                }
                                else if (map.Type == "int")
                                {
                                    val = int.Parse(field.Value);
                                }
                                else if (map.Type == "date" || map.Type == "datetime")
                                {
                                    val = DateTime.TryParse(field.Value, out var dtVal) ? (object)dtVal : null;
                                }
                                else if (map.Type == "byte[]")
                                {
                                    try { val = Convert.FromBase64String(field.Value); }
                                    catch { val = null; }
                                }
                                else
                                {
                                    val = field.Value;
                                }

                                // Overwrite the pre-initialised null with the real value.
                                // Dapper's Add with the same name replaces the earlier entry.
                                DbType dbType = map.Type switch
                                {
                                    "int" => DbType.Int32,
                                    "date" or "datetime" => DbType.DateTime,
                                    "byte[]" => DbType.Binary,
                                    _ => DbType.String,
                                };
                                dp.Add(map.ParamName, val, dbType);

                                // Keep rollNo in sync (mirrors original behaviour)
                                if (string.Equals(map.ParamName, "ROLLNO", StringComparison.OrdinalIgnoreCase)
                                    && string.IsNullOrEmpty(rollNo))
                                    rollNo = field.Value;
                            }
                            catch { /* Keep null — mirrors original "Keep DBNull" */ }
                        }
                        else
                        {
                            // Case-insensitive lookup for custom / Misc fields
                            var fieldDef = allFields.FirstOrDefault(f =>
                                string.Equals(f.FieldName?.Trim(), field.Key?.Trim(), StringComparison.OrdinalIgnoreCase));

                            if (fieldDef != null)
                                customFieldsDt.Rows.Add(fieldDef.FieldId, field.Value ?? "");
                        }
                    }
                }

                //// Documents (DocumentTitle_X / DocumentFile_X) — TVP
                //var documentsDt = new DataTable();
                //documentsDt.Columns.Add("DocumentTitle", typeof(string));
                //documentsDt.Columns.Add("DocumentContent", typeof(byte[]));
                //documentsDt.Columns.Add("DocumentPath", typeof(string));

                //for (int i = 1; i <= 10; i++)
                //{
                //    string titleKey = $"DocumentTitle_{i}";
                //    string fileKey = $"DocumentFile_{i}";
                //    string nameKey = $"DocumentFile_{i} Name";

                //    if (req.FieldValues.ContainsKey(titleKey) || req.FieldValues.ContainsKey(fileKey))
                //    {
                //        string title = req.FieldValues.GetValueOrDefault(titleKey) ?? "";
                //        string base64 = req.FieldValues.GetValueOrDefault(fileKey) ?? "";
                //        string fileName = req.FieldValues.GetValueOrDefault(nameKey) ?? "";

                //        if (!string.IsNullOrEmpty(base64))
                //        {
                //            try
                //            {
                //                byte[] content = Convert.FromBase64String(base64);
                //                documentsDt.Rows.Add(title, content, fileName);
                //            }
                //            catch { /* Skip invalid base64 */ }
                //        }
                //    }
                //}

                // Add TVP parameters — Dapper handles DataTable → SqlDbType.Structured automatically
                // when you pass an AsTableValuedParameter() wrapper.
                //dp.Add("CUSTOMFIELDS", customFieldsDt.AsTableValuedParameter("TYPE_STUDENTCUSTOMFIELDS"));
                //dp.Add("DOCUMENTS", documentsDt.AsTableValuedParameter("TYPE_STUDENTDOCUMENTS"));


               

               
                // Add TVP parameters — this is the part that was missing
                dp.Add("CUSTOMFIELDS", customFieldsDt.AsTableValuedParameter("TYPE_STUDENTCUSTOMFIELDS"));
                //dp.Add("DOCUMENTS", documentsDt.AsTableValuedParameter("TYPE_STUDENTDOCUMENTS"));

                // Execute the stored procedure
                using var conn = new SqlConnection(
                  _configuration.GetConnectionString("DefaultConnection")); // however you expose IDbConnection
                var result = conn.QueryFirst<SpResult>(
                    "SP_STUDENT_ADMISSION_UPSERT",
                    dp,
                    commandType: CommandType.StoredProcedure
                );

                return (result.Result > 0, result.Message, result.Result)    ;
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0);
            }
        }

        /// <summary>
        /// Retrieves complete student details from the database using a stored procedure
        /// that returns multiple result sets.
        /// 
        /// Result sets expected from SP_STUDENT_DETAILS_GET:
        ///   Table 0 — Basic info (personal, parent, guardian, credentials, status)
        ///   Table 1 — Addresses (current / permanent)
        ///   Table 2 — Transport details (route, vehicle, pickup point)
        ///   Table 3 — Hostel details (hostel, room)
        ///   Table 4 — Custom field values
        ///   Table 5 — Uploaded documents
        /// </summary>
        public StudentDetailsViewModel GetStudentDetails(int studentId, int companyId, int sessionId)
        {
            var model = new StudentDetailsViewModel();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var dp = new DynamicParameters();
                dp.Add("STUDENTID", studentId);
                dp.Add("COMPANYID", companyId);
                dp.Add("SESSIONID", sessionId);

                using var multi = conn.QueryMultiple(
                    "SP_STUDENT_DETAILS_GET",
                    dp,
                    commandType: CommandType.StoredProcedure
                );

                model.BasicInfo = multi.ReadFirstOrDefault<StudentBasicInfoViewModel>();
                model.Addresses = multi.Read<StudentAddressViewModel>().ToList();
                model.Transport = multi.ReadFirstOrDefault<StudentTransportDetailsViewModel>();
                model.Hostel = multi.ReadFirstOrDefault<StudentHostelDetailsViewModel>();
                model.CustomFields = multi.Read<StudentCustomFieldValueViewModel>().ToList();
                model.Documents = multi.Read<StudentDocumentViewModel>().ToList();
            }
            catch (Exception ex)
            {
                // Consider logging ex here
            }
            return model;
        }

        /// <summary>
        /// Retrieves a filtered list of active students with their custom field values.
        ///
        /// Flow:
        ///   1. Calls SP_STUDENT_LIST_GET with optional class/section/search filters.
        ///   2. Calls sp_StudentCustomFieldValues_GetAllActive once (avoids N+1 queries),
        ///      groups results by StudentID into a lookup dictionary.
        ///   3. Maps custom fields onto each student from the pre-built dictionary.
        ///
        /// Note: Dapper auto-maps SP column names to ViewModel properties —
        ///       no manual column-by-column mapping needed.
        /// </summary>
        public async Task<PagedResult<StudentListViewModel>> GetStudentList(
            int companyId, int sessionId, int? classId, int? sectionId, string? searchTerm, int PageNumber, int PageSize,int? studentId)
        {
            try
            {
                if(PageNumber == 0 && PageSize ==0) 
                {
                    PageNumber = 1;
                    PageSize = 10;
                }
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));


                // ── 1. Student list ─────────────────────────────────────────────────
                //var students = conn.Query<StudentListViewModel>(
                //    "SP_STUDENT_LIST_GET",
                //    new { COMPANYID = companyId, SESSIONID = sessionId, CLASSID = classId, SECTIONID = sectionId, SEARCHTERM = searchTerm },
                //    commandType: CommandType.StoredProcedure
                //).ToList();


                var param = new DynamicParameters();

                param.Add("@COMPANYID", companyId);
                param.Add("@SESSIONID", sessionId);
                param.Add("@CLASSID", classId);
                param.Add("@SECTIONID", sectionId);
                param.Add("@SEARCHTERM", searchTerm);
                param.Add("@PAGENUMBER", PageNumber);
                param.Add("@PAGESIZE", PageSize);
                param.Add("@STUDENTID", studentId);

                using var multi = await conn.QueryMultipleAsync(
                    "SP_STUDENT_LIST_GET",
                    param,
                    commandType: CommandType.StoredProcedure);

                var student = (await multi.ReadAsync<StudentListViewModel>()).ToList();

                int totalRecords = await multi.ReadFirstOrDefaultAsync<int>();

                var students =  new PagedResult<StudentListViewModel>
                {
                    Data = student,
                    TotalRecords = totalRecords,
                    PageNumber = PageNumber,
                    PageSize = PageSize
                };

                // ── 2. All custom field values in one query (prevents N+1) ──────────
                // Grouped into a dictionary: StudentID → list of field values
                var customVals = conn
                    .Query<StudentCustomFieldValueViewModel>(
                        "sp_StudentCustomFieldValues_GetAllActive",
                        commandType: CommandType.StoredProcedure);

                // ── 3. Attach custom fields + filter inactive students ───────────────
                // IsActive is mapped directly by Dapper; no manual column-name guessing needed
                return students;
            }
            catch
            {
                return new PagedResult<StudentListViewModel>();
            }
        }

        public async Task<List<StudentDropDwonBindViewModel>> GetStudentBind(StudentDropDwonBindRequestModel req) 
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@COMPANYID", req.CompanyID);
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@CLASSID", req.ClassID);
                param.Add("@SECTIONID", req.SectionID);
                param.Add("@USERID", req.UserId);

                var result = conn.Query<StudentDropDwonBindViewModel>(
                    "SP_STUDENT_BINDDROPDOWNLIST",
                    param,
                    commandType: CommandType.StoredProcedure).ToList() ;

                return result;
            }
            catch (Exception)
            {
                return new List<StudentDropDwonBindViewModel>();
            }
        }

        /// <summary>
        /// Retrieves all timeline records associated with the specified student.
        /// Timeline records may include activities, notes, events,
        /// documents, and other student-related history entries.
        /// </summary>
        /// <param name="studentId">
        /// The unique identifier of the student.
        /// </param>
        /// <returns>
        /// Returns a list of student timeline records.
        /// </returns>
        public List<StudentTimelineViewModel> GetStudentTimeline(int studentId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                // Load all active student custom field values in a single query.
                // This avoids executing a separate database query for each student
                // and improves performance by preventing the N+1 query problem.
                return conn.Query<StudentTimelineViewModel>(
                    "sp_Student_Timeline_Get",
                    new { StudentID = studentId },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<StudentTimelineViewModel>();
            }
        }
        public StudentTimelineViewModel GetTimelineById(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                // Load all active student custom field values in a single query.
                // This avoids executing a separate database query for each student
                // and improves performance by preventing the N+1 query problem.
                return conn.QueryFirstOrDefault<StudentTimelineViewModel>(
                    "SP_STUDENT_TIMELINE_GETBYID",
                    new { TimelineID = id },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return new StudentTimelineViewModel();
            }
        }
        /// <summary>
        /// Creates a new student timeline record or updates an existing one.
        /// Timeline records can include notes, events, achievements,
        /// activities, and optional document attachments.
        /// </summary>
        /// <param name="req">
        /// Contains timeline details including title, date,
        /// description, and optional document information.
        /// </param>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The academic session identifier.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public SpStudentTimeline UpsertStudentTimeline(
            StudentTimelineUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@TimelineID", req.TimelineID);
                param.Add("@StudentID", req.StudentID);
                param.Add("@Title", req.Title);
                param.Add("@TimelineDate", req.TimelineDate);
                param.Add("@Description", req.Description);
                param.Add("@DocumentContent", req.DocumentBase64);
                param.Add("@DocumentName", req.DocumentName);
                param.Add("@DocumentType", req.DocumentType);
                param.Add("@IsVisibleToStudent", req.IsVisibleToStudent);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpStudentTimeline>(
                    "sp_Student_Timeline_Upsert",
                    param,
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                var model =new SpStudentTimeline();
                model.Result = 0;
                model.Message = ex.Message;
                model.TimelineID = 0;
                return model;
            }
        }

        /// <summary>
        /// Deletes the specified student timeline record.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the timeline record.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteStudentTimeline(
            int id,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@TimelineID", id);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Student_Timeline_Delete",
                    param,
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

        /// <summary>
        /// Retrieves the document attached to a student timeline record.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the timeline record.
        /// </param>
        /// <returns>
        /// Returns the document content, file name, and content type.
        /// </returns>
        public StudentTimelineDocumentModel GetStudentTimelineDocument(int id)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<StudentTimelineDocumentModel>(
                    "sp_Student_Timeline_GetDocument",
                    new { TimelineID = id },
                    commandType: CommandType.StoredProcedure);

                return result;
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Activates or deactivates a student record and optionally
        /// stores disable reason information.
        /// </summary>
        /// <param name="req">
        /// Contains student status information including
        /// active status, disable reason, date, and notes.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) ToggleStudentStatus(
            StudentStatusToggleRequest req,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@StudentID", req.StudentID);
                param.Add("@IsActive", req.IsActive);
                param.Add("@DisableReasonID", req.DisableReasonID);
                param.Add("@DisableDate", req.DisableDate);
                param.Add("@DisableNote", req.DisableNote);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Student_ToggleStatus",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response received from database.");

                return (
                    Convert.ToInt32(result.RESULT) == 1,
                    Convert.ToString(result.MESSAGE) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves students and their associated additional classes
        /// based on the specified search criteria.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="classId">Optional class filter.</param>
        /// <param name="sectionId">Optional section filter.</param>
        /// <param name="searchTerm">Optional search text.</param>
        /// <returns>
        /// Returns a list of students with their additional class assignments.
        /// </returns>
        public List<MultiClassStudentCardViewModel> GetMultiClassStudents(
            int companyId,
            int sessionId,
            int? classId,
            int? sectionId,
            string? searchTerm)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var students = conn.Query<MultiClassStudentCardViewModel>(
                    "sp_Student_MultiClasses_SearchStudents",
                    new
                    {
                        ClassID = classId,
                        SectionID = sectionId,
                        SearchTerm = searchTerm,
                        CompanyID = companyId,
                        SessionID = sessionId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();

                foreach (var student in students)
                {
                    student.AdditionalClasses = conn
                        .Query<StudentMultiClassViewModel>(
                            "sp_Student_MultiClasses_Get",
                            new
                            {
                                StudentID = student.StudentID,
                                CompanyID = companyId,
                                SessionID = sessionId
                            },
                            commandType: CommandType.StoredProcedure)
                        .ToList();
                }

                return students;
            }
            catch
            {
                return new List<MultiClassStudentCardViewModel>();
            }
        }

        /// <summary>
        /// Retrieves a list of disabled students based on
        /// company, session, class, section, and search criteria.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="classId">Optional class filter.</param>
        /// <param name="sectionId">Optional section filter.</param>
        /// <param name="searchTerm">Optional search text.</param>
        /// <returns>
        /// Returns a list of disabled student records.
        /// </returns>
        public List<StudentListViewModel> GetDisabledStudentList(
            int companyId,
            int sessionId,
            int? classId,
            int? sectionId,
            string? searchTerm)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<StudentListViewModel>(
                    "sp_Student_GetDisabledList",
                    new
                    {
                        COMPANYID = companyId,
                        SESSIONID = sessionId,
                        CLASSID = classId,
                        SECTIONID = sectionId,
                        SEARCHTERM = searchTerm
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<StudentListViewModel>();
            }
        }

        /// <summary>
        /// Creates a new additional class assignment or updates
        /// an existing multi-class assignment for a student.
        /// </summary>
        /// <param name="req">
        /// Contains student, class, and section assignment details.
        /// </param>
        /// <param name="companyId">
        /// The company identifier.
        /// </param>
        /// <param name="sessionId">
        /// The academic session identifier.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) UpsertStudentMultiClass(
            StudentMultiClassUpsertRequest req,
            int companyId,
            int sessionId,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@MultiClassID", req.MultiClassID);
                param.Add("@StudentID", req.StudentID);
                param.Add("@ClassID", req.ClassID);
                param.Add("@SectionID", req.SectionID);
                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Student_MultiClasses_Upsert",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response received from database.");

                return (
                    Convert.ToInt32(result.RESULT) == 1,
                    Convert.ToString(result.MESSAGE) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes an additional class assignment associated with a student.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the multi-class record.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteStudentMultiClass(
            int id,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@MultiClassID", id);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "sp_Student_MultiClasses_Delete",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response received from database.");

                return (
                    Convert.ToInt32(result.RESULT) == 1,
                    Convert.ToString(result.MESSAGE) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        /// <summary>
        /// Deletes multiple student records in a single operation.
        /// </summary>
        /// <param name="studentIds">
        /// List of student identifiers to be deleted.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) BulkDeleteStudents(
            List<int> studentIds,
            int userId)
        {
            try
            {
                if (studentIds == null || !studentIds.Any())
                {
                    return (false, "No students selected for deletion.");
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                string ids = string.Join(",", studentIds);

                var param = new DynamicParameters();

                param.Add("@StudentIDs", ids);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "sp_Student_BulkDelete",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                    return (false, "No response received from database.");

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

        /// <summary>
        /// Deletes the specified student record from the system.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the student.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns a tuple containing the operation status and result message.
        /// </returns>
        public (bool Success, string Message) DeleteStudent(
            int id,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@STUDENTID", id);
                param.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_STUDENT_DELETE",
                    param,
                    commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return (false, "Failed to delete student record.");
                }

                return (
                    Convert.ToInt32(result.RESULT) > 0,
                    Convert.ToString(result.MESSAGE) ?? string.Empty
                );
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }


        public StudentCategoryViewModel? GetStudentCategoryById(
            int studentCategoryId,
            int companyId,
            int sessionId,
            int? userId = null)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@STUDENTCATEGORYID", studentCategoryId);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@UserID", userId, DbType.Int32);

                var result = conn.QueryFirstOrDefault<StudentCategoryViewModel>(
                    "SP_MST_STUDENTCATEGORY_GetById",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // SP returned no rows
                if (result == null) return null;

                // SP returned a row but reported failure (RESULT = 0)
                if (result.Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        public StudentHouseViewModel? GetStudentHouseById(
            int studentHouseId,
            int companyId,
            int sessionId,
            int? userId = null)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@STUDENTHOUSEID", studentHouseId);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@UserID", userId, DbType.Int32);

                var result = conn.QueryFirstOrDefault<StudentHouseViewModel>(
                    "SP_MST_STUDENTHOUSE_GESTUDENTHOUSEBYID",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // SP returned no rows
                if (result == null) return null;

                // SP returned a row but reported failure (RESULT = 0)
                if (result.Result != 1) return null;

                return result;
            }
            catch (Exception)
            {

                throw;
            }

        }


        public StudentDisableReasonViewModel GetDisableReasonsByID(
            int companyId,
            int sessionId,int disableReasonID, int? userID=null)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@DISABLEREASONID", disableReasonID);
                param.Add("@CompanyID", companyId);
                param.Add("@SessionID", sessionId);
                param.Add("@UserID", userID);

                var result = conn.QueryFirstOrDefault<StudentDisableReasonViewModel>(
                   "sp_Student_DisableReason_GetByID",
                   param,
                   commandType: CommandType.StoredProcedure);

                // SP returned no rows
                if (result == null) return null;

                // SP returned a row but reported failure (RESULT = 0)
                if (result.Result != 1) return null;

                return result;
                
            }
            catch
            {
                return new StudentDisableReasonViewModel();
            }
        }



        public async Task<List<StudentListViewModel>> GetStudentCopyList(
            int companyId, int sessionId)
        {
            try
            {
                
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));




                var param = new DynamicParameters();

                param.Add("@COMPANYID", companyId);
                param.Add("@SESSIONID", sessionId);

                var multi = await conn.QueryMultipleAsync(
                    "SP_STUDENT_LIST_GETFORCOPYSHOWDATA",
                    param,
                    commandType: CommandType.StoredProcedure);

                var student = (await multi.ReadAsync<StudentListViewModel>()).ToList();

                
                return student;
            }
            catch(Exception ex)
            {
                return new List<StudentListViewModel>();
            }
        }



        
        public async Task<(bool Success, string Message)> CopyStudentsToSession(CopyRequest req)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();
                parameters.Add("@FROM_COMPANYID", req.FromCompanyId);
                parameters.Add("@FROM_SESSIONID", req.FromSessionId);
                parameters.Add("@TO_COMPANYID", req.ToCompanyId);
                parameters.Add("@TO_SESSIONID", req.ToSessionId);
                parameters.Add("@STUDENT_IDS", null);
                parameters.Add("@COPY_PHOTO", 1);
                parameters.Add("@COPY_ADDRESS", 1);
                parameters.Add("@COPY_TRANSPORT", 1);
                parameters.Add("@COPY_HOSTEL", 1);
                parameters.Add("@COPY_CUSTOMFIELDS", 1);
                parameters.Add("@USERID", req.UserID);

                var result = await conn.QueryFirstOrDefaultAsync<SpResult>(
                    "SP_STUDENT_COPY_TO_SESSION",
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


        public (bool Success, string Message) UpdateStudentProfile(ProfileRequest req)
        {
            try
            {

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@StudentID", req.Id);
                parameters.Add("@PHOTODOC", req.PhotoDoc);
                parameters.Add("@USERID", req.UserId);
                parameters.Add("@MotherPhoto", req.MotherPhoto);
                parameters.Add("@FatherPhoto", req.FatherPhoto);
                parameters.Add("@GuardianPhoto", req.GuardianPhoto);
                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_Mst_Students_UpdateProfile",
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

        /// <summary>
        /// Retrieves a filtered list of active students with their custom field values.
        ///
        /// Flow:
        ///   1. Calls SP_STUDENT_LIST_GET with optional class/section/search filters.
        ///   2. Calls sp_StudentCustomFieldValues_GetAllActive once (avoids N+1 queries),
        ///      groups results by StudentID into a lookup dictionary.
        ///   3. Maps custom fields onto each student from the pre-built dictionary.
        ///
        /// Note: Dapper auto-maps SP column names to ViewModel properties —
        ///       no manual column-by-column mapping needed.
        /// </summary>
        public async Task<PagedResult<StudentHouseViewModel>> GetStudentHouseList(SubjectSearchRequest req)
        {
            try
            {
                if (req.PageNumber == 0 && req.PageSize == 0)
                {
                    req.PageNumber = 1;
                    req.PageSize = 10;
                }

                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@COMPANYID", req.CompanyID);
                param.Add("@SESSIONID", req.SessionID);
                param.Add("@SEARCHTERM", req.SearchKeyword);
                param.Add("@PAGENUMBER", req.PageNumber);
                param.Add("@PAGESIZE", req.PageSize);
                             

                var result = (await conn.QueryAsync<StudentHouseViewModel>(
                "SP_MST_STUDENTHOUSE_GETALLWITHPAGEINDEX",
                param,
                commandType: CommandType.StoredProcedure)).ToList();

                int res = result.FirstOrDefault()?.Result ?? 0;
                int totalRecords = result.FirstOrDefault()?.TOTALRECORDS ?? 0;
                int pageIndex = result.FirstOrDefault()?.CURRENTPAGE ?? 0;
                int pageSize = result.FirstOrDefault()?.PageSize ?? 0;


                var students = new PagedResult<StudentHouseViewModel>
                {
                    Data = result,
                    TotalRecords = totalRecords,
                    PageNumber = pageIndex,
                    PageSize = pageSize
                };                

                // ── 3. Attach custom fields + filter inactive students ───────────────
                // IsActive is mapped directly by Dapper; no manual column-name guessing needed
                return students;
            }
            catch
            {
                return new PagedResult<StudentHouseViewModel>();
            }
        }


        public List<StudentAddressDetailsModel> GetStudentAddressList(
            int companyId,
            int sessionId,
            int? studentId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                return conn.Query<StudentAddressDetailsModel>(
                    "SP_MST_STUDENT_ADDRESS_GETBYSTUDENT",
                    new
                    {
                        COMPANYID = companyId,
                        SESSIONID = sessionId,
                        STUDENTID = studentId
                    },
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<StudentAddressDetailsModel>();
            }
        }

        public async Task<bool> SaveStudentDocumentsBulkAsync(List<StudentDocumentSaveModel> documents, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var json = JsonSerializer.Serialize(documents);

                var parameters = new DynamicParameters();
                parameters.Add("@DocumentsJson", json);
                parameters.Add("@UserID", userId);

                var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                    "SP_STUDENT_DOCUMENT_BULK_INSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result?.RESULT == 1;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<StudentDocumentModel>> GetStudentDocumentsByStudentId(int studentId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parameters = new DynamicParameters();
                parameters.Add("@STUDENTID", studentId);

                var result = await conn.QueryAsync<StudentDocumentModel>(
                    "SP_STUDENT_DOCUMENT_GETBYSTUDENT",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
            catch (Exception ex)
            {
                return new List<StudentDocumentModel>();
            }
        }

        public async Task<List<StudentUserModel>> GetStudentLoginDetailsBind(int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@UserID", userId);

                var result = conn.Query<StudentUserModel>(
                    "sp_Student_Get_ByUserID",
                    param,
                    commandType: CommandType.StoredProcedure).ToList();

                return result;
            }
            catch (Exception)
            {
                return new List<StudentUserModel>();
            }
        }

        /// <summary>
        /// Retrieves the list of document type dropdown items.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A list of dropdown items.</returns>
        public List<DropdownModel> GetHostelRoomDropdownList(int companyId,int sessionID,int hostelID, int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.Query<DropdownModel>(
                    "sp_MstHostelRoom_GetByHostelId",
                    new
                    {
                        CompanyID = companyId,
                        SessionID= sessionID,
                        HostelID= hostelID,
                        UserId = userId
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

        public async Task<(bool Success, string Message)> BulkUpsertMultiClassAsync(
    int studentId, int companyId, int sessionId, int userId, string? ipAddress,
    List<MultiClassRowModel> rows)
        {
            try
            {
                var table = new DataTable();
                table.Columns.Add("MULTICLASSID", typeof(int));
                table.Columns.Add("CLASSID", typeof(int));
                table.Columns.Add("SECTIONID", typeof(int));
                table.Columns.Add("ISDELETED", typeof(bool));

                foreach (var r in rows)
                {
                    table.Rows.Add(r.MultiClassID, r.ClassID, r.SectionID, r.IsDeleted);
                }

                var parameters = new DynamicParameters();
                parameters.Add("@STUDENTID", studentId);
                parameters.Add("@COMPANYID", companyId);
                parameters.Add("@SESSIONID", sessionId);
                parameters.Add("@USERID", userId);
                parameters.Add("@IPADDRESS", ipAddress);
                parameters.Add("@ROWS", table.AsTableValuedParameter("dbo.TVP_STUDENT_MULTICLASS"));

                using var conn = new SqlConnection(
               _configuration.GetConnectionString("DefaultConnection"));
                var result = await conn.QuerySingleAsync<(int Result, string Message)>(
                    "SP_STUDENT_MULTICLASSES_BULK_UPSERT",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        public StudentDisableViewModel GetStudentDisable(int studentId, int companyId, int sessionId)
        {
            var model = new StudentDisableViewModel();
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));
                var dp = new DynamicParameters();
                dp.Add("StudentID", studentId);
                dp.Add("COMPANYID", companyId);
                dp.Add("SESSIONID", sessionId);

                
                var result = conn.QueryFirstOrDefault<StudentDisableViewModel>(
                   "tbl_Mst_Students_Disable_GetByID",
                   dp,
                   commandType: CommandType.StoredProcedure);

                // SP returned no rows
                if (result == null) return null;

                // SP returned a row but reported failure (RESULT = 0)
                if (result.Result != 1) return null;

                return result;
            }
            catch (Exception ex)
            {
                // Consider logging ex here
            }
            return model;
        }

        public (bool Success, string Message) UpsertStudentDisable(
            StudentDisableReq req,
            int userId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@StudentID", req.StudentID);
                param.Add("@SessionID", req.SessionID);
                param.Add("@CompanyID", req.CompanyID);
                param.Add("@DisableReasonID", req.DisableReasonID);
                param.Add("@DisableDate", req.DisableDate);
                param.Add("@DisableNote", req.DisableNote);
                param.Add("@UserID", userId);

                var result = conn.QueryFirstOrDefault<SpResult>(
                    "SP_STUDENT_DISABLE_UPDATE",
                    param,
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

        public (bool, string) ToggleStudentCategoryStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "SP_MST_STUDENTCATEGORY_TOGGLESTATUS",
                    new
                    {
                        STUDENTCATEGORYID = request.Ids,
                        IsActive = request.IsActive,
                        UserId = request.DoneBy
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> CopyStudentCategory(CopyRequest req)
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
                    "SP_MST_STUDENTCATEGORY_COPYSESSION",
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

        public (bool, string) ToggleStudentHouseStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "SP_MST_STUDENTHOUSE_TOGGLESTATUS",
                    new
                    {
                        STUDENTHOUSEID = request.Ids,
                        IsActive = request.IsActive,
                        UserId = request.DoneBy
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> CopyStudentHouse(CopyRequest req)
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
                    "SP_MST_STUDENTHOUSE_COPYSESSION",
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


        public (bool, string) ToggleDisableReasonStatus(StatusUpdateRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "SP_STUDENT_DISABLEREASON_TOGGLESTATUS",
                    new
                    {
                        DISABLEREASONID = request.Ids,
                        IsActive = request.IsActive,
                        UserId = request.DoneBy
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> CopyDisableReason(CopyRequest req)
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
                    "SP_STUDENT_DISABLEREASON_COPYSESSION",
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

        public (bool success, string message) UpsertStudentTimelineAttachmentFile(
  StudentTimelineAttachmentUpsertRequest req, int userId)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var parameters = new DynamicParameters();

                parameters.Add("@TIMELINEID", req.TimelineID);
                parameters.Add("@DOCUMENTCONTENT", req.Attachment);
                parameters.Add("@DOCUMENTNAME", req.FileName);
                parameters.Add("@DOCUMENTTYPE", req.FileType);
                parameters.Add("@USERID", userId);

                var result = conn.QueryFirstOrDefault<dynamic>(
                    "SP_STUDENT_TIMELINE_UPSERT_ATTACHMENT",
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

        public (bool, string) StudentChangePassword(StatusChangePassRequest request)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var result = conn.QueryFirstOrDefault<(int Result, string Message)>(
                    "SP_STUDENT_PASSWORD_CHANGE",
                    new
                    {
                        StudentID = request.StudentID,
                        Password = request.Password,
                        UserID = request.UserID
                    },
                    commandType: CommandType.StoredProcedure);

                return (result.Result == 1, result.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<StudentListViewModel>> GetStudentAllList(
            int companyId, int sessionId, int? classId, int? sectionId)
        {
            try
            {                
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@COMPANYID", companyId);
                param.Add("@SESSIONID", sessionId);
                param.Add("@CLASSID", classId);
                param.Add("@SECTIONID", sectionId);

                using var multi = await conn.QueryMultipleAsync(
                    "SP_STUDENT_GETALL",
                    param,
                    commandType: CommandType.StoredProcedure);

                var student = (await multi.ReadAsync<StudentListViewModel>()).ToList();


                return student;
            }
            catch
            {
                return new List<StudentListViewModel>();
            }
        }

        public async Task<StudentFeeTemplateListResponse>
        GetStudentFeeTemplateListAsync(int studentId,int companyId, int sessionId)
        {
            using var conn = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parameters = new DynamicParameters();

            parameters.Add("@STUDENTID", studentId);
            parameters.Add("@COMPANYID", companyId);
            parameters.Add("@SESSIONID", sessionId);

            using var multi = await conn.QueryMultipleAsync(
                "SP_STUDENT_FEE_TEMPLATE_LIST",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var data = (await multi
                .ReadAsync<StudentFeeTemplateResponse>())
                .ToList();

            var total = await multi
                .ReadFirstOrDefaultAsync<StudentFeeTemplateTotalResponse>();

            return new StudentFeeTemplateListResponse
            {
                Data = data,
                Total = total
            };
        }


        public List<FeesSearchStudentResponse> GetAllFeesSearchStudent(
            int companyId,string? studentSearch)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@COMPANYID", companyId);
                param.Add("@SEARCHTEXT", studentSearch);

                return conn.Query<FeesSearchStudentResponse>(
                    "SP_Fees_SearchStudent",
                    param,
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<FeesSearchStudentResponse>();
            }
        }

        public List<FeesSearchVoucherResponse> GetAllSearchVoucher(
            int companyId, string? studentSearch,int? voucherId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var param = new DynamicParameters();

                param.Add("@COMPANYID", companyId);
                param.Add("@SEARCHTEXT", studentSearch);
                param.Add("@VoucherId", voucherId);

                return conn.Query<FeesSearchVoucherResponse>(
                    "SP_Voucher_SearchVoucher",
                    param,
                    commandType: CommandType.StoredProcedure)
                    .ToList();
            }
            catch
            {
                return new List<FeesSearchVoucherResponse>();
            }
        }

        public async Task<List<int>> GetFilteredStudentIdsAsync(int companyId, string? search, int? classId, int? sectionId)
        {
            try
            {
                using var conn = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var ids = await conn.QueryAsync<int>(
                    "dbo.usp_GetFilteredStudentIds",
                    new { CompanyID = companyId, Search = search, ClassID = classId, SectionID = sectionId },
                    commandType: CommandType.StoredProcedure);

                return ids.ToList();
            }
            catch (Exception ex)
            {
                return [0];
            }
            
        }
    }
}
