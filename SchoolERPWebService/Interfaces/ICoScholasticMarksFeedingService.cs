using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface ICoScholasticMarksFeedingService
    {
        (bool success, string message) UpsertCoScholasticMarksFeeding(CoScholasticMarksFeedingRequest request);
        Task<List<CoScholasticMarksFeedingResponse>> GetAllCoScholasticMarksFeedingAsync(CoScholasticMarksFeedingGetRequest Req);
        (bool success, string message) DeleteMarksFeeding(List<int> coScholasticMarksFeedingID, int userId, string ipAddress);
    }
}
