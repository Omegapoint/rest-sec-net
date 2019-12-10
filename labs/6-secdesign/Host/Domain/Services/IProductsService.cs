using System.Security.Principal;
using System.Threading.Tasks;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Domain.Services
{
    public interface IProductsService
    {
        Task<ProductResult> GetById(IPrincipal principal, ProductId id);
        Task<ProductListResult> SearchById(IPrincipal principal, SearchTerm idTerm);
    }
}