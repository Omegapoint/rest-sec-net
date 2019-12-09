using System.Collections.Generic;
using System.Linq;
using SecureByDesign.Host.Domain;
using SecureByDesign.Host.Domain.Services;

namespace SecureByDesign.Host.Infrastructure
{
    public class ProductsInMemoryRepository : IProductsRepository
    {
        private readonly Dictionary<ProductId, Product> products = new Dictionary<ProductId, Product>
        {
            [new ProductId("abc")] = new Product(new ProductId("abc")),
            [new ProductId("def")] = new Product(new ProductId("def")),
            [new ProductId("ghi")] = new Product(new ProductId("ghi"))
        };

        public Product GetById(ProductId id)
        {
            return products.GetValueOrDefault(id);
        }

        public List<Product> SearchById(SearchTerm term)
        {
            return products.Values.Where(i => i.Id.Value.Contains(term.Value)).ToList();
        }
    }
}