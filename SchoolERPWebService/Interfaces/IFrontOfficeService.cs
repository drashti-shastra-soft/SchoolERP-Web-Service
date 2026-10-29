using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    /// <summary>
    /// Interface for managing Front Office operations like complaints, postal records, and call logs.
    /// </summary>
    public interface IFrontOfficeService
    {
        // --- Purpose ---

        /// <summary>
        /// Retrieves a list of all visit purposes (e.g., 'Inquiry', 'Admission') from the database.
        /// </summary>
        List<MstFOPurposeViewModel> GetAllPurposes(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific visit purpose using its unique ID.
        /// </summary>
        MstFOPurposeViewModel? GetPurposeByID(int id);

        /// <summary>
        /// Adds a new visit purpose or updates an existing one in the database.
        /// </summary>
        (bool success, string message) UpsertPurpose(MstFOPurposeUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a visit purpose from the system.
        /// </summary>
        (bool success, string message) DeletePurpose(List<int> id, int userId);

        /// <summary>
        /// Turns a visit purpose's active status on or off.
        /// </summary>
        (bool success, string message) TogglePurposeStatus(StatusUpdateRequest request);

        // --- Complaint Type ---

        /// <summary>
        /// Retrieves a list of all complaint categories (e.g., 'Staff', 'Facility') from the database.
        /// </summary>
        List<MstFOComplaintTypeViewModel> GetAllComplaintTypes(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific complaint type using its unique ID.
        /// </summary>
        MstFOComplaintTypeViewModel? GetComplaintTypeByID(int id);

        /// <summary>
        /// Adds a new complaint type or updates an existing one in the database.
        /// </summary>
        (bool success, string message) UpsertComplaintType(MstFOComplaintTypeUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a complaint type from the system.
        /// </summary>
        (bool success, string message) DeleteComplaintType(List<int> id, int userId);

        /// <summary>
        /// Turns a complaint type's active status on or off.
        /// </summary>
        (bool success, string message) ToggleComplaintTypeStatus(StatusUpdateRequest request);

        // --- Source ---

        /// <summary>
        /// Retrieves a list of all inquiry sources (e.g., 'Online', 'Advertisement') from the database.
        /// </summary>
        List<MstFOSourceViewModel> GetAllSources(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific inquiry source using its unique ID.
        /// </summary>
        MstFOSourceViewModel? GetSourceByID(int id);

        /// <summary>
        /// Adds a new inquiry source or updates an existing one in the database.
        /// </summary>
        (bool success, string message) UpsertSource(MstFOSourceUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes an inquiry source from the system.
        /// </summary>
        (bool success, string message) DeleteSource(List<int> id, int userId);

        /// <summary>
        /// Turns an inquiry source's active status on or off.
        /// </summary>
        (bool success, string message) ToggleSourceStatus(StatusUpdateRequest request);

        // --- Reference ---

        /// <summary>
        /// Retrieves a list of all reference categories (e.g., 'Existing Student', 'Staff Member') from the database.
        /// </summary>
        List<MstFOReferenceViewModel> GetAllReferences(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific reference using its unique ID.
        /// </summary>
        MstFOReferenceViewModel? GetReferenceByID(int id);

        /// <summary>
        /// Adds a new reference or updates an existing one in the database.
        /// </summary>
        (bool success, string message) UpsertReference(MstFOReferenceUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a reference from the system.
        /// </summary>
        (bool success, string message) DeleteReference(List<int> id, int userId);

        /// <summary>
        /// Turns a reference's active status on or off.
        /// </summary>
        (bool success, string message) ToggleReferenceStatus(StatusUpdateRequest request);

        // --- Complaint ---

        /// <summary>
        /// Retrieves a list of all registered complaints from the database.
        /// </summary>
        List<FOComplaintViewModel> GetAllComplaints(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Finds and returns the details of a specific complaint using its unique ID.
        /// </summary>
        FOComplaintViewModel? GetComplaintByID(int id);

        /// <summary>
        /// Adds a new complaint or updates an existing one, including details like who made it and who it is assigned to.
        /// </summary>
        SpComplaint UpsertComplaint(FOComplaintUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a complaint record from the system.
        /// </summary>
        (bool success, string message) DeleteComplaint(List<int> id, int userId);

        /// <summary>
        /// Updates whether a complaint record is currently active or archived.
        /// </summary>
        (bool success, string message) ToggleComplaintStatus(StatusUpdateRequest request);

        // --- Postal Receive ---

        /// <summary>
        /// Retrieves a list of all incoming postal records from the database.
        /// </summary>
        List<FOPostalReceiveViewModel> GetAllPostalReceives(int companyId, int sessionId, bool includeDeleted = false);
        Task<PhoneCallLogChartResponse> GetPhoneCallLogChartDataAsync(int companyId, int sessionId);

        /// <summary>
        /// Finds and returns the details of a specific incoming postal record using its unique ID.
        /// </summary>
        FOPostalReceiveViewModel? GetPostalReceiveByID(int id);

        /// <summary>
        /// Adds a new incoming postal record or updates an existing one, including sender details and optional attachments.
        /// </summary>
        SpPostalReceive UpsertPostalReceive(FOPostalReceiveUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes an incoming postal record from the system.
        /// </summary>
        (bool success, string message) DeletePostalReceive(List<int> id, int userId);

        /// <summary>
        /// Updates whether an incoming postal record is currently active or archived.
        /// </summary>
        (bool success, string message) TogglePostalReceiveStatus(StatusUpdateRequest request);

        // --- Postal Dispatch ---

        /// <summary>
        /// Retrieves a list of all outgoing postal records from the database.
        /// </summary>
        List<FOPostalDispatchViewModel> GetAllPostalDispatches(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<FOPostalDispatchViewModel>> GetAllPostalDispatchesWithPageIndex(FOPostalDispatchSearchRequest req);
        /// <summary>
        /// Finds and returns the details of a specific outgoing postal record using its unique ID.
        /// </summary>
        FOPostalDispatchViewModel? GetPostalDispatchByID(int id);

        /// <summary>
        /// Adds a new outgoing postal record or updates an existing one, including recipient details and optional attachments.
        /// </summary>
        SpPostalDispatch UpsertPostalDispatch(FOPostalDispatchUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes an outgoing postal record from the system.
        /// </summary>
        (bool success, string message) DeletePostalDispatch(List<int> id, int userId);

        /// <summary>
        /// Updates whether an outgoing postal record is currently active or archived.
        /// </summary>
        (bool success, string message) TogglePostalDispatchStatus(StatusUpdateRequest request);
        (bool success, string message) UpsertPostalDispatchAttachmentFile(
     FOPostalDispatchAttachmentUpsertRequest req, int userId);
        // --- Phone Call Log ---


        /// <summary>
        /// Retrieves a list of all phone call records from the database.
        /// </summary>
        List<FOPhoneCallLogViewModel> GetAllPhoneCallLogs(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<FOPhoneCallLogViewModel>> GetAllPhoneCallLogsWithPage(FOPhoneCallLogSearchRequest req);
        /// <summary>
        /// Finds and returns the details of a specific phone call record using its unique ID.
        /// </summary>
        FOPhoneCallLogViewModel? GetPhoneCallLogByID(int id);

        /// <summary>
        /// Adds a new phone call record or updates an existing one, including caller details, call duration, and follow-up dates.
        /// </summary>
        (bool success, string message) UpsertPhoneCallLog(FOPhoneCallLogUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a phone call record from the system.
        /// </summary>
        (bool success, string message) DeletePhoneCallLog(List<int> id, int userId);

        /// <summary>
        /// Updates whether a phone call record is currently active or archived.
        /// </summary>
        (bool success, string message) TogglePhoneCallLogStatus(StatusUpdateRequest request);

        // --- Visitor Book ---

        /// <summary>
        /// Retrieves a list of all visitor book entries from the database.
        /// </summary>
        List<FOVisitorBookViewModel> GetAllVisitors(int companyId, int sessionId, bool includeDeleted = false);

        /// <summary>
        /// Retrieves a list of all visitor book entries from the database using pageindex.
        /// </summary>
        Task<PagedResult<FOVisitorBookViewModel>> GetAllVisitorsWithPageIndex(FOVisitorBookSerchRequest req);
        /// <summary>
        /// Finds and returns the details of a specific visitor book entry using its unique ID.
        /// </summary>
        FOVisitorBookViewModel? GetVisitorByID(int id);

        /// <summary>
        /// Adds a new visitor book entry or updates an existing one, including visitor details and optional attachments.
        /// </summary>
        SpVisitorResult UpsertVisitor(FOVisitorBookUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes a visitor book record from the system.
        /// </summary>
        (bool success, string message) DeleteVisitor(List<int> id, int userId);

        /// <summary>
        /// Updates whether a visitor book record is currently active or archived.
        /// </summary>
        (bool success, string message) ToggleVisitorStatus(StatusUpdateRequest request);

        // --- Admission Inquiry ---

        /// <summary>
        /// Retrieves a list of all admission inquiries from the database based on filters.
        /// </summary>
        List<FOAdmissionInquiryViewModel> GetAllAdmissionInquiries(int companyId, int sessionId, DateTime? fromDate = null, DateTime? toDate = null, int sourceId = 0, int classId = 0, string? status = null);

        /// <summary>
        /// Finds and returns the details of a specific admission inquiry, including its follow-up history.
        /// </summary>
        FOAdmissionInquiryViewModel? GetAdmissionInquiryByID(int id);

        /// <summary>
        /// Adds a new admission inquiry or updates an existing one.
        /// </summary>
        (bool success, string message) UpsertAdmissionInquiry(FOAdmissionInquiryUpsertRequest req, int companyId, int sessionId, int userId);

        /// <summary>
        /// Removes an admission inquiry record from the system.
        /// </summary>
        (bool success, string message) DeleteAdmissionInquiry(List<int> id, int userId);

        /// <summary>
        /// Saves a new follow-up for an inquiry.
        /// </summary>
        (bool success, string message) SaveInquiryFollowUp(FOInquiryFollowUpSaveRequest req, int userId);

        /// <summary>
        /// Retrieves a list of all admission inquiries from the database based on filters.
        /// </summary>
        Task<PagedResult<FOAdmissionInquiryViewModel>> GetAllAdmissionInquiriesWithPageIndex(EnquirySearchRequest req);
        /// <summary>
        /// Dasboard
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<InquiryStatusSummaryViewModel> GetInquiryStatusCount(InquiryStatusCountRequest req);

        public (bool success, string message) UpsertVisitorAttachmentFile(
        FOVisitorBookAttachmentUpsertRequest req,int userId);

        Task<PagedResult<FOPostalReceiveViewModel>> GetAllPostalReceiveWithPage(ClassSearchRequest req);

        (bool success, string message) UpsertPostalReceiveAttachmentFile(
   FOPostalReceiveAttachmentUpsertRequest req, int userId);

        Task<PagedResult<FOComplaintViewModel>> GetAllComplaintsWithPage(ComplaintSearchRequest req);

        Task<PagedResult<MstFOPurposeViewModel>> GetAllPurposesWithPage(ClassSearchRequest req);

        Task<PagedResult<MstFOComplaintTypeViewModel>> GetAllComplaintTypesWithPage(ClassSearchRequest req);
        Task<PagedResult<MstFOSourceViewModel>> GetAllSourceWithPage(ClassSearchRequest req);
        Task<PagedResult<MstFOReferenceViewModel>> GetAllReferenceWithPage(ClassSearchRequest req);
        (bool success, string message) UpsertComplaintAttachmentFile(FOComplaintAttachmentUpsertRequest req, int userId);

        Task<(bool Success, string Message)> CopyPurpose(CopyRequest req);
        Task<(bool Success, string Message)> CopyComplaintType(CopyRequest req);
        Task<(bool Success, string Message)> CopySource(CopyRequest req);
        Task<(bool Success, string Message)> CopyReference(CopyRequest req);
        List<DropdownModel> GetSetUpFOMasterDropdowm(int companyId, int sessionID, string type);
    }
}
