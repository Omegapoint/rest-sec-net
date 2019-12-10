using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Controllers
{
    public class ProductResponseModel
    {
        public string Name {get; set;}
    }

    public static class ProductResponseModelExtension
    {
        public static ProductResponseModel MapToProductResponseModel(this Product product){
            return new ProductResponseModel{Name = product.Name.Value };
        }
    }
}