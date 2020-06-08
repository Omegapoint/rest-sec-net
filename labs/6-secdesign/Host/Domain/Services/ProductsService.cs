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
            if (!await _accessControlService.CanPerformOperation((ClaimsPrincipal)principal, ServicePermission.ProductRead))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - missing permission {ServicePermission.ProductRead}");
                return new ProductResult(ServiceResult.Forbidden, null);
            }

            if (!await _accessControlService.CanAccessObject((ClaimsPrincipal)principal, id))
            {
                //Note that depending on how information we want to ge the client we can treat this as Forbidden or NotFound.
                //Returning Forbidden will give the client the infromation that this product exists, but that de client does not have access.
                //Returning NotFound will give the client no information, the products that exists are only the product that teh client has access to.
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - no access to product resource {id.Value}");
                return new ProductResult(ServiceResult.NotFound, null);
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
            if (!await _accessControlService.CanPerformOperation((ClaimsPrincipal)principal, ServicePermission.ProductRead))
            {
                await _logger.Log(principal.Identity.Name, $"Audit: Unauthorized - missing permission {ServicePermission.ProductRead}");
                return new ProductListResult(ServiceResult.Forbidden, null);
            }

            var products = _productRepository.SearchById(idTerm);
            if (products.Count == 0)
            {
                await _logger.Log(principal.Identity.Name, $"Warning: Product resource {idTerm.Value} not found");
                return new ProductListResult(ServiceResult.NotFound, null);
            }

            // If search and products are returned then we need to exclude any unauthorized products from the result 
            // (or check that all result items are allowed and return NotFound if not) 
            var authorizedProducts = await _accessControlService.ExcludeUnauthorizedProducts((ClaimsPrincipal)principal, products);
            //TODO: Log Unauthorized for all excluded products

            var productIds = string.Join(',', products.Select(p => p.Id.Value));
            await _logger.Log(principal.Identity.Name, $"Audit: Granted access to product resources: {productIds}");
            return new ProductListResult(ServiceResult.Ok, authorizedProducts);
        }
    }
}