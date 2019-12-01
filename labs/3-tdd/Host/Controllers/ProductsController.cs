using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Tdd.Host.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<Product> Get()
        {
            var product = new Product(); // Repository

            if (!product.CanRead(User))
            {
                return Forbid();
            }

            return Ok(product);
        }
    }

    public class Product
    {
        public string Name => "My Product";

        public bool CanRead(ClaimsPrincipal principal)
        {
            return principal.HasClaim(c => c.Type == "scope" && c.Value.Contains("products.read"));
        }
    }    
}
