using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureByDesign.Host.Domain.Model;
using SecureByDesign.Host.Domain.Services;

namespace SecureByDesign.Host.Controllers
{
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        [HttpGet("api/products/{id}")]
        public async Task<ActionResult<ProductResponseModel>> GetProductById(string id)
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

        //Exampel of how to use [AllowAnonymous]
        [AllowAnonymous]
        [HttpGet("api/public/ping")]
        public ActionResult<string> Ping()
        {
            return "ping";
        }
    }
}