using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IPostalCodeService
    {
        Task<ApiResponse> UpsertPostalCodeAsync(PostalCodeEditModel model);
        Task<PostalCodeListViewModel?> GetPostalCodeByIdAsync(int postalCodeId);
        Task<List<PostalCodeListViewModel>> GetAllPostalCodeAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<PostalCodeListViewModel>> GetAllPostalCodeyWithPage(SerachPostalCode req);
        (bool success, string message) DeletePostalCodey(List<int> ids, int userId);
        (bool success, string message) TogglePostalCodeyStatus(StatusUpdateRequest request);
        Task<List<PostalCodeListViewModel>> SearchPostalCode(string term);
    }
}
