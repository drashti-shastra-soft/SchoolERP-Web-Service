using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IStudentInformationService
    {
        List<StudentDisableReasonViewModel> GetAllDisableReasons(int companyId, int sessionId, bool? IncludeIsActive = null);
        (bool Success, string Message) UpsertDisableReason(StudentDisableReasonUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteDisableReason(List<int> ids, int userId);

        List<StudentHouseViewModel> GetAllStudentHouses(int companyId, int sessionId, bool? IncludeIsActive = null);
        (bool Success, string Message) UpsertStudentHouse(StudentHouseUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteStudentHouse(List<int> ids, int userId);

        List<StudentCategoryViewModel> GetAllStudentCategories(int companyId, int sessionId, bool? includeDeleted = null);
        (bool Success, string Message) UpsertStudentCategory(StudentCategoryUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteStudentCategory(List<int> ids, int userId);
        string GetNewStudentRollNo(int companyId, int sessionId, Dictionary<string, string> dynamicValues = null);
        string GetNextSimpleAdmissionNo(int companyId);
        (bool Success, string Message, int StudentID) UpsertStudentAdmission(StudentAdmissionUpsertRequest req, int companyId, int sessionId, int userId);
        StudentDetailsViewModel GetStudentDetails(int studentId, int companyId, int sessionId);

        Task<PagedResult<StudentListViewModel>> GetStudentList(
            int companyId, int sessionId, int? classId, int? sectionId, string? searchTerm, int PageNumber, int PageSize, int? studentId);


        (bool Success, string Message) ToggleStudentStatus(StudentStatusToggleRequest req, int userId);
        List<MultiClassStudentCardViewModel> GetMultiClassStudents(int companyId, int sessionId, int? classId, int? sectionId, string? searchTerm);
        List<StudentListViewModel> GetDisabledStudentList(int companyId, int sessionId, int? classId, int? sectionId, string? searchTerm);
        (bool Success, string Message) UpsertStudentMultiClass(StudentMultiClassUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteStudentMultiClass(int id, int userId);
        (bool Success, string Message) BulkDeleteStudents(List<int> studentIds, int userId);
        (bool Success, string Message) DeleteStudent(int id, int userId);

        // Timeline
        List<StudentTimelineViewModel> GetStudentTimeline(int studentId);
        SpStudentTimeline UpsertStudentTimeline(StudentTimelineUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteStudentTimeline(int id, int userId);
        StudentTimelineDocumentModel GetStudentTimelineDocument(int id);

        StudentCategoryViewModel? GetStudentCategoryById(
            int studentCategoryId,
            int companyId,
            int sessionId,
            int? userId = null);

        StudentHouseViewModel? GetStudentHouseById(
            int studentHouseId,
            int companyId,
            int sessionId,
            int? userId = null);

        StudentDisableReasonViewModel GetDisableReasonsByID(
            int companyId,
            int sessionId,  int disableReasonID, int? userID=null);

        Task<List<StudentListViewModel>> GetStudentCopyList(
            int companyId, int sessionId);

        Task<(bool Success, string Message)> CopyStudentsToSession(CopyRequest req);

        (bool Success, string Message) UpdateStudentProfile(ProfileRequest req);

        Task<PagedResult<StudentHouseViewModel>> GetStudentHouseList(SubjectSearchRequest req);

        Task<List<StudentDropDwonBindViewModel>> GetStudentBind(StudentDropDwonBindRequestModel req);

        List<StudentAddressDetailsModel> GetStudentAddressList(
            int companyId,
            int sessionId,
            int? studentId);

        Task<bool> SaveStudentDocumentsBulkAsync(List<StudentDocumentSaveModel> documents, int userId);

        Task<List<StudentDocumentModel>> GetStudentDocumentsByStudentId(int studentId);
        Task<List<StudentUserModel>> GetStudentLoginDetailsBind(int userId);
        List<DropdownModel> GetHostelRoomDropdownList(int companyId, int sessionID, int hostelID, int userId);

        Task<(bool Success, string Message)> BulkUpsertMultiClassAsync(
    int studentId, int companyId, int sessionId, int userId, string? ipAddress,
    List<MultiClassRowModel> rows);

        StudentDisableViewModel GetStudentDisable(int studentId, int companyId, int sessionId);

        (bool Success, string Message) UpsertStudentDisable(
            StudentDisableReq req,
            int userId);
        Task<PagedResult<StudentCategoryViewModel>> GetAllStudentCategoriesWithPage(StudentCategorySearch req);

        (bool, string) ToggleStudentCategoryStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyStudentCategory(CopyRequest req);
        (bool, string) ToggleStudentHouseStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyStudentHouse(CopyRequest req);
        (bool, string) ToggleDisableReasonStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyDisableReason(CopyRequest req);
        Task<PagedResult<StudentDisableReasonViewModel>> GetAllDisableReasonsWithPage(StudentCategorySearch req);
        StudentTimelineViewModel GetTimelineById(int id);
        (bool success, string message) UpsertStudentTimelineAttachmentFile(
  StudentTimelineAttachmentUpsertRequest req, int userId);

        (bool, string) StudentChangePassword(StatusChangePassRequest request);

        Task<List<StudentListViewModel>> GetStudentAllList(
            int companyId, int sessionId, int? classId, int? sectionId);


        Task<StudentFeeTemplateListResponse>
         GetStudentFeeTemplateListAsync(int studentId, int companyId, int sessionId);

        List<FeesSearchStudentResponse> GetAllFeesSearchStudent(
            int companyId, string? studentSearch);

        List<FeesSearchVoucherResponse> GetAllSearchVoucher(
           int companyId, string? studentSearch, int? voucherId);

        Task<List<int>> GetFilteredStudentIdsAsync(int companyId, string? search, int? classId, int? sectionId);
    }
}
