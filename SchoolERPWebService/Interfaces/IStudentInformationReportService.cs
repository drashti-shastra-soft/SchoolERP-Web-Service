using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;
using System.Threading.Tasks;

namespace SchoolERPWebService.Interfaces
{
    public interface IStudentInformationReportService
    {
        Task<PagedResult<StudentInformationReportModel>> GetStudentReport(StudentReportRequest req);
        Task<PagedResult<StudentCountReportResponse>> GETStudentCountReport(StudentReportRequest req);
        Task<PagedResult<GuardianReportResponse>> GETStudentGuardianReport(StudentReportRequest req);
        Task<PagedResult<LoginCredentialReportResponse>> GETLoginCredentialReport(StudentReportRequest req);
        Task<PagedResult<AdmissionReportResponse>> GETAdmissionReport(StudentReportRequest req);
        Task<PagedResult<StudentGenderRatioReportResponse>> GetStudentGenderRatioReportReport(StudentReportRequest req);
        Task<PagedResult<TimeTableClassSubjectReportResponse>> GetClassSubjectReport(StudentReportRequest req);
        Task<PagedResult<StudentTeacherRatioReportResponse>> GetStudentTeacherRatioReport(StudentReportRequest req);
        Task<PagedResult<StudentHostelReportResponse>> GetStudentHostelReport(StudentReportRequest req);
        Task<PagedResult<StaffReportResponse>> GetStaffReport(StudentReportRequest req);
        Task<PagedResult<ItemStockReportResponse>> GetStockReport(StudentReportRequest req);
        Task<PagedResult<AddItemResponse>> GetAddItemReport(StudentReportRequest req);
        Task<PagedResult<ItemIssueResponse>> GetIssueItemReport(StudentReportRequest req);
    }
}
