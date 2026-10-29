using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface ILibraryService
    {
        List<BookViewModel> GetBookList(int companyId, string? searchTerm);
        BookViewModel GetBookById(int id);
        Task<SpBooksResult> UpsertBook(BookUpsertRequest req, int companyId, int userId);
        (bool Success, string Message) DeleteBook(List<int> id, int userId);
        (bool Success, string Message) ToggleBookStatus(StatusUpdateRequest request);
        /// <summary>
        /// Retrieves a paginated list of classes based on the specified search criteria.
        /// Automatically resolves the current user's Company and Session if they are not provided.
        /// </summary>
        Task<PagedResult<BookViewModel>> GetAllLibraryWithPage(LibrarySearchRequest req);

        // Membership
        List<LibraryMemberViewModel> GetMemberList(string memberType, int companyId, int? classId, int? sectionId, int? departmentId, string? search);
        List<MembershipSearchViewModel> SearchForMembership(string memberType, int companyId, int? classId, int? sectionId, int? departmentId, string? search);
        (bool Success, string Message) AddMember(LibraryMemberUpsertRequest req, int companyId, int userId);
        (bool Success, string Message) DeleteMember(int id, int userId);

        // Issue/Return
        List<IssueReturnViewModel> GetIssuedBooks(int memberId, int companyId);
        (bool Success, string Message) IssueBook(IssueReturnUpsertRequest req, int companyId, int userId);
        (bool Success, string Message) ReturnBook(int issueId, DateTime returnDate, int companyId, int userId);
        MemberDetailsViewModel GetMemberDetails(int memberId, int companyId);
        (bool Success, string Message) DeleteMemberEx(int id, int? studentId, int? staffId, int? companyId, int userId, string? modeType);

        Task<PagedResult<LibraryMemberViewModel>> GetAllStudentsMembershipWithPage(StudentsMembershipSearchRequest req);
        LibraryMember? GetLibraryMemberByID(int libraryMemberID);

        Task<PagedResult<StaffLibraryMember>> GetAllStaffMembershipWithPage(StaffLibraryMemberSearchModel req);
        Task<PagedResult<IssueReturnViewModel>> GetAllIssuedBooksWithPageIndex(IssueBookSearchModel req);
        Task<PagedResult<LibraryMemberViewModel>> GetAllLibraryMemberWithPageIndex(MemberSearchModel req);


        #region All Libary Master Table Dropdwonbind
        List<DropdownModel> GetLibraryDocumentTypeDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibraryDocumentStatusDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibraryCategoryDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibraryLanguageDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibrarySeriesDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibrarySupplierDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibraryBudgetDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibraryPublisherDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibraryAuthorDropdownList(int companyId, int userId);
        List<DropdownModel> GetLibrarySubjectDropdownList(int companyId, int? categoryId, int userId);
        Task<List<BookViewModel>> SearchBookTitle(string term, int companyId);
        #endregion

        (bool success, string message) UpsertBooksFrontPageAttachmentFile(
     BooksAttachmentUpsertRequest req, int userId);

        Task<PagedResult<AccessionDetailsModel>> GetAllAccessionWithPage(LibrarySearchRequest req);
        AccessionDetailsModel? GetAccessionById(int id, int companyid);
        AccessionDetailsModel? GetAccessionByNo(string accessionNo, int companyid);
        (bool success, string message) UpsertAccessionStatus(
     AccessionStatusUpsertRequest req, int userId);
        Task<List<AccessionDetailsModel>> GetAccessionForLabels(AccessionSearchRequest req);
        Task<IssueNoResponse> GetIssueNo(int companyID);
        public (bool success, string message) SaveIssueBook(
    LibraryIssueSaveRequest req, int userId);
        public (bool success, string message) ReturnIssueBook(
    LibraryReturnRequest req, int userId);
        Task<BookDetailsResult> GetBookDetails(int bookId);
        (bool success, string message, List<IssuedCard> issuedCards) AddStudentsMembershipBulkWithCards(
            BulkMembershipRequest request,
            int companyId,
            int userId,
            string? ipAddress);

        List<LibraryMemberSearchResult> SearchMember(int companyId, string memberType, string? searchText);
        List<AccessionDetailsModel> SearchAccessionNo(int companyId, string? searchText);
    }
}
