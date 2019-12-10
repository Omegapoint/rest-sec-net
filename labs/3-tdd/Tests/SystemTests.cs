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
        private readonly Uri baseUri = new Uri("http://localhost:5001/");

        [Fact]
        public async Task  GetProductByIdShouldReturn401WhenNotAuthenticated()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "/api/products/1"));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task  GetProductByIdShouldReturn200WhenAuthenticated()
        {
            var client = new TokenHttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "/api/products/1"));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
