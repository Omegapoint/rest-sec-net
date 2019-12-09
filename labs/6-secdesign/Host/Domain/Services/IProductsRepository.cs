using System.Collections.Generic;

namespace SecureByDesign.Host.Domain.Services
{
    public interface IProductsRepository{
        Product GetById(ProductId id);
        List<Product> SearchById(SearchTerm idSearchTerm);
    }
}