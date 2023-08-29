using Rahpele.Models.Data;
using Rahpele.Models;
using Rahpele.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Rahpele.ViewModels.RolePermission;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Rahpele.Services
{
    public class RolePermissionManager : IRolePermissionManager
    {
        private readonly ApplicationDbContext _context;

        public RolePermissionManager(ApplicationDbContext context)
        {
            _context = context;
        }


        public bool CheckPermission(Guid permissionId, string phoneNumber)
        {
            var user = _context.Users
                .Where(x => x.PhoneNumber == phoneNumber)
                .Select(x => new { x.Id, RoleIds = x.UserRoles.Select(ur => ur.RoleId) })
                .FirstOrDefault();

            if (user == null || !user.RoleIds.Any())
            {
                return false;
            }

            bool hasPermission = _context.RolePermissions
                .Any(rp => user.RoleIds.Contains(rp.RoleId) && rp.PermissionId == permissionId);

            return hasPermission;
        }


        public IEnumerable<ListRolesForManageViewModel> GetAllRolesForManage()
        {
            int rowCounter = 1;
            return _context.Roles
                .AsEnumerable()
                .Select((x, index) => new ListRolesForManageViewModel
                {
                    Id = x.Id,
                    RowId = rowCounter + index,
                    Title = x.Name
                })
                .ToList();
        }

        public Role GetRoleById(Guid roleId)
        {
            return _context.Roles.FirstOrDefault(r => r.Id == roleId);
        }

        public bool AddRole(ManageRoleViewModel model)
        {
            if(model != null)
            {

                Role role = new Role
                {
                    Name = model.Title,
                };
                _context.Roles.Add(role);
                _context.SaveChanges();

                foreach(var permissionId in model.PermissionIds)
                {
                    if(permissionId != null)
                    {
                        RolePermission rolePermission = new RolePermission
                        {
                            PermissionId = permissionId.Value,
                            RoleId = role.Id
                        };
                        _context.RolePermissions.Add(rolePermission);
                    }
                }
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public ManageRoleViewModel GetRoleForManage(Guid roleId)
        {
            return _context.Roles
                .Include(x => x.RolePermissions)
                .Where(x => x.Id == roleId)
                .Select(x => new ManageRoleViewModel
                {
                    Id = x.Id,
                    PermissionIds = x.RolePermissions.Select(rp => rp.PermissionId).ToList(),
                    Title = x.Name
                })
                .FirstOrDefault();
        }


        public bool UpdateRole(ManageRoleViewModel model)
        {
            var role = _context.Roles
                .Include(x => x.RolePermissions)
                .FirstOrDefault(x => x.Id == model.Id);

            if (role != null)
            {
                role.Name = model.Title;

                // حذف دسترسی‌های قبلی که از مدل حذف شده‌اند
                foreach (var rolePermission in role.RolePermissions.ToList())
                {
                    if (!model.PermissionIds.Contains(rolePermission.PermissionId))
                    {
                        _context.RolePermissions.Remove(rolePermission);
                    }
                }

                // اضافه کردن دسترسی‌های جدید
                foreach (var permissionId in model.PermissionIds)
                {
                    if (permissionId != null && !role.RolePermissions.Any(rp => rp.PermissionId == permissionId))
                    {
                        RolePermission rolePermission = new RolePermission
                        {
                            PermissionId = permissionId.Value,
                            RoleId = role.Id
                        };
                        role.RolePermissions.Add(rolePermission);
                    }
                }

                _context.Roles.Update(role);
                _context.SaveChanges();
                return true;
            }

            return false;
        }



        public IEnumerable<ListPermissionsForManageViewModel> GetAllPermissionsForManage()
        {
            int rowCounter = 1;
            return _context.Permissions
                .Include(x => x.Parent)
                .AsEnumerable()
                .Select((x, index) => new ListPermissionsForManageViewModel
                {
                    Id = x.Id,
                    RowId = rowCounter + index,
                    Title = x.Name,
                    ParentTitle = x.Parent != null ? x.Parent.Name : "---",
                })
                .ToList();
        }


        public List<Permission> GetListPermissions()
        {
            return _context.Permissions.ToList();
        }


        public List<Role> GetListRoles()
        {
            return _context.Roles.ToList();
        }

        public List<SelectListItem> GetListPermissionsForSelectList()
        {
            return _context.Permissions
              .Where(x => x.ParentId == null)
              .Select((x) => new SelectListItem
              {
                  Text = x.Name,
                  Value = x.Id.ToString()
              })
              .ToList();
        }

        public Permission GetPermissionById(Guid permissionId)
        {
            return _context.Permissions.FirstOrDefault(p => p.Id == permissionId);
        }

        public void AddPermission(ManagePermissionViewModel model)
        {
            Permission permission = new Permission
            {
                Name = model.Title,
                ParentId = model.ParentId,
            };
            _context.Permissions.Add(permission);
            _context.SaveChanges();
        }

        public void UpdatePermission(Permission permission)
        {
            _context.Permissions.Update(permission);
            _context.SaveChanges();
        }

        public IEnumerable<RolePermission> GetRolePermissionsByRoleId(Guid roleId)
        {
            return _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Include(rp => rp.Permission)
                .ToList();
        }

        public void AddPermissionToRole(Guid roleId, Guid permissionId)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
            var permission = _context.Permissions.FirstOrDefault(p => p.Id == permissionId);

            if (role != null && permission != null)
            {
                var rolePermission = new RolePermission
                {
                    Role = role,
                    Permission = permission
                };
                _context.RolePermissions.Add(rolePermission);
                _context.SaveChanges();
            }
        }

        public void RemovePermissionFromRole(Guid roleId, Guid permissionId)
        {
            var rolePermission = _context.RolePermissions.FirstOrDefault(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);
            if (rolePermission != null)
            {
                _context.RolePermissions.Remove(rolePermission);
                _context.SaveChanges();
            }
        }





    }
}
