using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IPromotedClassService
    {
        (bool success, string message) UpsertPromotedToClassAsync(PromotedToClassRequest request);
        (bool success, string message) DeletePromotedToClassAsync(List<int> promotionID, int userId, string ipAddress);
        Task<List<PromotedClassViewModel>> GetPromotedToClassAsync(PromotedToClassGetAllRequest request);
    }
}
