using System.Security.Claims;

namespace Headers.Host
{
    public class Product
    {
        public Product(ProductId id)
        {
            Id = id;
        }

        public ProductId Id { get; }

        public string Name => "My Product";

        public bool CanRead(ClaimsPrincipal principal)
        {
            return principal.HasClaim(c => c.Type == "urn:local:product:read" && c.Value == "true");
        }
    }
}
