using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IRemarkMasterService
    {
        (bool success, string message) UpsertRemark(RemarkMasterRequest request);
        Task<PagedResult<RemarkMasterViewModel>> GetAllRemarkWithPage(SubjectSearchRequest req);
        RemarkMasterViewModel? GetRemarkByID(int remarkGroupID);
        (bool success, string message) DeleteRemark(List<int> remarkID, int userId, string ipAddress);
        (bool success, string message) ToggleRemarkStatus(StatusUpdateRequest request);
    }
}
