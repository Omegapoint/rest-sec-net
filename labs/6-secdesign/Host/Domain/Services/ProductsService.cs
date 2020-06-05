using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Domain.Services
{
    public class ProductsService : IProductsService
    {
        private IAccessControlService _accessControlService;
        private IProductsRepository _productRepository;
        private ILoggingService _logger;
        public ProductsService(IAccessControlService accessControlService, IProductsRepository productRepository, ILoggingService logger){
            _accessControlService = accessControlService;
            _productRepository = productRepository;
            _logger = logger;
        }
        
        public async Task<ProductResult> GetById(IPrincipal principal, ProductId id)
        {
            if (!await _accessControlService.CanRead((ClaimsPrincipal)principal, this.GetType()))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - missing permission {ClaimSettings.ProductsRead}");
                return new ProductResult(ServiceResult.Forbidden, null);
            }

            if (!await _accessControlService.CanAccess((ClaimsPrincipal)principal, id))
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
            if (!await _accessControlService.CanRead((ClaimsPrincipal)principal, this.GetType()))
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

            // If search, and multiple products are returned, then we need to check that all result items are allowed 
            // (or remove from search result) 
            var productIds = string.Join(',', products.Select(p => p.Id.Value));
            if (!await _accessControlService.CanAccessList((ClaimsPrincipal)principal, products))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - no access to product resources from search {idTerm.Value}");
                return new ProductListResult(ServiceResult.Forbidden, null);
            }

            await _logger.Log(principal.Identity.Name, $"Audit: Granted access to product resources: {productIds}");
            return new ProductListResult(ServiceResult.Ok, products);
        }
    }
}