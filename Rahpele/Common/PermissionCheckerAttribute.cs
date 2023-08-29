using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Rahpele.Services.Interfaces;
using System;

namespace Rahpele.Common
{
    public class PermissionCheckerAttribute : Attribute, IAuthorizationFilter
    {
        private Guid _permissionId;
        private IRolePermissionManager _permissionService;
        private MemoryCache _cache;

        public PermissionCheckerAttribute(string permissionId)
        {
            if (Guid.TryParse(permissionId, out Guid parsedGuid))
            {
                _permissionId = parsedGuid;
            }
            else
            {
                throw new ArgumentException("Invalid GUID format.", nameof(permissionId));
            }
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string phoneNumber = context.HttpContext.User.Identity.Name;

                bool hasPermission = GetCachedUserPermission(phoneNumber);

                if (!hasPermission)
                {
                    _permissionService = (IRolePermissionManager)context.HttpContext.RequestServices.GetService(typeof(IRolePermissionManager));
                    hasPermission = _permissionService.CheckPermission(_permissionId, phoneNumber);
                    CacheUserPermission(phoneNumber, hasPermission);
                }

                if (!hasPermission)
                {
                    context.Result = new RedirectResult("/Home/Index");
                }
            }
            else
            {
                context.Result = new RedirectResult("/Authentication/Login");
            }
        }

        private bool GetCachedUserPermission(string phoneNumber)
        {
            if (_cache.TryGetValue(phoneNumber, out bool hasPermission))
            {
                return hasPermission;
            }
            return false;
        }

        private void CacheUserPermission(string phoneNumber, bool hasPermission)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // مدت زمان کش: 30 دقیقه
            };
            _cache.Set(phoneNumber, hasPermission, cacheEntryOptions);
        }
    }



}
