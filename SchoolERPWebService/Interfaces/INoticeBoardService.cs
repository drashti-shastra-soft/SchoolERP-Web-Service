using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface INoticeBoardService
    {
        Task<NoticeBoardSaveResponse> UpsertNoticeBoardAsync(NoticeBoardRequestModel model);

        Task<NoticeBoardModel?> GetNoticeBoardByIdAsync(int noticeBoardId, int? companyId = null, int? sessionId = null);

        Task<PagedResult<NoticeBoardModel>> GetAllNoticeBoardWithPage(NoticeBoardSearchRequest req);

        (bool success, string message) DeleteNoticeBoard(List<int> ids, int userId, string? ipAddress = null);

        (bool success, string message) ToggleNoticeBoardStatus(StatusUpdateRequest request);

        Task<ApiResponse> UpdateNoticeBoardAttachmentAsync(NoticeBoardAttachmentRequest model);
    }
}
