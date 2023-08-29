using Microsoft.AspNetCore.Mvc.Rendering;
using Rahpele.Models;
using Rahpele.ViewModels.RolePermission;

namespace Rahpele.Services.Interfaces
{
    public interface IRolePermissionManager
    {
        bool CheckPermission(Guid permissionId, string userName);

        IEnumerable<ListRolesForManageViewModel> GetAllRolesForManage();
        Role GetRoleById(Guid roleId);
        bool AddRole(ManageRoleViewModel model);
        ManageRoleViewModel GetRoleForManage(Guid roleId);
        bool UpdateRole(ManageRoleViewModel model);

        IEnumerable<ListPermissionsForManageViewModel> GetAllPermissionsForManage();
        List<Permission> GetListPermissions();
        List<Role> GetListRoles();
        Permission GetPermissionById(Guid permissionId);
        void AddPermission(ManagePermissionViewModel model);
        void UpdatePermission(Permission permission);
        List<SelectListItem> GetListPermissionsForSelectList();

        IEnumerable<RolePermission> GetRolePermissionsByRoleId(Guid roleId);
        void AddPermissionToRole(Guid roleId, Guid permissionId);
        void RemovePermissionFromRole(Guid roleId, Guid permissionId);
    }
}
