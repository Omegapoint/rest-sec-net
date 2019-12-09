using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Sample.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id:string}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<Product> GetById([FromRoute] string id)
        {
            if (!HasProductReadAccess(User))
            {
                return StatusCode(403);
            }

            var product = new Product(); // Get from repository

            return Ok(product);
        }

        private bool HasProductReadAccess(ClaimsPrincipal principal)
        {
            return principal.HasClaim(c => c.Type == "scope" && c.Value.Contains("products.read"));
        }
    }

    public class Product
    {
        public string Name => "My Product";  
    }    
}
