using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.ComponentModel.Design;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Tasks;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize]
    /// <summary>
    /// This is a technical 'API' controller that provides data to the website.
    /// It handles background requests like getting student lists, updating statuses, 
    /// and managing student categories without reloading the whole page.
    /// Think of it as a behind-the-scenes data provider.
    /// </summary>
    public class StudentInformationApiController : ControllerBase
    {
        private readonly IStudentInformationService _studentService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        private readonly IAttendanceService _attendanceService;
        private readonly IBulkMailService _bulkMail;

        public StudentInformationApiController(IStudentInformationService studentService,
            ICompanyService companyService,
            ISessionService sessionService,
            IAttendanceService attendanceService,
            IBulkMailService bulkMail)
        {
            _studentService = studentService;
            _companyService = companyService;
            _sessionService = sessionService;
            _attendanceService = attendanceService;
            _bulkMail = bulkMail;
        }

        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;
        
        /// <summary>
        /// Retrieves the attendance history for a student for a given year.
        /// Use this to get present/absent records for reporting or analysis.
        /// </summary>
        [HttpGet("GetStudentAttendanceHistory/{studentId}/{year}")]
        /// <summary>
        /// Provides the attendance record for a student (how many days present/absent).
        /// </summary>
        public IActionResult GetStudentAttendanceHistory(int studentId, int year)
        {
            try
            {
                var data = _attendanceService.GetStudentAttendanceHistory(studentId, year, GetCompanyId());
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all reasons why students can be disabled (e.g., Transfer Certificate, Left School).
        /// Use this to populate disable reason dropdowns or manage reasons.
        /// </summary>
        [HttpGet("GetAllDisableReasons")]
        /// <summary>
        /// Provides the list of all reasons why students are disabled.
        /// </summary>
        public IActionResult GetAllDisableReasons(int sessionID,int companyId, bool? IncludeIsActive = null)
        {
            try
            {
                var data = _studentService.GetAllDisableReasons(companyId, sessionID, IncludeIsActive);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Creates or updates a disable reason for students.
        /// Use this to add a new reason or edit an existing one.
        /// </summary>
        [HttpPost("UpsertDisableReason")]
        public IActionResult UpsertDisableReason([FromBody] StudentDisableReasonUpsertRequest req)
        {
            try
            {
                if(req.SessionID==null || req.SessionID == 0) 
                {
                    req.SessionID = GetSessionId();
                }
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = GetCompanyId();
                }
                var res = _studentService.UpsertDisableReason(req, req.CompanyID, req.SessionID, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        /// <summary>
        /// Deletes one or more disable reasons by their IDs.
        /// Use this to remove reasons that are no longer needed.
        /// </summary>
        [HttpPost("DeleteDisableReason")]
        public IActionResult DeleteDisableReason(List<int> ids)
        {
            try
            {
                var res = _studentService.DeleteDisableReason(ids, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves all student houses (e.g., Red House, Blue House).
        /// Use this to populate house dropdowns or manage house assignments.
        /// </summary>
        [HttpGet("GetAllStudentHouses")]
        /// <summary>
        /// Provides the list of all student houses.
        /// </summary>
        public IActionResult GetAllStudentHouses(int sessionID, int companyId, bool? IncludeIsActive = null)
        {
            try
            {
                if(companyId==null || companyId == 0) 
                {
                    companyId = GetCompanyId();
                }
                if (sessionID == null || sessionID == 0)
                {
                    sessionID = GetSessionId();
                }
                var data = _studentService.GetAllStudentHouses(companyId, sessionID, IncludeIsActive);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message=ex.Message });
            }
           
        }

        /// <summary>
        /// Creates or updates a student house.
        /// Use this to add a new house or edit an existing one.
        /// </summary>
        [HttpPost("UpsertStudentHouse")]
        public IActionResult UpsertStudentHouse([FromBody] StudentHouseUpsertRequest req)
        {
            try
            {
                if(req.SessionID==null || req.SessionID == 0) 
                {
                    req.SessionID = GetSessionId();
                }
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = GetCompanyId();
                }

                var res = _studentService.UpsertStudentHouse(req, req.CompanyID, req.SessionID, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
           
        }

        /// <summary>
        /// Deletes one or more student houses by their IDs.
        /// Use this to remove houses that are no longer needed.
        /// </summary>
        [HttpPost("DeleteStudentHouse")]
        public IActionResult DeleteStudentHouse(List<int> ids)
        {
            try
            {
                var res = _studentService.DeleteStudentHouse(ids, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
           
        }

        /// <summary>
        /// Retrieves all student categories (e.g., General, OBC, SC).
        /// Use this to populate category dropdowns or manage categories.
        /// </summary>
        [HttpGet("GetAllStudentCategories")]
        /// <summary>
        /// Provides the list of all student categories.
        /// </summary>
        public IActionResult GetAllStudentCategories(int sessionId,int companyId, bool? includeDeleted = null)
        {
            try
            {
                var data = _studentService.GetAllStudentCategories(companyId, sessionId, includeDeleted);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
           
        }

        /// <summary>
        /// Creates or updates a student category.
        /// Use this to add a new category or edit an existing one.
        /// </summary>
        [HttpPost("UpsertStudentCategory")]
        public IActionResult UpsertStudentCategory([FromBody] StudentCategoryUpsertRequest req)
        {
            try
            {
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = GetSessionId();
                }
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = GetCompanyId();
                }
                var res = _studentService.UpsertStudentCategory(req, req.CompanyID, req.SessionID, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Deletes one or more student categories by their IDs.
        /// Use this to remove categories that are no longer needed.
        /// </summary>
        [HttpPost("DeleteStudentCategory")]
        public IActionResult DeleteStudentCategory(List<int> ids)
        {
            try
            {
                var res = _studentService.DeleteStudentCategory(ids, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves the timeline (history/events) for a student.
        /// Use this to display important events in a student's academic life.
        /// </summary>
        [HttpGet("GetStudentTimeline/{studentId}")]
        public IActionResult GetStudentTimeline(int studentId)
        {
            try
            {
                var data = _studentService.GetStudentTimeline(studentId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Creates or updates a timeline entry for a student.
        /// Use this to add or edit events in a student's timeline.
        /// </summary>
        [HttpPost("UpsertTimeline")]
        public IActionResult UpsertTimeline([FromBody] StudentTimelineUpsertRequest req)
        {
            try
            {
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = GetSessionId();
                }
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = GetCompanyId();
                }
                var result = _studentService.UpsertStudentTimeline(req, req.CompanyID, req.SessionID, GetUserId());
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Deletes a timeline entry for a student by its ID.
        /// </summary>
        [HttpPost("DeleteTimeline/{id}")]
        public IActionResult DeleteTimeline(int id)
        {
            try
            {
                var result = _studentService.DeleteStudentTimeline(id, GetUserId());
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Downloads a document attached to a student's timeline entry.
        /// Use this to allow users to download timeline-related files.
        /// </summary>
        [HttpGet("DownloadTimelineDoc/{id}")]
        public IActionResult DownloadTimelineDoc(int id)
        {
            try
            {
                var result = _studentService.GetStudentTimelineDocument(id);
                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }

        /// <summary>
        /// Changes a student's status (e.g., from 'Active' to 'Disabled').
        /// Use this to enable or disable a student record.
        /// </summary>
        [HttpPost("ToggleStatus")]
        /// <summary>
        /// Changes a student's status (e.g., from 'Active' to 'Disabled').
        /// </summary>
        public IActionResult ToggleStatus([FromBody] StudentStatusToggleRequest req)
        {
            try
            {
                var res = _studentService.ToggleStudentStatus(req, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves a student by their unique ID, including all details.
        /// Use this to display or edit a student's profile.
        /// </summary>
        [HttpGet("GetByID")]
        public IActionResult GetByID(int id,int companyId,int sessionId)
        {
            try
            {
                if (companyId == 0 || companyId == null) 
                {
                    companyId = GetCompanyId();
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = GetSessionId();
                }
                var data = _studentService.GetStudentDetails(id, companyId, sessionId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        /// <summary>
        /// Retrieves students assigned to multiple classes, filtered by class, section, or search term.
        /// Use this for advanced student assignment scenarios.
        /// </summary>
        [HttpGet("GetMultiClassStudents")]
        public IActionResult GetMultiClassStudents(int? classId, int? sectionId,int? companyId,int? sessionId , string? searchTerm)
        {
            try
            {
                if(companyId==0 || companyId == null) 
                {
                    companyId = GetCompanyId();
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = GetSessionId();
                }
                var data = _studentService.GetMultiClassStudents(companyId.Value, sessionId.Value, classId, sectionId, searchTerm);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves a paginated list of students, filtered by class, section, or search term.
        /// Use this for student list grids or dropdowns.
        /// </summary>
        [HttpGet("GetStudentList")]
        public async Task<IActionResult> GetStudentList(int? companyId,int? sessionId, int? classId, int? sectionId, string? searchTerm, int PageNumber, int PageSize, int? studentId)
        {
            try
            {
                var data = await _studentService.GetStudentList(companyId.Value, sessionId.Value, classId, sectionId, searchTerm, PageNumber, PageSize, studentId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Creates or updates a multi-class assignment for a student.
        /// Use this to assign a student to multiple classes.
        /// </summary>
        [HttpPost("UpsertMultiClass")]
        public IActionResult UpsertMultiClass([FromBody] StudentMultiClassUpsertRequest req)
        {
            try
            {
                var res = _studentService.UpsertStudentMultiClass(req, GetCompanyId(), GetSessionId(), GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Deletes a multi-class assignment for a student by its ID.
        /// </summary>
        [HttpPost("DeleteMultiClass/{id}")]
        public IActionResult DeleteMultiClass(int id)
        {
            try
            {
                var res = _studentService.DeleteStudentMultiClass(id, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }


        /// <summary>
        /// Generates the next student roll number based on rules and settings.
        /// Use this to auto-generate roll numbers for new admissions.
        /// </summary>
        [HttpPost("GetNewStudentRollNo")]
        public IActionResult GetNewStudentRollNo([FromBody] StudentRollNoRequest request)
        {
            try
            {
                var rollNo = _studentService.GetNewStudentRollNo(
                    request.CompanyID, request.SessionId,
                    request.DynamicValues);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Roll number generated successfully.",
                    Data = rollNo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = string.Empty
                });
            }
        }


        /// <summary>
        /// Generates the next available admission number for a new student.
        /// Use this to auto-generate admission numbers for new admissions.
        /// </summary>
        [HttpGet("GetNextAdmissionNo")]
        public IActionResult GetNextAdmissionNo()
        {
            try
            {
                var admissionNo = _studentService.GetNextSimpleAdmissionNo(
                    GetCompanyId());

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Admission number generated successfully.",
                    Data = admissionNo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = string.Empty
                });
            }
        }


        /// <summary>
        /// Creates or updates a student admission record.
        /// Use this to add a new student or edit an existing student's admission details.
        /// </summary>
        [HttpPost("UpsertStudentAdmission")]
        public IActionResult UpsertStudentAdmission(
            [FromBody] StudentAdmissionUpsertRequest request)
        {
            try
            {
                if(request.CompanyID==null && request.CompanyID == 0) 
                {
                    request.CompanyID = GetCompanyId();
                }
                if (request.SessionId == null && request.SessionId == 0)
                {
                    request.SessionId = GetSessionId();
                }
                var result = _studentService.UpsertStudentAdmission(
                    request,
                    request.CompanyID,
                    request.SessionId,
                    GetUserId());

                return Ok(new ApiResponse<int>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.StudentID
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                });
            }
        }


        /// <summary>
        /// Deletes multiple students at once by their IDs.
        /// Use this for bulk student deletion operations.
        /// </summary>
        [HttpPost("BulkDeleteStudents")]
        public IActionResult BulkDeleteStudents([FromBody] List<int> studentIds)
        {
            try
            {
                var result = _studentService.BulkDeleteStudents(
                    studentIds,
                    GetUserId());

                return Ok(new ApiResponse<object>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        /// <summary>
        /// Retrieves the list of disabled students, optionally filtered by class, section, or search term.
        /// Use this to manage students who are no longer active.
        /// </summary>
        [HttpGet("GetDisabledStudentList")]
        public IActionResult GetDisabledStudentList(
            int? classId = null,
            int? sectionId = null,
            string? searchTerm = null, int? sessionId = null, int? companyId = null)
        {
            try
            {
                if(companyId==null || companyId == 0) 
                {
                    companyId = GetCompanyId();
                }
                var data = _studentService.GetDisabledStudentList(
                    companyId.Value,
                    sessionId.Value,
                    classId,
                    sectionId,
                    searchTerm);

                return Ok(new ApiResponse<List<StudentListViewModel>>
                {
                    Success = true,
                    Message = "Disabled student list retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<StudentListViewModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new List<StudentListViewModel>()
                });
            }
        }



        /// <summary>
        /// Retrieves a student category by its unique ID.
        /// Use this to display or edit a specific student category.
        /// </summary>
        [HttpGet("GetStudentCategoryById")]
        public IActionResult GetStudentCategoryById(int id,int companyId,int sessionId)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID." });
                if (companyId == 0 || companyId == null)
                {
                    companyId = GetCompanyId();
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = GetSessionId();
                }

                var result = _studentService.GetStudentCategoryById(
                    studentCategoryId: id, companyId,
                    sessionId,
                    userId: GetUserId() > 0 ? GetUserId() : null);

                if (result == null)
                    return Ok(new { success = false, message = "Record not found." });

                return Ok(new { success = true, message = result.Message, data = result });
            }
            catch (Exception)
            {
                throw;
            }            
        }



        /// <summary>
        /// Retrieves a student house by its unique ID.
        /// Use this to display or edit a specific student house.
        /// </summary>
        [HttpGet("GetStudentHouseById/{id}")]
        public IActionResult GetStudentHouseById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID." });
                var sessionId = GetSessionId();

                var result = _studentService.GetStudentHouseById(
                    studentHouseId: id,
                    companyId: GetCompanyId(),
                    sessionId: sessionId,
                    userId: GetUserId() > 0 ? GetUserId() : null);

                if (result == null)
                    return Ok(new { success = false, message = "Record not found." });

                return Ok(new { success = true, message = result.Message, data = result });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves a disable reason by its unique ID.
        /// Use this to display or edit a specific disable reason.
        /// </summary>
        [HttpGet("GetDisableReasonsByID/{id}")]
        public IActionResult GetDisableReasonsByID(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID." });
                var sessionId = GetSessionId();

                var result = _studentService.GetDisableReasonsByID(
                    disableReasonID: id,
                    companyId: GetCompanyId(),
                    sessionId: sessionId,
                    userID: GetUserId() > 0 ? GetUserId() : null);

                if (result == null)
                    return Ok(new { success = false, message = "Record not found." });

                return Ok(new { success = true, message = result.Message, data = result });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates a student's profile information, such as photos or documents.
        /// Use this to save changes to a student's profile.
        /// </summary>
        [HttpPost("UpdateStudentProfile")]
        public IActionResult UpdateStudentProfile([FromBody] ProfileRequest req)
        {
            try
            {
                req.UserId = GetUserId();
                var result = _studentService.UpdateStudentProfile(req);
                return Ok(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
        }


        /// <summary>
        /// Retrieves a paginated list of student houses based on search and filter criteria.
        /// Use this for displaying student houses in a paged grid or table.
        /// </summary>
        [HttpPost("GetStudentHouseList")]
        public async Task<IActionResult> GetStudentHouseList([FromBody] SubjectSearchRequest request)
        {
            try
            {

                int userId = GetUserId();
                int companyId = _companyService.GetUserCurrentCompany(userId) ?? 0;

                if (request.SessionID == null)
                {
                    request.SessionID = _sessionService.GetUserCurrentSession(userId) ?? 0;
                }
                if (companyId == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<StudentHouseViewModel>>.SuccessResponse(new List<StudentHouseViewModel>()));

                var data = await _studentService.GetStudentHouseList(request);
                return Ok(ApiResponse<PagedResult<StudentHouseViewModel>>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves a filtered list of students for dropdowns or binding scenarios.
        /// Use this for populating student selection lists.
        /// </summary>
        [HttpPost("GetStudentBind")]
        public async Task<IActionResult> GetStudentBind(StudentDropDwonBindRequestModel req)
        {
            try
            {
                var data = await _studentService.GetStudentBind(req);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Database Error: " + ex.Message });
            }            
        }


        /// <summary>
        /// Retrieves the address details for a student.
        /// Use this to display or edit a student's address information.
        /// </summary>
        [HttpGet("GetStudentAddressList")]
        public IActionResult GetStudentAddressList(int companyId,int sessionId,int studentId)
        {
            try
            {
                if (studentId <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID." });
                
                var result = _studentService.GetStudentAddressList(
                    companyId,
                    sessionId, studentId);

                if (result == null)
                    return Ok(new { success = false, message = "Record not found." });

                return Ok(new { success = true,  data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Saves multiple student documents in bulk.
        /// Use this to upload and associate documents with student records.
        /// </summary>
        [HttpPost("SaveStudentDocumentsBulk")]
        public IActionResult SaveStudentDocumentsBulk([FromBody] List<StudentDocumentSaveModel> documents)
        {
            try
            {               

                var result = _studentService.SaveStudentDocumentsBulkAsync(documents,
                    GetUserId());

                return Ok(new { success = true, result });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<int>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                });
            }
        }

        /// <summary>
        /// Retrieves all documents associated with a student.
        /// Use this to display or download student documents.
        /// </summary>
        [HttpGet("GetStudentDocuments")]
        public async Task<IActionResult> GetStudentDocuments(int studentId)
        {
            try
            {
                var documents = await _studentService.GetStudentDocumentsByStudentId(studentId);
                return Ok(new { success = true, data = documents });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpGet("GetStudentLoginDetailsBind")]
        public async Task<IActionResult> GetStudentLoginDetailsBind(int userId)
        {
            try
            {
                var data =await _studentService.GetStudentLoginDetailsBind(userId);

                return Ok(new ApiResponse<List<StudentUserModel>>
                {
                    Success = true,
                    Message = "Disabled student list retrieved successfully.",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<StudentUserModel>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = new List<StudentUserModel>()
                });
            }
        }

        [HttpGet("GetHostelRoomDropdownList")]
        public IActionResult GetHostelRoomDropdownList(int companyId, int sessionID, int hostelID)
        {
            try
            {
                int userId = GetUserId();
                var result = _studentService.GetHostelRoomDropdownList(companyId, sessionID, hostelID, userId);

                return Ok(new ApiResponse<List<DropdownModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("BulkUpsertMultiClass")]
        public async Task<IActionResult> BulkUpsertMultiClass([FromBody] MultiClassBulkSaveRequest request)
        {
            try
            {
                if (request == null || request.StudentID <= 0 || request.Rows == null || request.Rows.Count == 0)
                {
                    return Ok(new { success = false, message = "No changes to save." });
                }

                if(request.CompanyID==0|| request.CompanyID == null) 
                {
                    request.CompanyID = GetCompanyId();
                }
                if (request.SessionID == 0 || request.SessionID == null)
                {
                    request.SessionID = GetSessionId();
                }
                int currentUserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

                var (success, message) = await _studentService.BulkUpsertMultiClassAsync(
                    request.StudentID, request.CompanyID, request.SessionID, currentUserId, ip, request.Rows);

                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetStudentDisable")]
        public IActionResult GetStudentDisable(int studentId, int companyId, int sessionId)
        {
            try
            {
                if (companyId == 0 || companyId == null)
                {
                    companyId = GetCompanyId();
                }
                if (sessionId == 0 || sessionId == null)
                {
                    sessionId = GetSessionId();
                }
                var data = _studentService.GetStudentDisable(studentId, companyId, sessionId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost("UpsertStudentDisable")]
        public IActionResult UpsertStudentDisable([FromBody] StudentDisableReq req)
        {
            try
            {
                if (req.SessionID == null || req.SessionID == 0)
                {
                    req.SessionID = GetSessionId();
                }
                if (req.CompanyID == null || req.CompanyID == 0)
                {
                    req.CompanyID = GetCompanyId();
                }

                var res = _studentService.UpsertStudentDisable(req, GetUserId());
                return Ok(new { success = res.Success, message = res.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllStudentCategoriesWithPage")]
        public async Task<IActionResult> GetAllStudentCategoriesWithPage([FromBody] StudentCategorySearch request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = GetCompanyId();
                }
                if (request.SessionID == null)
                {
                    request.SessionID = GetSessionId();
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<StudentCategoryViewModel>>.SuccessResponse(new List<StudentCategoryViewModel>()));

                var data = await _studentService.GetAllStudentCategoriesWithPage(request);
                return Ok(ApiResponse<PagedResult<StudentCategoryViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("CopyStudentCategory")]
        public async Task<IActionResult> CopyStudentCategory([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = GetUserId();
                }
                // ── Execute copy ──────────────────────────────────────────
                var result = await _studentService.CopyStudentCategory(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost("ToggleStudentCategoryStatus")]
        public async Task<IActionResult> ToggleStudentCategoryStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                request.DoneBy = GetUserId();
                var (success, message) = _studentService.ToggleStudentCategoryStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("CopyStudentHouse")]
        public async Task<IActionResult> CopyStudentHouse([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = GetUserId();
                }
                // ── Execute copy ──────────────────────────────────────────
                var result = await _studentService.CopyStudentHouse(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost("ToggleStudentHouseStatus")]
        public async Task<IActionResult> ToggleStudentHouseStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                request.DoneBy = GetUserId();
                var (success, message) = _studentService.ToggleStudentHouseStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("CopyDisableReason")]
        public async Task<IActionResult> CopyDisableReason([FromBody] CopyRequest request)
        {
            try
            {

                if (request.UserID == null || request.UserID == 0)
                {
                    request.UserID = GetUserId();
                }
                // ── Execute copy ──────────────────────────────────────────
                var result = await _studentService.CopyDisableReason(request);

                if (result.Success)
                {
                    return Ok(new { success = result.Success, message = result.Message });
                }
                else
                {
                    return Ok(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An unexpected error occurred." });
            }
        }

        [HttpPost("ToggleDisableReasonStatus")]
        public async Task<IActionResult> ToggleDisableReasonStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                request.DoneBy = GetUserId();
                var (success, message) = _studentService.ToggleDisableReasonStatus(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("GetAllDisableReasonsWithPage")]
        public async Task<IActionResult> GetAllDisableReasonsWithPage([FromBody] StudentCategorySearch request)
        {
            try
            {
                if (request.CompanyID == null)
                {
                    request.CompanyID = GetCompanyId();
                }
                if (request.SessionID == null)
                {
                    request.SessionID = GetSessionId();
                }
                if (request.CompanyID == 0 || request.SessionID == 0)
                    return Ok(ApiResponse<List<StudentDisableReasonViewModel>>.SuccessResponse(new List<StudentDisableReasonViewModel>()));

                var data = await _studentService.GetAllDisableReasonsWithPage(request);
                return Ok(ApiResponse<PagedResult<StudentDisableReasonViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetTimelineById/{id}")]
        public IActionResult GetTimelineById(int id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest(new { success = false, message = "Invalid ID." });               

                var result = _studentService.GetTimelineById(id);

                if (result == null)
                    return Ok(new { success = false, message = "Record not found." });

                return Ok(new { success = true, message = result.Message, data = result });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("UpsertStudentTimelineAttachmentFile")]
        public IActionResult UpsertStudentTimelineAttachmentFile([FromBody] StudentTimelineAttachmentUpsertRequest req)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _studentService.UpsertStudentTimelineAttachmentFile(req, GetUserId());
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpPost("StudentChangePassword")]
        public async Task<IActionResult> StudentChangePassword([FromBody] StatusChangePassRequest request)
        {
            try
            {
                request.UserID = GetUserId();
                var (success, message) = _studentService.StudentChangePassword(request);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        [HttpGet("GetStudentAllList")]
        public async Task<IActionResult> GetStudentAllList(int? companyId, int? sessionId, int? classId, int? sectionId)
        {
            try
            {
                var data = await _studentService.GetStudentAllList(companyId.Value, sessionId.Value, classId, sectionId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }

        }

        [HttpGet("GetStudentFeeTemplateList")]
        public async Task<IActionResult> GetStudentFeeTemplateList(int studentId,int companyId, int sessionId)
        {
            try
            {
                if (studentId == null)
                {
                    return BadRequest(new
                    {
                        Result = 0,
                        Message = "REQUEST IS REQUIRED"
                    });
                }

                var response = await _studentService.GetStudentFeeTemplateListAsync(studentId, companyId, sessionId);
                return Ok(new { success = true, Data = response });
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Result = 0,
                    Message = "UNABLE TO FETCH STUDENT FEE TEMPLATE LIST",
                    TechnicalMessage = ex.Message
                });
            }
        }

        [HttpGet("GetAllFeesSearchStudent")]
        public IActionResult GetAllFeesSearchStudent(int companyId, string? studentSearch)
        {
            try
            {
                var data = _studentService.GetAllFeesSearchStudent(companyId, studentSearch);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }

        }

        [HttpGet("GetAllSearchVoucher")]
        public IActionResult GetAllSearchVoucher(int companyId, string? studentSearch, int? voucherId)
        {
            try
            {
                var data = _studentService.GetAllSearchVoucher(companyId, studentSearch, voucherId);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }

        }

        [HttpGet("GetFilteredStudentIds")]
        /// <summary>
        /// Provides the list of all reasons why students are disabled.
        /// </summary>
        public async Task<IActionResult> GetFilteredStudentIds(int companyId, string? search, int? classId, int? sectionId)
        {
            try
            {
                var data =await _studentService.GetFilteredStudentIdsAsync(companyId, search, classId, sectionId);

                return Ok(new ApiResponse<List<int>>
                {
                    Success = true,
                    Data = data
                });

               // return Ok(new { success = true, Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }


        [HttpPost("SendBulkMail")]
        public async Task<IActionResult> SendBulkMail([FromBody] BulkMailSendRequest req)
        {
            try
            {
                var result = await _bulkMail.QueueBulkMailAsync(req, req.CompanyId);
                return Ok(new { result }); // { success, message, batchId, queued, skipped }

            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "SendBulkMail failed");
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpGet("BatchStatus")]
        public async Task<IActionResult> BatchStatus(Guid batchId)
        {
            var status = await _bulkMail.GetBatchStatusAsync(batchId);
            return Ok(new { status });           // { total, sent, failed, pending }
        }
    }
}
