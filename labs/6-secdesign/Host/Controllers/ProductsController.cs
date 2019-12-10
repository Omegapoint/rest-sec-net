using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecureByDesign.Host.Domain.Model;
using SecureByDesign.Host.Domain.Services;

namespace SecureByDesign.Host.Controllers
{
    [ApiController]
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseModel>> GetById(string id)
        {
            if (!ProductId.IsValidId(id))
            {
                return BadRequest(); // https://stackoverflow.com/q/3290182/291299
            }

            var productId = new ProductId(id);

            var productResult = await productsService.GetById(User, productId);

            if (productResult.Result == ServiceResult.NotFound)
            {
                return NotFound();
            }

            if (productResult.Result == ServiceResult.Forbidden)
            {
                return Forbid();
            }

            return Ok(productResult.Value.MapToProductResponseModel());
        }
    }
}