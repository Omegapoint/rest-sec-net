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
        public async Task ApiShouldReturnContentTypeOptions()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "404"));

            Assert.True(response.Headers.TryGetValues("X-Content-Type-Options", out var values));
            Assert.Contains("nosniff", values);
        }

        [Fact]
        public async Task ApiShouldNotReturnServer()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "404"));

            Assert.False(response.Headers.Contains("Server"));
        }

        [Fact(Skip = "Only possible for https hosting")]
        public async Task ApiShouldReturnHsts()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(new Uri(baseUri, "404"));

            Assert.True(response.Headers.TryGetValues("Strict-Transport-Security", out var values));
            Assert.Contains("max-age=", values);
        }
    }
}
