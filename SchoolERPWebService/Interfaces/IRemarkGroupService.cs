using SchoolERPWebService.Domain.Common;
using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IRemarkGroupService
    {
        Task<PagedResult<RemarkGroupViewModel>> GetAllRemarkGroupWithPage(SubjectSearchRequest req);
        (bool success, string message) UpsertRemarkGroup(RemarkGroupRequest request);
        RemarkGroupViewModel? GetRemarkGroupByID(int remarkGroupID);
        (bool success, string message) DeleteRemarkGroup(List<int> remarkGroupID, int userId, string ipAddress);
        (bool success, string message) ToggleRemarkGroupStatus(StatusUpdateRequest request);
        List<DropdownModel> GetRemarkGroupDropdownList(int companyID, int sessionID, int userId);
    }
}
