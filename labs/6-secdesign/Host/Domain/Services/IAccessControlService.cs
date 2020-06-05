using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Domain.Services
{
    public interface IAccessControlService
    {
        Task<Permissions> GetPermissions(IPrincipal principal);
        Task<bool> CanRead(IPrincipal principal, Type serviceType);
        Task<bool> CanAccess(IPrincipal principal, ProductId requestedProductId);
        Task<bool> CanAccessList(IPrincipal principal, List<Product> requestedProducts);
    }
}