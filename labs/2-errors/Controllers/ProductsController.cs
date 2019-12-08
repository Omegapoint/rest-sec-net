using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Errors.Controllers
{
    [ApiController]
    [Route("/api/products")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<Product> GetById(string id)
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
            return principal.HasClaim(c => c.Type == "urn:local:product:read" && c.Value == "true");
        }
    }    
}
