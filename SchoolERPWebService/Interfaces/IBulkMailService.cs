using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IBulkMailService
    {
        Task<BulkMailResult> QueueBulkMailAsync(BulkMailSendRequest req, int companyId);
        Task<BulkMailBatchStatus> GetBatchStatusAsync(Guid batchId);
    }
}
