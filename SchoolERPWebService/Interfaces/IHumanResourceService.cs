using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing human resources, including staff, departments, designations, and leave types.
    /// </summary>
    public interface IHumanResourceService
    {
        // --- Designation ---

        /// <summary>
        /// Retrieves a list of all job designations (e.g., 'Teacher', 'Accountant') from the database.
        /// </summary>
        List<HRDesignationViewModel> GetAllDesignations(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific designation using its unique ID.
        /// </summary>
        HRDesignationViewModel? GetDesignationByID(int id);

        /// <summary>
        /// Adds a new designation or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertDesignation(HRDesignationUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a designation from the system.
        /// </summary>
        (bool Success, string Message) DeleteDesignation(List<int> id, int userId);

        /// <summary>
        /// Turns a designation's active status on or off.
        /// </summary>
        (bool success, string message) ToggleDesignationStatus(StatusUpdateRequest request);

        // --- Department ---

        /// <summary>
        /// Retrieves a list of all school departments (e.g., 'Academic', 'Admin') from the database.
        /// </summary>
        DepartmentListResponse GetAllDepartments(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific department using its unique ID.
        /// </summary>
        HRDepartmentViewModel? GetDepartmentByID(int id); 

        /// <summary>
        /// Adds a new department or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertDepartment(HRDepartmentUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a department from the system.
        /// </summary>
        (bool Success, string Message) DeleteDepartment(List<int> id, int userId);

        /// <summary>
        /// Turns a department's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleDepartmentStatus(StatusUpdateRequest request);

        // --- Leave Type ---

        /// <summary>
        /// Retrieves a list of all leave categories (e.g., 'Sick Leave', 'Casual Leave') from the database.
        /// </summary>
        List<HRLeaveTypeViewModel> GetAllLeaveTypes(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific leave type using its unique ID.
        /// </summary>
        HRLeaveTypeViewModel? GetLeaveTypeByID(int id);

        /// <summary>
        /// Adds a new leave type or updates an existing one in the database.
        /// </summary>
        (bool Success, string Message) UpsertLeaveType(HRLeaveTypeUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a leave type from the system.
        /// </summary>
        (bool Success, string Message) DeleteLeaveType(List<int> id, int userId);

        /// <summary>
        /// Turns a leave type's active status on or off.
        /// </summary>
        (bool Success, string Message) ToggleLeaveTypeStatus(StatusUpdateRequest request);

        // --- Staff ---

        /// <summary>
        /// Retrieves a complete list of all staff members from the database.
        /// </summary>
        List<HRStaffViewModel> GetAllStaff(int companyId, int sessionId, int? staffId);
        Task<PagedResult<HRStaffViewModel>> GetAllStaffWithPage(SearchHRStaffRequest req);
        Task<PagedResult<HRStaffViewModel>> GetAllDisableStaffWithPage(SearchHRStaffRequest req);
        /// <summary>
        /// Finds and returns the details of a specific staff member using its unique ID.
        /// </summary>
        HRStaffViewModel? GetStaffByID(int id);

        /// <summary>
        /// Adds a new staff member or updates an existing one, including personal details, documents, and system access.
        /// </summary>
        StaffUpsertDTO UpsertStaff(HRStaffUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a staff member from the system.
        /// </summary>
        (bool Success, string Message) DeleteStaff(List<int> id, int userId);

        /// <summary>
        /// Generates a unique code for a new staff member.
        /// </summary>
        string GetNewStaffCode(int companyId, int sessionId);

        /// <summary>
        /// Retrieves the binary content and metadata of a specific staff document.
        /// </summary>
        (byte[] Bytes, string FileName, string ContentType) GetStaffDocument(int staffId, string docType);

        // --- Attendance ---

        /// <summary>
        /// Retrieves staff members and their attendance status for a specific date and optional role.
        /// </summary>
        List<HRStaffAttendanceViewModel> GetStaffAttendance(int companyId, int sessionId, DateTime date, int? roleId);
        Task<PagedResult<HRStaffAttendanceViewModel>> GetAllStaffAttendanceWithPage(HRStaffAttendanceSearchRequest req);
        /// <summary>
        /// Saves or updates an attendance record for a staff member.
        /// </summary>
        (bool Success, string Message) SaveStaffAttendance(HRStaffAttendanceUpsertRequest req, int companyId, int sessionId, int userId);

        // --- Apply Leave ---

        /// <summary>
        /// Retrieves all leave applications for the current school and session.
        /// </summary>
        List<HRApplyLeaveViewModel> GetAllApplyLeave(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific leave application using its unique ID.
        /// </summary>
        HRApplyLeaveViewModel? GetApplyLeaveByID(int id);

        /// <summary>
        /// Submits a new leave application or updates an existing one.
        /// </summary>
        (bool Success, string Message) UpsertApplyLeave(HRApplyLeaveUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a leave application from the system.
        /// </summary>
        (bool Success, string Message) DeleteApplyLeave(List<int> id, int userId);

        /// <summary>
        /// Updates the status of a leave application (Approve/Disapprove) and records admin remarks.
        /// </summary>
        (bool Success, string Message) UpdateApplyLeaveStatus(HRApplyLeaveStatusUpdateRequest req, int userId);

        /// <summary>
        /// Retrieves the leave quota balance for a specific staff member and leave type.
        /// </summary>
        dynamic GetLeaveBalance(int staffId, int leaveTypeId, int companyId, int sessionId);

        /// <summary>
        /// Retrieves all leave type balances for a specific staff member.
        /// </summary>
        List<dynamic> GetStaffAllLeaveBalances(int staffId, int companyId, int sessionId);
        /// <summary>
        /// Retrieves the binary content and metadata of a specific leave application document.
        /// </summary>
        (byte[] Bytes, string FileName, string ContentType) GetApplyLeaveDocument(int id);

        // --- Payroll ---

        /// <summary>
        /// Retrieves payroll records for a specific month and year, optionally filtered by role.
        /// </summary>
        List<HRPayrollViewModel> GetAllPayroll(int companyId, int sessionId, int month, int year, int? roleId);

        /// <summary>
        /// Generates a payroll record for a staff member for a specific month/year.
        /// </summary>
        (bool Success, string Message) GeneratePayroll(HRPayrollGenerateRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Fetches staff details and attendance summary for payroll generation.
        /// </summary>
        HRPayrollGenerationViewModel GetPayrollGenerationData(int staffId, int month, int year, int companyId, int sessionId);

        /// <summary>
        /// Saves a detailed payroll record with individual earnings and deductions.
        /// </summary>
        (bool Success, string Message) SaveDetailedPayroll(HRPayrollSaveRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) MarkAsPaid(HRPayrollPaymentRequest req, int userId);
        HRPayrollDetailsViewModel GetPayrollDetails(int payrollId);
        List<HRPayrollViewModel> GetStaffPayroll(int staffId);
        List<HRApplyLeaveViewModel> GetStaffLeaves(int staffId);
        HRAttendanceHistoryViewModel GetStaffAttendanceHistory(int staffId, int year, int companyId);
        List<HRStaffTimelineViewModel> GetStaffTimeline(int staffId);
        HRStaffTimelineViewModel? GetTimelineByID(int id);
        SpStaffTimeline UpsertTimeline(HRStaffTimelineUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteTimeline(int id, int userId);
        (byte[] Bytes, string FileName, string ContentType) GetTimelineDocument(int id);
        (bool Success, string Message) ToggleStaffStatus(int staffId, bool isActive, int userId, DateTime? statusDate);
        (bool Success, string Message) UpdateProfile(
           int staffId,
           string PhotoDoc,
           int userId);

        Task<PagedResult<HRDepartmentViewModel>> GetAllDepartmentsWithPage(SearchRequest req);
        Task<PagedResult<HRDesignationViewModel>> GetAllDesignationsWithPage(SearchRequest req);
        Task<PagedResult<HRLeaveTypeViewModel>> GetAllLeaveTypesWithPage(SearchRequest req);
        Task<LeaveQuotaModel> GetLeaveBalance(int leaveTypeId, int staffId);
        Task<PagedResult<HRApplyLeaveViewModel>> GetAllApplyLeaveWithPage(SearchRequest req);
        BulkUpsertResult SaveStaffAttendanceBulk(
    HRStaffAttendanceUpsertRequestModel request,
    int userId,
    string ipAddress);

        Task<StaffAttendanceSummaryModel?> GetStaffAttendanceSummary(
           int companyId, int sessionId, int staffId, int? year = null, int? status = null);
        (bool success, string message) UpsertStaffTimelineAttachmentFile(
 StudentTimelineAttachmentUpsertRequest req, int userId);
    }
}
