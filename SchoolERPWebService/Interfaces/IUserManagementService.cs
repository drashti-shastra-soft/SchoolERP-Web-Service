using SchoolERPWebService.Domain;
using SchoolERPWebService.Domain.Common;

namespace SchoolERPWebService.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<UserPermissionViewModel>> GetUserPermissionsAsync(int userId, int roleId);
        Task<List<UserPermissionViewModel>> GetUserPermissions(int userId, string? menuUrlPrefix = null, string? permissionName = null, int? roleID = null);
        List<MstUserTypeViewModel> GetAllUserTypes();
        MstUserTypeViewModel? GetUserTypeByID(int userTypeID);
        (bool success, string message) UpsertUserType(MstUserTypeUpsertRequest request, int userId, string ipAddress);
        (bool success, string message) ToggleUserTypeStatus(StatusUpdateRequest request);
        List<MstRoleViewModel> GetAllRoles();
        MstRoleViewModel? GetRoleByID(int roleID);
        Task<(bool success, string message, int roleId)> UpsertRole(MstRoleUpsertRequest request, int userId, string ipAddress);
        (bool success, string message) ToggleRoleStatus(StatusUpdateRequest request);
        List<RoleMenuPermissionViewModel> GetPermissionsMatrix(int roleId);
        (bool success, string message) SaveRolePermissions(MstRolePermissionSaveRequest request, int adminId, string ipAddress);
        (bool success, string message) DeleteRole(List<int> roleID, int userId, string ipAddress);
        List<MstPermissionViewModel> GetAllPermissions();
        MstPermissionViewModel? GetPermissionByID(int permissionID);
        (bool success, string message) UpsertPermission(MstPermissionUpsertRequest request, int userId, string ipAddress);
        (bool success, string message) TogglePermissionStatus(StatusUpdateRequest request);
        (bool success, string message) DeletePermission(List<int> permissionID, int userId, string ipAddress);
        List<UserRoleViewModel> GetAllRolesByUser(int userId);
        Task<PagedResult<MstPermissionViewModel>> GetAllPermissionWithPage(SearchRequest req);
        Task<PagedResult<MstRoleViewModel>> GetAllRoleWithPage(SearchRequest req);
        List<SearchByRoleModel> GetAllRolesBySearch(int roleID, string searchTerm, int companyID, int sessionID);
        Task<PagedResult<MstUserTypeViewModel>> GetAllUserTypesWithPage(SearchRequest req);
        (bool success, string message) DeleteUserType(
            List<int> userTypeID,
            int userId);
    }
}
