using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Headers.Host
{
    public class HeadersMiddleware
    {
        private readonly RequestDelegate next;
        
        public HeadersMiddleware(RequestDelegate next) {
            this.next = next;
        }

        public async Task Invoke(HttpContext context) {
            context.Response.OnStarting((state) => {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

                // Please note that Kestrel will add this header back.
                // Update class Program to configure Kestrel.
                context.Response.Headers.Remove("Server");

                return Task.CompletedTask;
            }, null);

            await next(context);
        }
    }

    public static class HeadersMiddlewareExtensions  {
        public static IApplicationBuilder UseHeaders(this IApplicationBuilder app) {
            return app.UseMiddleware<HeadersMiddleware>();
        }
    }
}

