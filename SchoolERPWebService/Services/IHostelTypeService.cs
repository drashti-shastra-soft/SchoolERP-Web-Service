using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Services
{
    public interface IHostelTypeService
    {
        // ============================================================
        // UPSERT (CREATE / UPDATE)
        // ============================================================
        Task<ApiResponse> UpsertHostelTypeAsync(HostelTypeUpsertRequest model);
        Task<HostelTypeModel?> GetHostelTypeByIdAsync(int hostelTypeId);
        Task<List<HostelTypeModel>> GetAllHostelTypesAsync(int companyId, int sessionId, bool includeDeleted = false);
        Task<PagedResult<HostelTypeModel>> GetAllHostelTypeWithPage(HostelTypeSearchRequest req);
        (bool success, string message) DeleteHostelType(List<int> ids, int userId);
        (bool success, string message) ToggleHostelTypeStatus(StatusUpdateRequest request);
        Task<(bool Success, string Message)> CopyHostelTypeToSession(CopyRequest req);
    }
}
