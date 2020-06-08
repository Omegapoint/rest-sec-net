using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Memory;
using SecureByDesign.Host;
using SecureByDesign.Host.Domain.Model;
using SecureByDesign.Host.Domain.Services;
using SecureByDesign.Host.Infrastructure;
using Xunit;

namespace Tests
{
    //Note that the tests assumes a test repository with the products "abc", "def" and "ghi", but not "xyz".
    [Trait("Category", "Unit")]
    public class ProductsServiceTests
    {
        [Fact]
        public async void GetById_ReturnsForbidden_IfNoValidReadClaim()
        {
            var identity = new ClaimsIdentity(new[]{new Claim(ClaimSettings.UrnLocalIdentity, "testId")});
            var principal = new ClaimsPrincipal(identity);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var permissions = new Permissions{
                GrantedServicePermissions = new List<ServicePermission>{ServicePermission.ProductWrite},
                GrantedProducts = new List<ProductId>{new ProductId("abc"), new ProductId("def")}
            };
            cache.Set("testId|", permissions);
            var productsService = new ProductsService(new AccessControlService(cache, new PermissionsInMemoryRepository(), new CentralizedLoggingService()), new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await productsService.GetById(principal, new ProductId("abc"));

            Assert.Equal(ServiceResult.Forbidden, result.Result);
            Assert.Null(result.Value);
        }

        [Fact]
        public async void GetById_ReturnsNotfound_IfNoAccessToGivenProduct()
        {
            var identity = new ClaimsIdentity(new[]{new Claim(ClaimSettings.UrnLocalIdentity, "testId"),});
            var principal = new ClaimsPrincipal(identity);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var permissions = new Permissions{
                GrantedServicePermissions = new List<ServicePermission>{ServicePermission.ProductRead},
                GrantedProducts = new List<ProductId>{new ProductId("abc"), new ProductId("def")}
            };
            cache.Set("testId|", permissions);
            var productsService = new ProductsService(new AccessControlService(cache, new PermissionsInMemoryRepository(), new CentralizedLoggingService()), new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await productsService.GetById(principal, new ProductId("ghi"));

            Assert.Equal(ServiceResult.NotFound, result.Result);
            Assert.Null(result.Value);
        }

        // Testing successful resource access is important to verify that the
        // correct claim is needed to authorize access.  If we did not, then
        // requiring a lower claim, e.g. "read:guest" would not be caught by the
        // NoValidScopeClaim test above; This test will catch such configuration errors.
        [Fact]
        public async void GetById_ReturnsOk_IfValidClaims()
        {
            var identity = new ClaimsIdentity(new[]{new Claim(ClaimSettings.UrnLocalIdentity, "testId")});
            var principal = new ClaimsPrincipal(identity);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var permissions = new Permissions{
                GrantedServicePermissions = new List<ServicePermission>{ServicePermission.ProductRead},
                GrantedProducts = new List<ProductId>{new ProductId("abc"), new ProductId("def")}
            };
            cache.Set("testId|", permissions);
            var productsService = new ProductsService(new AccessControlService(cache, new PermissionsInMemoryRepository(), new CentralizedLoggingService()), new ProductsInMemoryRepository(), new CentralizedLoggingService());

            var result = await productsService.GetById(principal, new ProductId("abc"));

            Assert.Equal(ServiceResult.Ok, result.Result);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async void GetById_ReturnsNotFound_IfValidClaimButNotExisting()
        {
            var identity = new ClaimsIdentity(new[]{new Claim(ClaimSettings.UrnLocalIdentity, "testId")});
            var principal = new ClaimsPrincipal(identity);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var permissions = new Permissions{
                GrantedServicePermissions = new List<ServicePermission>{ServicePermission.ProductRead},
                GrantedProducts = new List<ProductId>{new ProductId("abc"), new ProductId("def"), new ProductId("xyz")}
            };
            cache.Set("testId|", permissions);
            var productsService = new ProductsService(new AccessControlService(cache, new PermissionsInMemoryRepository(), new CentralizedLoggingService()), new ProductsInMemoryRepository(), new CentralizedLoggingService());
            var result = await productsService.GetById(principal, new ProductId("xyz"));

            Assert.Equal(ServiceResult.NotFound, result.Result);
            Assert.Null(result.Value);
        }
    }
}

