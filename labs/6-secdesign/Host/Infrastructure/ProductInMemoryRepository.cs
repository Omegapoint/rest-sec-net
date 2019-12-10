using System.Collections.Generic;
using System.Linq;
using SecureByDesign.Host.Domain.Model;
using SecureByDesign.Host.Domain.Services;

namespace SecureByDesign.Host.Infrastructure
{
    public class ProductsInMemoryRepository : IProductsRepository
    {
        private readonly Dictionary<string, ProductDataObject> products = new Dictionary<string, ProductDataObject>
        {
            ["abc"] = new ProductDataObject{Id = "abc", Name = "Product1"},
            ["def"] = new ProductDataObject{Id = "def", Name = "Product2"},
            ["gih"] = new ProductDataObject{Id = "gih", Name = "Product3"},
        };

        public Product GetById(ProductId id)
        {
            var result = products.GetValueOrDefault(id.Value);

            return new Product(new ProductId(result.Id), new ProductName(result.Name));
        }

        public List<Product> SearchById(SearchTerm term)
        {
            var result = products.Values.Where(i => i.Id.Contains(term.Value)).ToList();

            return result.Select(pdo => new Product(new ProductId(pdo.Id), new ProductName(pdo.Name))).ToList();
        }
    }

    public class ProductDataObject
    {
        public string Id {get; set;}
        public string Name {get; set;}
    }
}