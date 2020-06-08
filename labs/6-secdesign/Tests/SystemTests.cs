using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    [Trait("Category", "System")]
    public class SystemTests
    {
        private readonly Uri baseUri = new Uri("http://localhost:5009/");

        [Fact]
        public async Task  GetProductByIdShouldReturn401WhenNotAuthenticated()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "/api/products/abc"));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task  GetProductByIdShouldReturn404WhenNoAccess()
        {
            var client = new TokenHttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "/api/products/ghi"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task  GetProductByIdShouldReturn404WhenNotExists()
        {
            var client = new TokenHttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "/api/products/xyz"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task  GetProductByIdShouldReturn200WhenAuthenticated()
        {
            var client = new TokenHttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "/api/products/abc"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task  PingReturn200WhenNotAuthenticated()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "/api/public/ping"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
