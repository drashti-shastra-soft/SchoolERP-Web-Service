using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ICoScholasticService
    {
        Task<PagedResult<CoScholasticViewModel>> GetAllCoScholasticWithPage(CoScholasticSearchRequest request);
        (bool success, string message) UpsertExam(CoScholasticRequest request);
        CoScholasticViewModel? GetCoScholasticByID(int coScholasticID);
        Task<List<CoScholasticViewModel>> GetAllCoScholasticAsync(int companyId, bool? isActive = false);
        (bool success, string message) DeleteCoScholastic(List<int> coScholasticID, int userId, string ipAddress);
        (bool success, string message) ToggleCoScholasticStatus(StatusUpdateRequest request);
    }
}
