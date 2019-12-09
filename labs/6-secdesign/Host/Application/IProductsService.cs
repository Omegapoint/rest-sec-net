using System.Security.Principal;
using System.Threading.Tasks;
using SecureByDesign.Host.Domain;

namespace SecureByDesign.Host.Application
{
    public interface IProductsService
    {
        Task<ProductResult> GetById(IPrincipal principal, ProductId id);
        Task<ProductListResult> SearchById(IPrincipal principal, SearchTerm idTerm);
    }
}