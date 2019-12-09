using System.Collections.Generic;
using SecureByDesign.Host.Domain;

namespace SecureByDesign.Host.Application
{
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

    public class ProductListResult
    {
        public ProductListResult(ServiceResult result, IEnumerable<Product> products)
        {
            Result = result;
            Value = products;
        }

        public ServiceResult Result { get; }
        public IEnumerable<Product> Value { get; }
    }

    

}