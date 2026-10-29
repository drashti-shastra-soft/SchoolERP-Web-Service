using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IDownloadCenterService
    {
        List<ContentTypeViewModel> GetContentTypeList(int companyId, string? searchTerm);
        ContentTypeViewModel GetContentTypeById(int id);
        (bool Success, string Message) UpsertContentType(ContentTypeUpsertRequest req, int companyId, int userId);
        (bool Success, string Message) DeleteContentType(List<int> id, int companyId, int userId);
        (bool Success, string Message) ToggleContentTypeStatus(StatusUpdateRequest request);
        Task<PagedResult<ContentTypeViewModel>> GetAllContentTypeWithPage(SearchRequest req);
        Task<(bool Success, string Message)> CopyContentType(CopyForCompanyRequest req);
        // Video Tutorials
        List<VideoTutorialViewModel> GetVideoTutorialList(int companyId, int? classId, int? sectionId, string? searchTerm);
        Task<PagedResult<VideoTutorialViewModel>> GetAllVideoTutoriaWithPage(SearchRequest req);
        VideoTutorialViewModel GetVideoTutorialById(int id);
        (bool Success, string Message) UpsertVideoTutorial(VideoTutorialUpsertRequest req, int companyId, int userId);
        (bool Success, string Message) DeleteVideoTutorial(List<int> id, int userId);
        (bool Success, string Message) ToggleVideoTutorialStatus(StatusUpdateRequest request);

        // Upload Content
        List<ContentViewModel> GetContentList(int companyId, string? searchTerm);
        Task<PagedResult<ContentViewModel>> GetAllContentWithPage(SearchRequest req);
        (bool Success, string Message) SaveContent(ContentViewModel content, int companyId, int userId);
        (bool Success, string Message) DeleteContent(int id, int userId);

        // Shared Links
        (bool Success, string Message, string Token) GenerateSharedLink(SharedLinkUpsertRequest req, int companyId, int userId);
        List<SharedLinkViewModel> GetSharedLinkList(int companyId);
        Task<PagedResult<SharedLinkViewModel>> GetAllSharedLinkWithPage(SearchRequest req);
        (bool Success, string Message) DeleteSharedLink(List<int> id,int companyId);
        (bool Success, string Message) SaveUploadContent(string title, int typeId, string fileType, string fileName, string filePath, string fileSize, int companyId, int userId);

        Task<Domain.GenerateSharedLinkResult> ExecuteGenerateSharedLinkSpAsync(
           Domain.GenerateSharedLinkRequest request, int companyId, int userId, string userName);


    }
}
