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
        Task<bool> CanPerformOperation(IPrincipal principal, ServicePermission servicePermission);
        Task<bool> CanAccessObject(IPrincipal principal, ProductId requestedProductId);
        Task<bool> CanAccessAllInList(IPrincipal principal, List<Product> requestedProducts);
        Task<List<Product>> ExcludeUnauthorizedProducts(IPrincipal principal, List<Product> requestedProducts);
    }
}