using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IStudentIDCardService
    {
        List<StudentIDCardViewModel> GetAll(int companyId, int sessionId);
        StudentIDCardViewModel GetByID(int id);
        (int Result, string Message) Upsert(StudentIDCardUpsertRequest request, int userId, int companyId, int sessionId);
        (int Result, string Message) Delete(List<int> id, int userId);
        //(int Result, string Message) ToggleStatus(int id, bool isActive, int userId);
        (bool success, string message) ToggleStudentIDCardStatus(StatusUpdateRequest request);
        string GenerateIDCard(int studentId, int idCardId, int companyId, int sessionId);
    }
}
