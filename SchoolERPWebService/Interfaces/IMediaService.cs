using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IMediaService
    {
        Task<ApiResponse> Upsert(
           MediaRequest request,
           int userId);
        Task<PagedResult<MediaViewModel>> GetAllMediaWithPage(SearchRequest request);
    }
}
