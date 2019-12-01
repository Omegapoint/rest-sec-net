using System.Security.Claims;
using SecureByDesign.Host;
using Xunit;

namespace Tests
{
    [Trait("Category", "Unit")]
    public class ProductsServiceTests
    {
        [Fact]
        public void GetById_ReturnsForbidden_IfNoValidScopeClaim()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.Scope, "not a valid scope"),
                new Claim(ClaimSettings.UrnLocalProductId, "abc"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService();

            var result = product.GetById(principal, new ProductId("abc"));

            Assert.Equal(ServiceResult.Forbidden, result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public void GetById_ReturnsForbidden_IfNoValidProductIdClaim()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.Scope, ClaimSettings.ReadProduct),
                new Claim(ClaimSettings.UrnLocalProductId, "abc"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService();

            var result = product.GetById(principal, new ProductId("def"));

            Assert.Equal(ServiceResult.Forbidden, result.Result);
            Assert.Null(result.Value);
        }

        // Testing successful resource access is important to verify that the
        // correct claim is needed to authorize access.  If we did not, then
        // requiring a lower claim, e.g. "read:guest" would not be caught by the
        // NoValidScopeClaim test above; This test will catch such configuration errors.
        [Fact]
        public void GetById_ReturnsOk_IfValidClaims()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.Scope, ClaimSettings.ReadProduct),
                new Claim(ClaimSettings.UrnLocalProductId, "abc"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService();

            var result = product.GetById(principal, new ProductId("abc"));

            Assert.Equal(ServiceResult.Ok, result.Result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void GetById_ReturnsNotFound_IfValidClaimButNotExisting()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.Scope, ClaimSettings.ReadProduct),
                new Claim(ClaimSettings.UrnLocalProductId, "xyz"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService();

            var result = product.GetById(principal, new ProductId("xyz"));

            Assert.Equal(ServiceResult.NotFound, result.Result);
            Assert.Null(result.Value);
        }
    }
}

