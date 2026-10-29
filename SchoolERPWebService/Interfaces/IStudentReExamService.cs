using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IStudentReExamService
    {
        Task<List<StudentReExamViewModel>> GetStudentReExamAsync(StudentReExamRequestModel request);
        (bool success, string message) SaveStudentReExamAsync(
      StudentReExamRequest request);
        (bool success, string message) DeleteStudentReExamAsync(List<int> reExamID, int userId, string ipAddress);
    }
}
