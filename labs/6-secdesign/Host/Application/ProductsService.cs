using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using SecureByDesign.Host.Domain;
using SecureByDesign.Host.Domain.Services;

namespace SecureByDesign.Host.Application
{
    public class ProductsService : IProductsService
    {
        private IProductsRepository _productRepository;
        private ILoggingService _logger;
        public ProductsService(IProductsRepository productRepository, ILoggingService logger){
            _productRepository = productRepository;
            _logger = logger;
        }
        
        public async Task<ProductResult> GetById(IPrincipal principal, ProductId id)
        {
            if (!CanRead((ClaimsPrincipal)principal))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - missing permission {ClaimSettings.ProductsRead}");
                return new ProductResult(ServiceResult.Forbidden, null);
            }

            if (!CanAccess((ClaimsPrincipal)principal, id))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - no access to product resource {id.Value}");
                return new ProductResult(ServiceResult.Forbidden, null);
            }

            var product = _productRepository.GetById(id);
            if (product == null)
            {
                await _logger.Log(principal.Identity.Name, $"Warning: Product resource {id.Value} not found");
                return new ProductResult(ServiceResult.NotFound, null);
            }

            await _logger.Log(principal.Identity.Name, $"Audit: Granted access to product resource: {id.Value}");
            return new ProductResult(ServiceResult.Ok, product);
        }

        public async Task<ProductListResult> SearchById(IPrincipal principal, SearchTerm idTerm)
        {
            if (!CanRead((ClaimsPrincipal)principal))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - missing permission {ClaimSettings.ProductsRead}");
                return new ProductListResult(ServiceResult.Forbidden, null);
            }

            var products = _productRepository.SearchById(idTerm);
            if (products.Count == 0)
            {
                await _logger.Log(principal.Identity.Name, $"Warning: Product resource {idTerm.Value} not found");
                return new ProductListResult(ServiceResult.NotFound, null);
            }

            var productIds = string.Join(',', products.Select(p => p.Id.Value));
            // If search, and multiple products are returned, then we need to check that all result items are allowed (or remove from search result) 
            if (!CanAccessList((ClaimsPrincipal)principal, products))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - no access to product resources from search {idTerm.Value}");
                return new ProductListResult(ServiceResult.Forbidden, null);
            }

            await _logger.Log(principal.Identity.Name, $"Audit: Granted access to product resources: {productIds}");
            return new ProductListResult(ServiceResult.Ok, products);
        }

        private static bool CanRead(ClaimsPrincipal claimsPrincipal){
            return claimsPrincipal.HasClaim(c => string.Equals(c.Type, ClaimSettings.ProductsRead, StringComparison.Ordinal));
        }

        private static bool CanAccess(ClaimsPrincipal claimsPrincipal, ProductId id){
            return claimsPrincipal.HasClaim(c => 
                string.Equals(c.Type, ClaimSettings.UrnLocalProductIds, StringComparison.Ordinal) && 
                string.Equals(c.Value, id.Value, StringComparison.Ordinal));
        }

        private static bool CanAccessList(ClaimsPrincipal claimsPrincipal, List<Product> products){
            if(!claimsPrincipal.HasClaim(c => string.Equals(c.Type, ClaimSettings.UrnLocalProductIds, StringComparison.Ordinal))){
                return false;
            }
            
            var valueList = claimsPrincipal.FindFirstValue(ClaimSettings.UrnLocalProductIds).Split(',', StringSplitOptions.RemoveEmptyEntries);
            return products.All(p => valueList.Any(s => string.Equals(p.Id.Value, s, StringComparison.Ordinal)));
        }
    }
}