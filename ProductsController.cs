using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo
{
    [Route("products")]
    public class ProductsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var product = new Product(); // repository

            if (!product.CanGet(User))
            {
                return Forbid();
            }

            return Ok(product);
        }
    }

    public class Product
    {
        private const int MinAge = 58;

        public string Name => "My Product";

        public bool CanGet(ClaimsPrincipal principal)
        {
            var ageClaimValue = principal
                .Claims
                .FirstOrDefault(claim => claim.Type == "urn:omegapoint:age")
                ?.Value;

            return int.TryParse(ageClaimValue, out var age) && age >= MinAge;
        }
    }
}