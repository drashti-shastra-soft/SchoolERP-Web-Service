using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IRoutePickupPointService
    {
        List<RoutePickupPointViewModel> GetAllRoutePickupPoints(int companyId, int sessionId);
        RoutePickupPointViewModel? GetRoutePickupPointByID(int id);
        (bool Success, string Message) UpsertRoutePickupPoint(RoutePickupPointUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeleteRoutePickupPoint(List<int> id, int userId);
        //(bool Success, string Message) ToggleRoutePickupPointStatus(int id, bool isActive, int userId);
        (bool success, string message) ToggleRoutePickupPointStatus(StatusUpdateRequest request);
    }
}
