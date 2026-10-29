using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IChangeExamService
    {
        (bool success, string message) UpsertChangeExamAsync(ExamMappingRequest request);
        Task<List<StudentResponse>> GetChangeExamAsync(ChangeExamRequest request);
    }
}
