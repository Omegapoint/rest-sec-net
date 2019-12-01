using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace SecureByDesign.Host
{
    public class ProductsService : IProductsService
    {
        private readonly Dictionary<ProductId, Product> productRepository = new Dictionary<ProductId, Product>
        {
            [new ProductId("abc")] = new Product(new ProductId("abc")),
            [new ProductId("def")] = new Product(new ProductId("def")),
            [new ProductId("ghi")] = new Product(new ProductId("ghi"))
        };

        public ProductResult GetById(IPrincipal principal, ProductId id)
        {
            if (!((ClaimsPrincipal)principal).HasClaim(
                c => c.Type == ClaimSettings.Scope && c.Value.Contains(ClaimSettings.ReadProduct)))
            {
                // TODO: audit log unauthorized, reason Scope
                return new ProductResult(ServiceResult.Forbidden, null);
            }

            if (!((ClaimsPrincipal)principal).HasClaim(
                c => c.Type == ClaimSettings.UrnLocalProductId && string.Equals(c.Value, id.Value, StringComparison.Ordinal)))
            {
                // TODO: audit  log unauthorized, reason UrnLocalProductId
                return new ProductResult(ServiceResult.Forbidden, null);
            }

            var product = productRepository.GetValueOrDefault(id);
            if (product == null)
            {
                // TODO: log warning, not found
                return new ProductResult(ServiceResult.NotFound, null);
            }

            // If search, and multiple products are returned, then we also need to check that all result items are allowed 
            if (!((ClaimsPrincipal)principal).HasClaim(
                c => c.Type == ClaimSettings.UrnLocalProductId && string.Equals(c.Value, product.Id.Value, StringComparison.Ordinal)))
            {
                //TODO: audit log unauthorized, reason UrnLocalProductId
                return new ProductResult(ServiceResult.Forbidden, null);
            }

            // TODO: audit log access
            return new ProductResult(ServiceResult.Ok, product);
        }
    }

    public interface IProductsService
    {
        ProductResult GetById(IPrincipal principal, ProductId id);
    }

    public class ProductResult
    {
        public ProductResult(ServiceResult result, Product product)
        {
            Result = result;
            Value = product;
        }

        public ServiceResult Result { get; }
        public Product Value { get; }
    }

    public enum ServiceResult
    {
        Forbidden,
        NotFound,
        Ok
    }
}