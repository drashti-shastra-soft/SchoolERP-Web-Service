using SchoolERPWebService.Domain;

namespace SchoolERPWebService.Interfaces
{
    public interface IFeeTemplateService
    {
        Task<FeeTemplateResponse> UpsertFeeTemplateAsync(
            FeeTemplateRequest request);
        Task<ApiResponse> UpsertFeeTemplateDetailsAsync(
            int templateId,
            int companyId,
            int sessionId,
            List<FeeTemplateDetailRequest> details,
            int userId,
            string? ipAddress);

        Task<FeeTemplateGetByIdResponse> GetFeeTemplateByIdAsync(
    int feeTemplateId,
    int companyId,
    int sessionId);

        Task<FeeTemplatePageApiResponse> GetFeeTemplatePageAsync(
    FeeTemplatePageRequest request);

        (bool success, string message) DeleteFeeTemplet(List<int> ids, int userId, string ipAddress);
        Task<List<FeeTemplateGetByClassSectionResponse>>
    GetFeeTemplateByClassSectionAsync(FeeTemplateGetByClassSectionRequest request);

        Task<StudentFeeTemplateDetailsResponse?> GetFeeTemplateDetailsAsync(
    int feeTemplateId);

        Task<List<StudentInstallmentSummary>>
    GetStudentInstallmentSummary(int studentId, int companyId, int sessionId);

        Task<List<PaymentModeSummary>>
   GetPaymentModeSummary(StudentFeeCollectionRequest request);
    }
}
