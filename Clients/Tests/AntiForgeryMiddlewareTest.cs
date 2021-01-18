using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Clients.Test.Unit
{
    public class AntiForgeryMiddlewareTest
    {
        [Theory]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("PATCH")]
        [InlineData("UNKNOWN")] // Represents a verb we do not know about
        public async Task ShouldRejectEmptyNonReadRequests(string method)
        {
            var middleware = new AntiForgeryMiddleware(ctx => Task.CompletedTask);

            var context = new DefaultHttpContext();
            context.Request.Method = method;  

            await middleware.Invoke(context);
            
            Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        }
        
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData(null, "token")]
        [InlineData("", "token")]
        [InlineData("cookie", null)]
        [InlineData("cookie", "")]
        [InlineData("cookie", "not valid base64")]
        [InlineData("cookie", "<script>")]
        [InlineData("cookie", "--sql();")]
        [InlineData("cookie", "Cookie")] // Should be case sensitive
        [InlineData("cookie", "cIz973f7X9uk0VHxCbYb-AhumSxZnJ3jEJPCQP8A0k4")]
        [InlineData("cIz973f7X9uk0VHxCbYb-AhumSxZnJ3jEJPCQP8A0k4", "cIz973f7X9uk0VHxCbYb-AhumSxZnJ3jEJPCQP8A0k")] // Same start
        [InlineData("cIz973f7X9uk0VHxCbYb-AhumSxZnJ3jEJPCQP8A0k4", "cIz973f7X9uk0VHxCbYb-AhumSxZnJ3jEJPCQP8A0k4 ")] // Same start
        public async Task ShouldRejectWrongToken(string cookie, string token)
        {
            var middleware = new AntiForgeryMiddleware(ctx => Task.CompletedTask);

            var context = new DefaultHttpContext();
            context.Request.Method = HttpMethods.Post;
            context.Request.Headers["Cookie"] = $"CSRF={cookie}";
            context.Request.Headers["X-CSRF-Token"] = token;

            await middleware.Invoke(context);
            
            Assert.Equal((int)HttpStatusCode.Forbidden, context.Response.StatusCode);
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("HEAD")]
        [InlineData("CONNECT")]
        [InlineData("OPTIONS")]
        [InlineData("TRACE")]
        public async Task ShouldPassThroughReadRequests(string method)
        {
            var middleware = new AntiForgeryMiddleware(ctx => Task.CompletedTask);

            var context = new DefaultHttpContext();
            context.Request.Method = method;

            await middleware.Invoke(context);
            
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }

        [Fact]
        public async Task ShouldSetTokenOnGetRequests()
        {
            var middleware = new AntiForgeryMiddleware(ctx => Task.CompletedTask);

            var context = new DefaultHttpContext();
            context.Request.Method = HttpMethods.Get;
            context.Request.Scheme = Uri.UriSchemeHttps;

            await middleware.Invoke(context);

            var cookie = context.Response.Headers["Set-Cookie"];
            Assert.Single(cookie);
            
            Assert.StartsWith("CSRF=", cookie.Single());
            Assert.EndsWith("; path=/; secure; samesite=strict", cookie.Single());
        }
        
        [Fact]
        public async Task ShouldAcceptCorrectToken()
        {
            string token;

            { // Get the CSRF token
                var middleware = new AntiForgeryMiddleware(ctx => Task.CompletedTask);

                var context = new DefaultHttpContext();
                context.Request.Method = HttpMethods.Get;  

                await middleware.Invoke(context);

                var cookie = context.Response.Headers["Set-Cookie"];
                token = Regex.Match(cookie, "^CSRF=([a-zA-Z0-9_-]+);").Groups[1].Value;
            }

            { // Use the CSRF token
                var middleware = new AntiForgeryMiddleware(ctx => Task.CompletedTask);

                var context = new DefaultHttpContext();
                context.Request.Method = HttpMethods.Post;
                context.Request.Headers["Cookie"] = $"CSRF={token}";
                context.Request.Headers["__Host-X-CSRF-Token"] = token;

                await middleware.Invoke(context);
                
                Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
            }
        }
        
        [Fact]
        public async Task ShouldSetStrongToken()
        {
            var tokens = new List<string>();

            for (var i = 0; i < 1000; i++)
            {
                var middleware = new AntiForgeryMiddleware(ctx => Task.CompletedTask);

                var context = new DefaultHttpContext();
                context.Request.Method = HttpMethods.Get;  

                await middleware.Invoke(context);

                var cookie = context.Response.Headers["Set-Cookie"];
                var token = Regex.Match(cookie, "^CSRF=([a-zA-Z0-9_-]+);").Groups[1].Value;
                
                tokens.Add(token);
            }
            
            Assert.Equal(tokens, tokens.Distinct()); // All tokens must be unique
            Assert.All(tokens, token => Assert.True(token.Length > 40));
        }
    }
}
