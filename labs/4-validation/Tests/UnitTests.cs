using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Validation.Host;
using Validation.Host.Controllers;
using Xunit;

namespace Tests
{
    [Trait("Category", "Unit")]
    public class UnitTests
    {
        [Fact]
        public void GetProductsByIdShouldReturn403WhenMissingClaims()
        {
            var controller = new ProductsController();

            var claims = new[] { new Claim("scope", "not a valid scope") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = controller.GetById("abc");

            Assert.IsType<ForbidResult>(result.Result);
        }

        // Testing successful resource access is important to verify that the
        // correct claim is needed to authorize access.  If we did not, then
        // requiring a lower claim, e.g. "read:guest" would not be caught by the
        // 403 test above; This test will catch such configuration errors.
        [Fact]
        public void GetProductsByIdShouldReturn200WhenAuthorized()
        {
            var controller = new ProductsController();

            var claims = new[] { new Claim("scope", "read:product") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = controller.GetById("abc");

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [MemberData(nameof(IdInjection))]
        [MemberData(nameof(InvalidIds))]
        public void GetProductsByIdShouldReturn400WhenInvalidId(string id)
        {
            var controller = new ProductsController();

            var claims = new[] { new Claim("scope", "read:product") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = controller.GetById(id);

            Assert.IsType<BadRequestResult>(result.Result);
        }
        
        [Fact]
        public void GetProductsByIdShouldReturn404WhenNotFound()
        {
            var controller = new ProductsController();

            var claims = new[] { new Claim("scope", "read:product") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = controller.GetById("def"); // This is a valid, non-existing id

            Assert.IsType<NotFoundResult>(result.Result);
        }
        
        [Theory]
        [MemberData(nameof(IdInjection))]
        [MemberData(nameof(InvalidIds))]
        public void ProductIdShouldReject(string id)
        {
            Assert.False(ProductId.IsValidId(id));
        }

        [Theory]
        [MemberData(nameof(IdInjection))]
        [MemberData(nameof(InvalidIds))]
        public void ProductIdConstructorShouldThrow(string id)
        {
            Assert.Throws<ArgumentException>(() => new ProductId(id));
        }

        public static IEnumerable<object[]> IdInjection => new[]
        {
            new object[] { "<script>" },
            new object[] { "'1==1" },
            new object[] { "--sql" }
        };

        public static IEnumerable<object[]> InvalidIds => new[]
        {
            new object[] { "" },
            new object[] { "no spaces" },
            new object[] { "thisisanidthatistoolong" },
            new object[] { "#" }
        };
    }
}
