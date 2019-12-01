using System.Collections.Generic;

namespace Validation.Host
{
    public class Repository
    {
        private readonly Dictionary<ProductId, Product> dictionary = new Dictionary<ProductId, Product>
        {
            [new ProductId("abc")] = new Product(new ProductId("abc"))
        };

        public Product GetById(ProductId id)
        {
            return dictionary.GetValueOrDefault(id);
        }
    }
}