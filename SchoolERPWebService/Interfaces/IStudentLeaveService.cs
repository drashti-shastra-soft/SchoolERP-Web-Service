using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IStudentLeaveService
    {
        /// <summary>
        /// Retrieves the list of student leave applications, optionally filtered by class,
        /// section, and status, for a given company.
        /// </summary>
        /// <param name="classId">Optional class ID to filter leave applications by.</param>
        /// <param name="sectionId">Optional section ID to filter leave applications by.</param>
        /// <param name="status">Optional leave status to filter by (e.g. Pending / Approved / Rejected).</param>
        /// <param name="companyId">Company context under which leave applications are retrieved.</param>
        /// <returns>A list of <see cref="StudentLeaveViewModel"/> matching the given filters.</returns>
        List<StudentLeaveViewModel> GetLeaveApplications(int? classId, int? sectionId, int? status, int companyId);

        /// <summary>
        /// Updates the approval/rejection status of a student leave application.
        /// </summary>
        /// <param name="leaveAppId">ID of the leave application whose status is being updated.</param>
        /// <param name="status">New status value to set (e.g. Approved / Rejected).</param>
        /// <param name="companyId">Company context for validation.</param>
        /// <param name="userId">ID of the user performing the status update (for audit/history).</param>
        /// <returns>
        /// A tuple indicating whether the update succeeded (<c>Success</c>) and a corresponding
        /// user-friendly <c>Message</c>.
        /// </returns>
        (bool Success, string Message) UpdateLeaveStatus(int leaveAppId, int status, int companyId, int userId);

        /// <summary>
        /// Creates a new student leave application, or updates an existing one when
        /// <see cref="StudentLeaveUpsertRequest.LeaveAppID"/> is provided.
        /// </summary>
        /// <param name="req">Request payload containing leave application details (student, dates, reason, attachment, etc.).</param>
        /// <param name="companyId">Company context under which the leave application is saved.</param>
        /// <param name="userId">ID of the user performing the create/update (for audit/history).</param>
        /// <returns>
        /// An <see cref="SpLeaveApplicationResult"/> containing the result flag, message, and
        /// the primary key (LeaveAppID) of the created/updated record.
        /// </returns>
        SpLeaveApplicationResult UpsertLeaveApplication(StudentLeaveUpsertRequest req, int companyId, int userId);

        /// <summary>
        /// Retrieves the stored attachment (file bytes, name, and content type) for a given
        /// student leave application.
        /// </summary>
        /// <param name="leaveAppId">ID of the leave application whose attachment is requested.</param>
        /// <param name="companyId">Company context for validation.</param>
        /// <returns>
        /// A tuple containing the attachment <c>Bytes</c>, <c>FileName</c>, and <c>ContentType</c>,
        /// or <c>null</c> values if no attachment exists.
        /// </returns>
        (byte[]? Bytes, string? FileName, string? ContentType) GetLeaveAttachment(int leaveAppId, int companyId);

        /// <summary>
        /// Retrieves a paged, searchable/filterable list of student leave applications.
        /// </summary>
        /// <param name="req">
        /// Search/paging request containing filters (class, section, status, keyword) along with
        /// page number and page size.
        /// </param>
        /// <returns>
        /// A <see cref="PagedResult{T}"/> of <see cref="StudentLeaveViewModel"/> along with paging
        /// metadata (total records, total pages, current page, page size).
        /// </returns>
        Task<PagedResult<StudentLeaveViewModel>> GetAllLeaveApplicationsWithPage(StudentLeaveSearchRequest req);

        /// <summary>
        /// Retrieves a single student leave application by its primary key.
        /// </summary>
        /// <param name="leaveAppId">ID of the leave application to retrieve.</param>
        /// <param name="companyId">Company context for validation.</param>
        /// <returns>
        /// The matching <see cref="StudentLeaveViewModel"/>, or <c>null</c> if no record is found.
        /// </returns>
        StudentLeaveViewModel? GetLeaveApplicationsById(int? leaveAppId, int? companyId);

        /// <summary>
        /// Updates/replaces the attachment file for an existing student leave application
        /// without affecting the other leave application fields.
        /// </summary>
        /// <param name="req">
        /// Request payload containing the leave application ID and the new attachment
        /// (file data, type, and name).
        /// </param>
        /// <param name="userId">ID of the user performing the update (for audit/history).</param>
        /// <returns>
        /// A tuple indicating whether the update succeeded (<c>success</c>) and a corresponding
        /// user-friendly <c>message</c>.
        /// </returns>
        (bool success, string message) UpsertLeaveApplicationAttachmentFile(
            LeaveApplicationAttachmentUpsertRequest req, int userId);

        /// <summary>
        /// Deletes the specified Leave Application 
        /// </summary>
        (bool Success, string Message) DeleteLeaveApplication(
            List<int> ids,
            int userId, int companyId);
    }
}
