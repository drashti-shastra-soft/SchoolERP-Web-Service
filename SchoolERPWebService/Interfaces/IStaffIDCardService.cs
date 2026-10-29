using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IStaffIDCardService
    {
        List<StaffIDCardViewModel> GetAll(int companyId, int sessionId);
        StaffIDCardViewModel GetByID(int id);
        (int Result, string Message) Upsert(StaffIDCardUpsertRequest request, int userId, int companyId, int sessionId);
        (int Result, string Message) Delete(List<int> id, int userId);
        (bool success, string message) ToggleStaffIDCardStatus(StatusUpdateRequest request);
        //(int Result, string Message) ToggleStatus(int id, bool isActive, int userId);
    }
}
