using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Domain.Services
{
    public class AccessControlService : IAccessControlService
    {
        IDistributedCache _permissionsCache;
        public AccessControlService(IDistributedCache permissionsCache){
            _permissionsCache = permissionsCache;
        }
        
        public async Task<Permissions> GetPermissions(IPrincipal principal)
        {
            var permissions = await LookupUserPermissions(principal);
            return permissions;
        }
        public async Task<bool> CanRead(IPrincipal principal, Type serviceType)
        {
            var permissions = await LookupUserPermissions(principal);

            return permissions.GrantedServices.Contains(serviceType.Name); 
        }

        public async Task<bool> CanAccess(IPrincipal principal, ProductId requestedProductId)
        {
            var permissions = await LookupUserPermissions(principal);

            return permissions.GrantedProducts.Any(p => p == requestedProductId);
        }

        public async Task<bool> CanAccessList(IPrincipal principal, List<Product> requestedProducts)
        {
            var permissions = await LookupUserPermissions(principal);

            return requestedProducts.Select(p => p.Id).ToList().All(permissions.GrantedProducts.Contains);
        }

        private async Task<Permissions> LookupUserPermissions(IPrincipal principal){
            var key = (principal.Identity as ClaimsPrincipal).Claims.Single(c => c.Type == "sub").Type;
            var permissionsAsBytes = await _permissionsCache.GetAsync(key);
            var permissions = Permissions.CreateFromByteArray(permissionsAsBytes);
            if(permissions != null)
                return permissions;
            
            //TODO: Get permission from repo based on identity (sub as key)
            permissions = new Permissions{
                GrantedServices = new List<string>{typeof(ProductsService).Name},
                GrantedProducts = new List<ProductId>{new ProductId("abc"), new ProductId("def")}
            };

            await _permissionsCache.SetAsync(key, permissions.ToByteArray(), new DistributedCacheEntryOptions{AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)});
            return permissions;
        }
    }

    public class Permissions
    {
        public List<string> GrantedServices {get; set;} 
        public List<ProductId> GrantedProducts {get; set;} 

        public byte[] ToByteArray()
        {
            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                return ms.ToArray();
            }
        }

        public static Permissions CreateFromByteArray(byte[] data)
        {
            if(data == null)
                return default(Permissions);
            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (Permissions)obj;
            }
        }
    }
}