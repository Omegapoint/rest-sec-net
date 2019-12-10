using System.Security.Claims;
using SecureByDesign.Host;
using SecureByDesign.Host.Domain.Model;
using SecureByDesign.Host.Domain.Services;
using SecureByDesign.Host.Infrastructure;
using Xunit;

namespace Tests
{
    [Trait("Category", "Unit")]
    public class ProductsServiceTests
    {
        [Fact]
        public async void GetById_ReturnsForbidden_IfNoValidReadClaim()
        {
            var claims = new[]
            {
                new Claim("not valid read claim", "true"),
                new Claim(ClaimSettings.UrnLocalProductIds, "abc"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService(new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await product.GetById(principal, new ProductId("abc"));

            Assert.Equal(ServiceResult.Forbidden, result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public async void GetById_ReturnsForbidden_IfNoValidProductIdClaim()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.UrnLocalProductRead, "true"),
                new Claim(ClaimSettings.UrnLocalProductIds, "abc"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService(new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await product.GetById(principal, new ProductId("def"));

            Assert.Equal(ServiceResult.Forbidden, result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public async void GetById_ReturnsForbidden_IfNoValidProductIdClaimInLIst()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.UrnLocalProductRead, "true"),
                new Claim(ClaimSettings.UrnLocalProductIds, "abc,def"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService(new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await product.GetById(principal, new ProductId("ghi"));

            Assert.Equal(ServiceResult.Forbidden, result.Result);
            Assert.Null(result.Value);
        }

        // Testing successful resource access is important to verify that the
        // correct claim is needed to authorize access.  If we did not, then
        // requiring a lower claim, e.g. "read:guest" would not be caught by the
        // NoValidScopeClaim test above; This test will catch such configuration errors.
        [Fact]
        public async void GetById_ReturnsOk_IfValidClaims()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.UrnLocalProductRead, "true"),
                new Claim(ClaimSettings.UrnLocalProductIds, "abc"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService(new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await product.GetById(principal, new ProductId("abc"));

            Assert.Equal(ServiceResult.Ok, result.Result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async void GetById_ReturnsNotFound_IfValidClaimButNotExisting()
        {
            var claims = new[]
            {
                new Claim(ClaimSettings.UrnLocalProductRead, "true"),
                new Claim(ClaimSettings.UrnLocalProductIds, "xyz"),
            };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            var product = new ProductsService(new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await product.GetById(principal, new ProductId("xyz"));

            Assert.Equal(ServiceResult.NotFound, result.Result);
            Assert.Null(result.Value);
        }
    }
}

