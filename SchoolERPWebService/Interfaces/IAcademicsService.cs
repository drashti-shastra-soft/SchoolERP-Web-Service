using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Defines server-side operations for managing academic schedules, class teachers, and student promotions.
    /// Provides methods for handling timetables, class teacher assignments, and student promotion workflows.
    /// </summary>
    public interface IAcademicsService
    {
        /// <summary>
        /// Retrieves the timetable records for a specific class and section.
        /// Use this to get all scheduled periods for a class-section combination.
        /// </summary>
        List<TimeTableViewModel> GetTimeTableByClass(int companyId, int sessionId, int classId, int sectionId);

        /// <summary>
        /// Retrieves the timetable entries for a specific staff member, filtered by company and session.
        /// Use this to get all periods assigned to a staff member.
        /// </summary>
        List<TimeTableViewModel> GetTimeTableByStaff(TimeTableSearchRequest request);

        /// <summary>
        /// Inserts or updates a timetable record.
        /// Use this to add or edit a timetable entry for a class, section, subject, or staff.
        /// </summary>
        (bool success, string message) UpsertTimeTable(TimeTableUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Deletes a timetable slot by its unique ID.
        /// Use this to remove a specific period from the timetable.
        /// </summary>
        (bool success, string message) DeleteTimeTableSlot(int id, int userId);

        /// <summary>
        /// Retrieves all class teacher assignments for the specified company and session.
        /// Use this to get which staff are assigned as class teachers for multiple classes/sections.
        /// </summary>
        List<ClassTeacherViewModel> GetAllClassTeachers(int companyId, int sessionId, List<int> classId = null, List<int> sectionId = null);

        /// <summary>
        /// Creates or updates a class teacher assignment.
        /// Use this to assign or update staff as class teachers for classes/sections.
        /// </summary>
        (bool success, string message) UpsertClassTeacher(ClassTeacherUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Deletes one or more class teacher assignments by their IDs.
        /// Use this to remove class teacher assignments.
        /// </summary>
        (bool success, string message) DeleteClassTeacher(List<int> id, int userId);

        /// <summary>
        /// Retrieves a list of students eligible for promotion based on company, session, class, and section.
        /// Use this to get the list of students for the promotion process.
        /// </summary>
        List<StudentPromotionViewModel> GetStudentsForPromotion(int companyId, int sessionId, int classId, int sectionId);

        /// <summary>
        /// Promotes students to the next class or section.
        /// Use this to finalize and save student promotions.
        /// </summary>
        (bool success, string message) PromoteStudents(PromotionRequest req, int companyId, int userId);

        /// <summary>
        /// Retrieves a paginated list of class teacher assignments based on search and filter criteria.
        /// Use this for displaying assignments in a paged grid or table.
        /// </summary>
        Task<PagedResult<ClassTeacherViewModel>> GetAllClassTeachersWithPage(AcademicsSearchRequest req);

        /// <summary>
        /// Retrieves a paginated list of students eligible for promotion, based on search and filter criteria.
        /// Use this for displaying students in a paged grid or table during the promotion process.
        /// </summary>
        Task<PagedResult<StudentPromotionViewModel>> GetForPromotionPageIndex(SearchPromotedStudent req);

        /// <summary>
        /// Retrieves a paginated list of students eligible for promotion, based on search and filter criteria.
        /// Use this for displaying students in a paged grid or table during the promotion process.
        /// </summary>
        Task<PagedResult<StudentDemotionViewModel>> GetForDemotionPageIndex(SearchPromotedStudent req);

        /// <summary>
        /// Retrieves a timetable slot by its unique ID.
        /// Use this to fetch details of a specific timetable entry for viewing or editing.
        /// </summary>
        TimeTableViewModel? GetTimeTableById(int companyId, int sessionId, int timeTableID);

        /// <summary>
        /// Toggles the active status (enable/disable) of one or more class teacher assignments.
        /// Use this to activate or deactivate class teacher assignments without deleting them.
        /// </summary>
        (bool success, string message) ToggleAcademicsClassTeacherStatus(StatusUpdateRequest request);

        /// <summary>
        /// Inserts or updates multiple timetable records in bulk.
        /// Use this to efficiently add or update several timetable entries at once (e.g., for a whole class or section).
        /// </summary>
        (bool success, string message) UpsertBulkTimeTable(BulkTimeTableUpsertRequest req, int userId);

        (bool success, DemoteResponse response) DemoteStudents(
            DemotionRequest req, int userId, string? ipAddress = null);
    }
}
