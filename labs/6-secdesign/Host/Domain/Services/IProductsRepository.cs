using System.Collections.Generic;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Domain.Services
{
        public interface IProductsRepository{
        Product GetById(ProductId id);
        List<Product> SearchById(SearchTerm idSearchTerm);
    }
}