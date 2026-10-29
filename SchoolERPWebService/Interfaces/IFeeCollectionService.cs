using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IFeeCollectionService
    {
        Task<StudentFeeTemplateListResponse> GetStudentFeeListAsync(StudentFeeListRequest request);
        Task<FeeCollectResponse> CollectOfflineAsync(FeeCollectRequest request);
        Task<OnlineStartResponse> StartOnlineAsync(OnlineStartRequest request);
        Task<FeeCollectResponse> FinalizeOnlineAsync(OnlineFinalizeRequest request);
        Task<FeeCollectResponse> CancelOnlineAsync(OnlineCancelRequest request);

        /// <summary>Returns the HTTP status the webhook endpoint should answer with (200 / 400 / 500).</summary>
        Task<int> HandleWebhookAsync(string gatewayName, string body, string signatureHeader);
        Task<RefundResponse> RefundAsync(RefundRequest request);
        Task<PagedResult<StudentFeeGetAllResponse>> GetAllStudentFeeWithPage(SearchRequest req);
    }
}