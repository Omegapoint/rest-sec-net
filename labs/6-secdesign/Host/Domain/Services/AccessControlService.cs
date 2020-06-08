using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Domain.Services
{
    public class AccessControlService : IAccessControlService
    {
        IMemoryCache _permissionsCache;
        IPermissionsRepository _permissionsRepository;
        ILoggingService _loggingService;

        public AccessControlService(IMemoryCache permissionsCache, IPermissionsRepository permissionsRepository, ILoggingService loggingService){
            _permissionsCache = permissionsCache;
            _permissionsRepository = permissionsRepository;
            _loggingService = loggingService;
        }
        
        public async Task<Permissions> GetPermissions(IPrincipal principal)
        {
            var permissions = await LookupUserPermissions(principal);
            return permissions;
        }
        public async Task<bool> CanPerformOperation(IPrincipal principal, ServicePermission servicePermission)
        {
            var permissions = await LookupUserPermissions(principal);

            return permissions.GrantedServicePermissions.Any(sp => sp == servicePermission);
        }

        public async Task<bool> CanAccessObject(IPrincipal principal, ProductId requestedProductId)
        {
            var permissions = await LookupUserPermissions(principal);

            return permissions.GrantedProducts.Any(p => p == requestedProductId);
        }

        public async Task<bool> CanAccessAllInList(IPrincipal principal, List<Product> requestedProducts)
        {
            var permissions = await LookupUserPermissions(principal);

            return requestedProducts.Select(p => p.Id).ToList().All(permissions.GrantedProducts.Contains);
        }

        public async Task<List<Product>> ExcludeUnauthorizedProducts(IPrincipal principal, List<Product> requestedProducts)
        {
            var permissions = await LookupUserPermissions(principal);
            var authorizedProducts = requestedProducts.Where(p => permissions.GrantedProducts.Any(gp => gp == p.Id)).ToList();
            return authorizedProducts;
        }

        private async Task<Permissions> LookupUserPermissions(IPrincipal principal){
            var identity = (principal as ClaimsPrincipal).Claims.Single(c => c.Type == ClaimSettings.UrnLocalIdentity).Value;
            var scopes = (principal as ClaimsPrincipal).Claims.Where(c => c.Type == ClaimSettings.Scope).Select(c => c.Value).ToList();
            var accessControlParameters = new AccessControlParameters(identity, scopes);
            var key = accessControlParameters.ToCacheKey();

            _permissionsCache.TryGetValue<Permissions>(key, out var permissions);
            if(permissions != null)
                return permissions;
            
            permissions = await _permissionsRepository.GetPermissions(accessControlParameters);
            if(permissions == null)
            {
                await _loggingService.Log(identity, $"No persmissions found for {key}");
                return null;
            }

            _permissionsCache.Set<Permissions>(key, permissions, new MemoryCacheEntryOptions{AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)});
            return permissions;
        }
    }
}