using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERPWebService.Interfaces;
using SchoolERPWebService.Services;
using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolERPWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LibraryApiController : Controller
    {
        private readonly ILibraryService _libraryService;
        private readonly ICompanyService _companyService;
        private readonly ISessionService _sessionService;
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryApiController"/> class.
        /// </summary>
        /// <param name="libraryService">The library service implementation.</param>
        /// <param name="companyService">The company service implementation.</param>
        /// <param name="sessionService">The session service implementation.</param>
        public LibraryApiController(ILibraryService libraryService,
            ICompanyService companyService,
            ISessionService sessionService)
        {
            _libraryService = libraryService;
            _companyService = companyService;
            _sessionService = sessionService;
        }

        /// <summary>
        /// Gets the current user ID from the user claims.
        /// </summary>
        /// <returns>The user ID, or 0 if not found.</returns>
        private int GetUserId() => int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("UserId")?.Value, out var id) ? id : 0;

        /// <summary>
        /// Gets the current company ID of the user.
        /// </summary>
        /// <returns>The company ID, or 0 if not found.</returns>
        private int GetCompanyId() => _companyService.GetUserCurrentCompany(GetUserId()) ?? 0;

        /// <summary>
        /// Gets the current session ID of the user.
        /// </summary>
        /// <returns>The session ID, or 0 if not found.</returns>
        private int GetSessionId() => _sessionService.GetUserCurrentSession(GetUserId()) ?? 0;
        /// <summary>
        /// Retrieves the list of books for the specified company.
        /// Optionally filters the results using a search term.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="searchTerm">Optional search text to filter books.</param>
        /// <returns>A list of books matching the criteria.</returns>
        [HttpGet("GetBookList")]
        public IActionResult GetBookList(int companyId, string? searchTerm)
        {
            try
            {
                var result = _libraryService.GetBookList(companyId, searchTerm);

                return Ok(new ApiResponse<List<BookViewModel>>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<BookViewModel>>
                {
                    Success = false,
                    Message=ex.Message,
                    Data = null
                });
            }
            
        }

        /// <summary>
        /// Retrieves the details of a specific book by its unique identifier.
        /// </summary>
        /// <param name="id">The book identifier.</param>
        /// <returns>The book details if found; otherwise null.</returns>
        [HttpGet("GetBookById")]
        public IActionResult GetBookById(int id)
        {
            try
            {
                var result = _libraryService.GetBookById(id);

                return Ok(new ApiResponse<BookViewModel>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<BookViewModel>
                {
                    Success = true,
                    Message=ex.Message,
                    Data = null
                });
            }
           
        }

        /// <summary>
        /// Creates or updates a library book record.
        /// </summary>
        /// <param name="req">Book information to save.</param>
        /// <returns>
        /// Returns success status and operation message.
        /// </returns>
        [HttpPost("UpsertBook")]
        public async Task<IActionResult> UpsertBook([FromBody] BookUpsertRequest req)
        {
            try
            {
                var companyId = GetCompanyId();
                var userId = GetUserId();
                if (req.CompanyID == null)
                {
                    req.CompanyID = GetCompanyId();
                }
                var result = await _libraryService.UpsertBook(req, req.CompanyID.Value, userId);
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Deletes a library book.
        /// </summary>
        /// <param name="id">Book identifier.</param>
        /// <returns>
        /// Returns success status and operation message.
        /// </returns>
        [HttpPost("DeleteBook")]
        public IActionResult DeleteBook(List<int> id)
        {
            try
            {
                var userId = GetUserId();

                var result = _libraryService.DeleteBook(id, userId);

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
           
        }

        /// <summary>
        /// Changes the active/inactive status of a library book.
        /// </summary>
        /// <param name="id">Book identifier.</param>
        /// <returns>
        /// Returns success status and operation message.
        /// </returns>
        [HttpPost("ToggleBookStatus")]
        public IActionResult ToggleBookStatus([FromBody] StatusUpdateRequest request)
        {
            try
            {
                var userId = GetUserId();
                request.DoneBy = userId;
                var result = _libraryService.ToggleBookStatus(request);

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Retrieves library members based on member type and optional search filters.
        /// This method is used to display registered library members such as students,
        /// staff, or all members.
        /// </summary>
        /// <param name="memberType">
        /// Type of member to retrieve (Student, Staff, or All).
        /// </param>
        /// <param name="companyId">
        /// Company identifier used for data isolation.
        /// </param>
        /// <param name="classId">
        /// Optional class filter for student members.
        /// </param>
        /// <param name="sectionId">
        /// Optional section filter for student members.
        /// </param>
        /// <param name="departmentId">
        /// Optional department filter for staff members.
        /// </param>
        /// <param name="search">
        /// Optional search text for member name, admission number, or card number.
        /// </param>
        /// <returns>
        /// A collection of library members matching the specified criteria.
        /// </returns>
        [HttpGet("GetMemberList")]
        public IActionResult GetMemberList(
            string memberType,
            int companyId,
            int? classId,
            int? sectionId,
            int? departmentId,
            string? search)
        {
            try
            {
                var result = _libraryService.GetMemberList(
                memberType,
                companyId,
                classId,
                sectionId,
                departmentId,
                search);

                return Ok(ApiResponse<List<LibraryMemberViewModel>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }

        /// <summary>
        /// Searches students or staff members who are eligible for library membership.
        /// This method is typically used before registering a new library member.
        /// </summary>
        /// <param name="memberType">
        /// Member type to search for (Student or Staff).
        /// </param>
        /// <param name="companyId">
        /// Company identifier.
        /// </param>
        /// <param name="classId">
        /// Optional class filter.
        /// </param>
        /// <param name="sectionId">
        /// Optional section filter.
        /// </param>
        /// <param name="departmentId">
        /// Optional department filter.
        /// </param>
        /// <param name="search">
        /// Optional search keyword.
        /// </param>
        /// <returns>
        /// A collection of matching students or staff members.
        /// </returns>
        [HttpGet("SearchForMembership")]
        public IActionResult SearchForMembership(
            string memberType,
            int companyId,
            int? classId,
            int? sectionId,
            int? departmentId,
            string? search)
        {
            try
            {
                var result = _libraryService.SearchForMembership(
             memberType,
             companyId,
             classId,
             sectionId,
             departmentId,
             search);

                return Ok(ApiResponse<List<MembershipSearchViewModel>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
         
        }

        /// <summary>
        /// Registers a new library member and generates a library membership record.
        /// This method supports both student and staff memberships.
        /// </summary>
        /// <param name="req">
        /// Membership information to be saved.
        /// </param>
        /// <param name="companyId">
        /// Company identifier.
        /// </param>
        /// <param name="userId">
        /// User performing the operation.
        /// </param>
        /// <returns>
        /// Success status and operation message.
        /// </returns>
        [HttpPost("AddMember")]
        public IActionResult AddMember(
            [FromBody] LibraryMemberUpsertRequest req,
            int companyId,
            int userId)
        {
            try
            {
                var result = _libraryService.AddMember(
                req,
                companyId,
                userId);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    result.Success,
                    result.Message
                }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.SuccessResponse(new
                {
                    Success=false,
                    Message = ex.Message
                }));
            }
            
        }

        /// <summary>
        /// Deletes a library member using the specified member ID.
        /// </summary>
        /// <param name="id">
        /// The unique identifier of the library member.
        /// </param>
        /// <param name="userId">
        /// The ID of the user performing the delete operation.
        /// </param>
        /// <returns>
        /// Returns the operation result along with a success or failure message.
        /// </returns>
        [HttpDelete("DeleteMember")]
        public IActionResult DeleteMember(int id, int userId)
        {
            try
            {
                var result = _libraryService.DeleteMember(id, userId);

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            
        }

        /// <summary>
        /// Retrieves all books currently issued to a specific library member.
        /// </summary>
        /// <param name="memberId">
        /// The library member ID.
        /// </param>
        /// <param name="companyId">
        /// The company or school identifier.
        /// </param>
        /// <returns>
        /// Returns the list of issued books.
        /// </returns>
        [HttpGet("GetIssuedBooks")]
        public IActionResult GetIssuedBooks(
            int memberId,
            int companyId)
        {
            try
            {
                var data = _libraryService.GetIssuedBooks(
                memberId,
                companyId);

                return Ok(new ApiResponse<List<IssueReturnViewModel>>
                {
                    Success = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<IssueReturnViewModel>>
                {
                    Success = false,
                    Message = ex.Message,   
                    Data = null
                });
            }
            
        }

        /// <summary>
        /// Issues a book to a library member and records the issue and due return dates.
        /// </summary>
        /// <param name="request">Book issue details.</param>
        /// <returns>A response indicating whether the book was issued successfully.</returns>
        [HttpPost("IssueBook")]
        public IActionResult IssueBook(IssueReturnUpsertRequest request)
        {
            try
            {
                var result = _libraryService.IssueBook(
                request,
                GetCompanyId(),
                GetUserId());

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            
        }

        /// <summary>
        /// Records the return of a previously issued book.
        /// </summary>
        /// <param name="request">Book return information.</param>
        /// <returns>A response indicating whether the return was recorded successfully.</returns>
        [HttpPost("ReturnBook")]
        public IActionResult ReturnBook([FromBody] ReturnBookIssue req)
        {
            try
            {
                var result = _libraryService.ReturnBook(
               req.issueId,
               req.returnDate,
               GetCompanyId(),
               GetUserId());

                return Ok(new ApiResponse<bool>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Success
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
           
        }

        /// <summary>
        /// Retrieves detailed information for a library member.
        /// </summary>
        /// <param name="memberId">Library member identifier.</param>
        /// <param name="companyId">Company identifier.</param>
        /// <returns>Member information including card number and contact details.</returns>
        [HttpGet("GetMemberDetails")]
        public IActionResult GetMemberDetails(int memberId, int companyId)
        {
            try
            {
                var result = _libraryService.GetMemberDetails(memberId, companyId);

                return Ok(new ApiResponse<MemberDetailsViewModel>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<MemberDetailsViewModel>
                {
                    Success = false,
                    Message=ex.Message
                });
            }
            
        }

        /// <summary>
        /// Deletes a library member.
        /// </summary>
        /// <param name="id">Library member identifier.</param>
        /// <param name="studentId">Student identifier.</param>
        /// <param name="staffId">Staff identifier.</param>
        /// <returns>Operation result.</returns>
        [HttpDelete]
        [Route("DeleteMemberEx")]
        public IActionResult DeleteMemberEx(
            int id,
            int? studentId,
            int? staffId, string? modeType)
        {
            try
            {
                int userId = GetUserId();

                var result = _libraryService.DeleteMemberEx(
                    id,
                    studentId,
                    staffId,
                    GetCompanyId(),
                    userId, modeType);

                return Ok(new ApiResponse<object>
                {
                    Success = result.Success,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }


        /// <summary>
        /// Retrieves a paginated list of Libary based on the specified search criteria.
        /// Automatically resolves the current user's Company if they are not provided.
        /// </summary>
        [HttpPost("GetAllLibraryWithPage")]
        public async Task<IActionResult> GetAllLibraryWithPage([FromBody] LibrarySearchRequest request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<BookViewModel>>.SuccessResponse(new List<BookViewModel>()));

                var data = await _libraryService.GetAllLibraryWithPage(request);
                return Ok(ApiResponse<PagedResult<BookViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves a paginated list of student library memberships based on the search criteria.
        /// </summary>
        /// <param name="request">The search filter and pagination options.</param>
        /// <returns>A paginated list of student library memberships.</returns>
        [HttpPost("GetAllStudentsMembershipWithPage")]
        public async Task<IActionResult> GetAllStudentsMembershipWithPage([FromBody] StudentsMembershipSearchRequest request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<LibraryMemberViewModel>>.SuccessResponse(new List<LibraryMemberViewModel>()));

                var data = await _libraryService.GetAllStudentsMembershipWithPage(request);
                return Ok(ApiResponse<PagedResult<LibraryMemberViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves details of a library member by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the library member.</param>
        /// <returns>The library member details.</returns>
        [HttpGet("GetLibraryMemberByID/{id}")]
        public IActionResult GetLibraryMemberByID(int id)
        {
            try
            {
                var data = _libraryService.GetLibraryMemberByID(id);
                if (data == null) return NotFound(ApiResponse<LibraryMember>.ErrorResponse("Class not found"));
                return Ok(ApiResponse<LibraryMember>.SuccessResponse(data));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message }); 
            }
            
        }

        /// <summary>
        /// Retrieves a paginated list of staff library memberships based on the search criteria.
        /// </summary>
        /// <param name="request">The search filter and pagination options.</param>
        /// <returns>A paginated list of staff library memberships.</returns>
        [HttpPost("GetAllStaffMembershipWithPage")]
        public async Task<IActionResult> GetAllStaffMembershipWithPage([FromBody] StaffLibraryMemberSearchModel request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<StaffLibraryMember>>.SuccessResponse(new List<StaffLibraryMember>()));

                var data = await _libraryService.GetAllStaffMembershipWithPage(request);
                return Ok(ApiResponse<PagedResult<StaffLibraryMember>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        /// <summary>
        /// Retrieves a paginated list of all issued books based on the search criteria.
        /// </summary>
        /// <param name="request">The search filter and pagination options.</param>
        /// <returns>A paginated list of issued books.</returns>
        [HttpPost("GetAllIssuedBooksWithPageIndex")]
        public async Task<IActionResult> GetAllIssuedBooksWithPageIndex([FromBody] IssueBookSearchModel request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<IssueReturnViewModel>>.SuccessResponse(new List<IssueReturnViewModel>()));

                var data = await _libraryService.GetAllIssuedBooksWithPageIndex(request);
                return Ok(ApiResponse<PagedResult<IssueReturnViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves a paginated list of library members based on the search criteria.
        /// </summary>
        /// <param name="request">The search filter and pagination options.</param>
        /// <returns>A paginated list of library members.</returns>
        [HttpPost("GetAllLibraryMemberWithPageIndex")]
        public async Task<IActionResult> GetAllLibraryMemberWithPageIndex([FromBody] MemberSearchModel request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<LibraryMemberViewModel>>.SuccessResponse(new List<LibraryMemberViewModel>()));

                var data = await _libraryService.GetAllLibraryMemberWithPageIndex(request);
                return Ok(ApiResponse<PagedResult<LibraryMemberViewModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        #region Bind AlL Library Master Table DropDwon 
        /// <summary>
        /// Retrieves a dropdown list of library document types.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of document type dropdown items.</returns>
        [HttpGet("GetLibraryDocumentTypeDropdownList")]
        public IActionResult GetLibraryDocumentTypeDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibraryDocumentTypeDropdownList(companyId, userId);

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
        /// <summary>
        /// Retrieves a dropdown list of library document statuses.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of document status dropdown items.</returns>
        [HttpGet("GetLibraryDocumentStatusDropdownList")]
        public IActionResult GetLibraryDocumentStatusDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibraryDocumentStatusDropdownList(companyId, userId);

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
        /// <summary>
        /// Retrieves a dropdown list of library categories.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of library category dropdown items.</returns>
        [HttpGet("GetLibraryCategoryDropdownList")]
        public IActionResult GetLibraryCategoryDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibraryCategoryDropdownList(companyId, userId);

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

        /// <summary>
        /// Retrieves a dropdown list of library languages.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of library language dropdown items.</returns>
        [HttpGet("GetLibraryLanguageDropdownList")]
        public IActionResult GetLibraryLanguageDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibraryLanguageDropdownList(companyId, userId);

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

        /// <summary>
        /// Retrieves a dropdown list of library series.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of library series dropdown items.</returns>
        [HttpGet("GetLibrarySeriesDropdownList")]
        public IActionResult GetLibrarySeriesDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibrarySeriesDropdownList(companyId, userId);

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

        /// <summary>
        /// Retrieves a dropdown list of library suppliers.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of library supplier dropdown items.</returns>
        [HttpGet("GetLibrarySupplierDropdownList")]
        public IActionResult GetLibrarySupplierDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibrarySupplierDropdownList(companyId, userId);

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

        /// <summary>
        /// Retrieves a dropdown list of library budgets.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of library budget dropdown items.</returns>
        [HttpGet("GetLibraryBudgetDropdownList")]
        public IActionResult GetLibraryBudgetDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibraryBudgetDropdownList(companyId, userId);

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

        /// <summary>
        /// Retrieves a dropdown list of library publishers.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of library publisher dropdown items.</returns>
        [HttpGet("GetLibraryPublisherDropdownList")]
        public IActionResult GetLibraryPublisherDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibraryPublisherDropdownList(companyId, userId);

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

        /// <summary>
        /// Retrieves a dropdown list of library subjects, optionally filtered by category.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="categoryId">The optional category identifier to filter by.</param>
        /// <returns>A list of library subject dropdown items.</returns>
        [HttpGet("GetLibrarySubjectDropdownList")]
        public IActionResult GetLibrarySubjectDropdownList(int companyId, int? categoryId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibrarySubjectDropdownList(companyId, categoryId, userId);

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

        /// <summary>
        /// Searches for book titles containing the specified term.
        /// </summary>
        /// <param name="term">The search term.</param>
        /// <returns>A list of matching book titles.</returns>
        [HttpGet("SearchBookTitle")]
        public async Task<IActionResult> SearchBookTitle([FromQuery] string term, int companyId)
        {
            try
            {
                if (term == null)
                    return BadRequest(new ApiResponse
                    {
                        Result = 0,
                        Message = "Valid search books title are required."
                    });

                var response = await _libraryService.SearchBookTitle(term, companyId);
                return Ok(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves a dropdown list of library authors.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <returns>A list of library author dropdown items.</returns>
        [HttpGet("GetLibraryAuthorDropdownList")]
        public IActionResult GetLibraryAuthorDropdownList(int companyId)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.GetLibraryAuthorDropdownList(companyId, userId);

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
        #endregion

        /// <summary>
        /// Creates or updates a book's front page attachment file.
        /// </summary>
        /// <param name="req">The request model containing book and attachment details.</param>
        /// <returns>The success status and message of the operation.</returns>
        [HttpPost("UpsertBooksFrontPageAttachmentFile")]
        public IActionResult UpsertBooksFrontPageAttachmentFile([FromBody] BooksAttachmentUpsertRequest req)
        {
            try
            {
                int userId = GetUserId();
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _libraryService.UpsertBooksFrontPageAttachmentFile(req, userId);
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }


        /// <summary>
        /// Retrieves a paginated list of accession records based on the search criteria.
        /// </summary>
        /// <param name="request">The search filter and pagination options.</param>
        /// <returns>A paginated list of accession records.</returns>
        [HttpPost("GetAllAccessionWithPage")]
        public async Task<IActionResult> GetAllAccessionWithPage([FromBody] LibrarySearchRequest request)
        {
            try
            {
                int userId = GetUserId();

                if (request.CompanyID == null)
                {
                    request.CompanyID = _companyService.GetUserCurrentCompany(userId) ?? 0;
                }
                if (request.CompanyID == 0)
                    return Ok(ApiResponse<List<AccessionDetailsModel>>.SuccessResponse(new List<AccessionDetailsModel>()));

                var data = await _libraryService.GetAllAccessionWithPage(request);
                return Ok(ApiResponse<PagedResult<AccessionDetailsModel>>.SuccessResponse(data));

            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves accession details by accession ID and company ID.
        /// </summary>
        /// <param name="id">The accession ID.</param>
        /// <param name="companyId">The company ID.</param>
        /// <returns>The accession details.</returns>
        [HttpGet("GetAccessionById")]
        public IActionResult GetAccessionById(int id,int companyId)
        {
            try
            {
                var result = _libraryService.GetAccessionById(id, companyId);

                return Ok(new ApiResponse<AccessionDetailsModel>
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

        /// <summary>
        /// Retrieves accession details by accession number and company ID.
        /// </summary>
        /// <param name="accessionNo">The accession number.</param>
        /// <param name="companyid">The company ID.</param>
        /// <returns>The accession details.</returns>
        [HttpGet("GetAccessionByNo")]
        public IActionResult GetAccessionByNo(string accessionNo, int companyid)
        {
            try
            {
                var result = _libraryService.GetAccessionByNo(accessionNo, companyid);

                return Ok(new ApiResponse<AccessionDetailsModel>
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

        /// <summary>
        /// Retrieves or generates the next issue number for the given company.
        /// </summary>
        /// <param name="companyid">The company ID.</param>
        /// <returns>The generated/retrieved issue number.</returns>
        [HttpGet("GetIssueNo")]
        public async Task<IActionResult> GetIssueNo(int companyid)
        {
            try
            {
                var result =await _libraryService.GetIssueNo(companyid);

                return Ok(new ApiResponse<IssueNoResponse>
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

        /// <summary>
        /// Creates or updates an accession status.
        /// </summary>
        /// <param name="req">The accession status details to save.</param>
        /// <returns>An object indicating success status and result message.</returns>
        [HttpPost("UpsertAccessionStatus")]
        public IActionResult UpsertAccessionStatus([FromBody] AccessionStatusUpsertRequest req)
        {
            try
            {
                int userId = GetUserId();
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (success, message) = _libraryService.UpsertAccessionStatus(req, userId);                
                return Ok(new { success, message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Retrieves accession records matching label search criteria.
        /// </summary>
        /// <param name="req">The accession search request.</param>
        /// <returns>A list of accession records for label generation.</returns>
        [HttpPost("GetAccessionForLabels")]
        public async Task<IActionResult> GetAccessionForLabels(AccessionSearchRequest req)
        {
            try
            {
                var result =await _libraryService.GetAccessionForLabels(req);

                return Ok(new ApiResponse<List<AccessionDetailsModel>>
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

        /// <summary>
        /// Saves a book issue transaction.
        /// </summary>
        /// <param name="req">The book issue save request details.</param>
        /// <returns>A response indicating success or failure status.</returns>
        [HttpPost("SaveIssueBook")]
        public  IActionResult SaveIssueBook([FromBody] LibraryIssueSaveRequest req)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();
            if (req.CompanyID == null)
            {
                req.CompanyID = GetCompanyId();
            }
            var result = _libraryService.SaveIssueBook(req,userId);
            return Ok(new { result.success, result.message });
        }

        /// <summary>
        /// Marks an issued book as returned.
        /// </summary>
        /// <param name="req">The library return request details.</param>
        /// <returns>A response indicating success or failure status.</returns>
        [HttpPost("ReturnIssueBook")]
        public IActionResult ReturnIssueBook([FromBody] LibraryReturnRequest req)
        {
            var companyId = GetCompanyId();
            var userId = GetUserId();
            if (req.CompanyID == null)
            {
                req.CompanyID = GetCompanyId();
            }
            var result = _libraryService.ReturnIssueBook(req, userId);
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                result.success,
                result.message
            }));
        }

        /// <summary>
        /// Retrieves detailed information for a specific book including its issue history.
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns>The book details and transaction history if found.</returns>
        [HttpGet("GetBookDetails")]
        public async Task<IActionResult> GetBookDetails(int bookId)
        {
            try
            {
                var result = await _libraryService.GetBookDetails(bookId);
                if (result.Book == null)
                    return Ok(new { success = false, message = "Book not found." });
                //BookDetailsResult
                return Ok(new ApiResponse<BookDetailsResult>
                {
                    Success = true,
                    Data = result
                });
               // return Ok(new { success = true, data = result.Book, issueHistory = result.IssueHistory });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message=ex.Message});
            }
            
        }

        /// <summary>
        /// Registers multiple student library memberships in bulk with card details.
        /// </summary>
        /// <param name="req">The bulk membership request containing list of students.</param>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="userId">The identifier of the user performing the operation.</param>
        /// <returns>A response indicating success or failure status.</returns>
        [HttpPost("AddStudentsMembershipBulkWithCards")]
        public IActionResult AddStudentsMembershipBulkWithCards(
           [FromBody] BulkMembershipRequest req,
           int companyId,
           int userId)
        {
            try
            {
                var address = string.Empty;
                var result = _libraryService.AddStudentsMembershipBulkWithCards(
                    req,
                    companyId,
                    userId, address);

                return Ok(ApiResponse<object>.SuccessResponse(new
                {
                    result.success,
                    result.message
                }));
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Error: " + ex.Message });
            }
            
        }


        /// <summary>
        /// Searches for library members matching criteria.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="memberType">The member type to search (Student/Staff/All).</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>A list of matching library members.</returns>
        [HttpGet("SearchMember")]
        public IActionResult SearchMember(int companyId, string memberType, string? searchText)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.SearchMember(companyId, memberType, searchText);

                return Ok(new ApiResponse<List<LibraryMemberSearchResult>>
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

        /// <summary>
        /// Searches for accession numbers matching the search criteria.
        /// </summary>
        /// <param name="companyId">The company identifier.</param>
        /// <param name="searchText">The search text.</param>
        /// <returns>A list of matching accession records.</returns>
        [HttpGet("SearchAccessionNo")]
        public IActionResult SearchAccessionNo(int companyId, string? searchText)
        {
            try
            {
                int userId = GetUserId();
                var result = _libraryService.SearchAccessionNo(companyId, searchText);

                return Ok(new ApiResponse<List<AccessionDetailsModel>>
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
    }
}
