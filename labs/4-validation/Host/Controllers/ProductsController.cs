using Microsoft.AspNetCore.Mvc;

namespace Validation.Host.Controllers
{
    [ApiController]
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly Repository repository = new Repository();

        [HttpGet("{id}")]
        public ActionResult<Product> GetById(string id)
        {
            if (!ProductId.IsValidId(id))
            {
                return BadRequest(); // https://stackoverflow.com/q/3290182/291299
            }

            var productId = new ProductId(id);

            var product = repository.GetById(productId);

            if (product == null)
            {
                return NotFound();
            }

            if (!product.CanRead(User))
            {
                return Forbid();
            }

            return Ok(product);
        }
    }
}
