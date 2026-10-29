using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IPublisherService
    {
        Task<ApiResponse> UpsertPublisherAsync(PublisherRequest model);
        Task<PublisherModel?> GetPublisherByIdAsync(int publisherId);
        Task<PagedResult<PublisherModel>> GetAllPublishertWithPage(SearchRequest req);
        (bool success, string message) DeletePublisher(List<int> ids, int userId);
        (bool success, string message) TogglePublisherStatus(StatusUpdateRequest request);
        List<PublisherModel> GetAll(int companyId, bool? includdeleted);
        Task<(bool Success, string Message)> CopyLibraryPublisher(CopyForCompanyRequest req);
    }
}
