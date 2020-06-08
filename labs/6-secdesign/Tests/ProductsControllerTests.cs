using System.Collections.Generic;
using System.Security.Principal;
using SecureByDesign.Host;
using SecureByDesign.Host.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using SecureByDesign.Host.Domain.Model;
using SecureByDesign.Host.Domain.Services;
using System.Threading.Tasks;

namespace Tests
{
    [Trait("Category", "Unit")]
    public class ProductsControllerTests
    {
        [Fact]
        public async void GetProductsByIdShouldReturn403WhenCanNotRead()
        {
            var productServiceMock = new Mock<IProductsService>();
            productServiceMock.Setup(ps => ps.GetById(It.IsAny<IPrincipal>(), It.IsAny<ProductId>())).Returns(Task.FromResult(new ProductResult(ServiceResult.Forbidden, null)));

            var controller = new ProductsController(productServiceMock.Object);

            var result = await controller.GetProductById("abc");

            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async void GetProductsByIdShouldReturn200WhenAuthorized()
        {
            var productServiceMock = new Mock<IProductsService>();
            productServiceMock.Setup(ps => ps.GetById(It.IsAny<IPrincipal>(), It.IsAny<ProductId>()))
                .Returns(Task.FromResult(new ProductResult(ServiceResult.Ok, new Product(new ProductId("abc"), new ProductName("Product1")))));

            var controller = new ProductsController(productServiceMock.Object);

            var result = await controller.GetProductById("abc");

            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [MemberData(nameof(IdInjection))]
        [MemberData(nameof(InvalidIds))]
        public async void GetProductsByIdShouldReturn400WhenInvalidId(string id)
        {
            var productServiceMock = new Mock<IProductsService>();
            productServiceMock.Setup(ps => ps.GetById(It.IsAny<IPrincipal>(), It.IsAny<ProductId>()))
                .Returns(Task.FromResult(new ProductResult(ServiceResult.Ok, new Product(new ProductId("abc"), new ProductName("Product2")))));

            var controller = new ProductsController(productServiceMock.Object);

            var result = await controller.GetProductById(id);

            Assert.IsType<BadRequestResult>(result.Result);
        }

        [Fact]
        public async void GetProductsByIdShouldReturn404WhenNotFound()
        {
            var productServiceMock = new Mock<IProductsService>();
            productServiceMock.Setup(ps => ps.GetById(It.IsAny<IPrincipal>(), It.IsAny<ProductId>())).Returns(Task.FromResult(new ProductResult(ServiceResult.NotFound, null)));
            
            var controller = new ProductsController(productServiceMock.Object);

            var result = await controller.GetProductById("def"); // This is a valid, non-existing id

            Assert.IsType<NotFoundResult>(result.Result);
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
