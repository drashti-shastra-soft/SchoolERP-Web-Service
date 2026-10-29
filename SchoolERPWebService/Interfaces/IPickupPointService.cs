using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IPickupPointService
    {
        List<PickupPointViewModel> GetAllPickupPoints(int companyId, int sessionId);
        PickupPointViewModel? GetPickupPointByID(int id);
        (bool Success, string Message) UpsertPickupPoint(PickupPointUpsertRequest req, int companyId, int sessionId, int userId);
        (bool Success, string Message) DeletePickupPoint(List<int> id, int userId);
        //(bool Success, string Message) TogglePickupPointStatus(int id, bool isActive, int userId);
        (bool Success, string Message) TogglePickupPointStatus(StatusUpdateRequest request);
    }
}
