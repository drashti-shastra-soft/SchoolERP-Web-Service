using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IExamMarksheetDownloadPermissionServices
    {
        Task<List<ExamMarksheetDownloadPermissionViewModel>> GetAllExamMarksheetDownloadPermission(ExamGroupGetAllRequest request);
        Task<ApiResponse> SavePermissionAsync(PermissionRequest request);
        Task<ExamMarksheetDownloadPermissionSetAccessResponse> SetAccessAsync(
    ExamMarksheetDownloadPermissionSetAccessRequest request);
        (bool success, string message) Delete(DeleteRequest request, int userId);
    }
}
