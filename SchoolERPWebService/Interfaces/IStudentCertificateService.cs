using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IStudentCertificateService
    {
        List<StudentCertificateViewModel> GetAll(int companyId, int sessionId);
        StudentCertificateViewModel GetByID(int id);
        (bool success, string message) Upsert(StudentCertificateUpsertRequest request, int userId, int companyId, int sessionId);
        (bool success, string message) Delete(List<int> id, int userId);
        (bool success, string message) ToggleStatus(StatusUpdateRequest request);
        string GenerateCertificate(int studentId, int certificateId, int companyId, int sessionId);
    }
}
