using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Clients.Test.System
{
    public class AntiForgeryTest
    {
        private readonly HttpClientWithOptions httpClient = new HttpClientWithOptions();

        [Trait("Category", "Production")]
        [Trait("Category", "System")]
        [Fact]
        public async Task ShouldSetTokenOnGetRequests()
        {
            var response = await httpClient.GetAsync("404"); // Any GET will work, even a 404

            Assert.True(response.Headers.TryGetValues("Set-Cookie", out var cookie));
            
            Assert.Single(cookie);
            
            Assert.StartsWith("CSRF=", cookie.Single());
            Assert.EndsWith("; path=/; samesite=strict", cookie.Single());
        }
    }
}