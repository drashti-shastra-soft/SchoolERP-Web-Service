using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IClassCoScholasticService
    {
        Task<int> SaveAsync(ClassCoScholasticSaveRequest request, int userId, string? userName);
        Task<List<ClassCoScholasticTreeItemViewModel>> GetTreeAsync(int companyId, int sessionId, int classId);
        Task<List<DropdownModel>> GetClassCoScholasticListAsync(int companyId, int sessionId, int classId);
    }
}
