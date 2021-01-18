using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Clients
{
    // Double Submit Cookie design from:
    // https://cheatsheetseries.owasp.org/cheatsheets/Cross-Site_Request_Forgery_Prevention_Cheat_Sheet.html
    public class AntiForgeryMiddleware
    {
        // Mitigate sub-domain attacks with the cookie name prefix "__Host-"
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie#Directives
        // https://tools.ietf.org/html/draft-west-cookie-prefixes-05
        private const string CookieName = "__Host-CSRF";
        private static readonly string HeaderName = $"X-CSRF-Token";

        private readonly RequestDelegate next;

        public AntiForgeryMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsReadMethod(context.Request.Method))
            {
                // A read request is always the start of a web application.
                // We use this opportunity to create the CSRF token if not
                // already present.
                if (!context.Request.Cookies.ContainsKey(CookieName))
                {
                    var token = CreateToken();

                    var options = new CookieOptions 
                    {
                        // Must be reachable by JavaScript, this is the whole point of the double
                        // submit pattern.
                        HttpOnly = false,
                        
                        // Useful if developers work in HTTP, but needs production mitigation.
                        // If production environment is limited to only HTTPS, then this is OK.
                        // We prefer if developers work in HTTPS and this is fixed to true.
                        Secure = context.Request.IsHttps,
                        
                        SameSite = SameSiteMode.Strict,
                        
                        // The user cannot choose to reject CSRF tokens
                        IsEssential = true
                    };

                    // Return the token as-is (stateless implementation).  If we accept
                    // storing state, we could use a secret that is known only to this
                    // implementation (e.g. encryption or salt-based hash.)
                    context.Response.Cookies.Append(CookieName, token, options);
                }

                await next(context); // Read operations are always OK

                return;
            }

            var cookie = context.Request.Cookies[CookieName];

            if (!IsValidToken(context, cookie))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                // Abort the pipeline
                return;
            }

            await next(context);
        }

        private static bool IsReadMethod(string method)
        {
            return method.ToUpperInvariant() switch
            {
                "GET"     => true,
                "HEAD"    => true,
                "OPTIONS" => true,
                "TRACE"   => true,
                "CONNECT" => true,
                
                _ => false
            };
        }

        private static string CreateToken()
        {
            // The CSRF token is considered a secret.  Assuming that an attacker has
            // the capability to guess CSRF secrets at offline capacity, we need at
            // least 96 bits of entropy, or 32 bytes (32 * log 8 / log 2 = 96)
            //
            // https://tools.ietf.org/html/rfc4086
            // http://en.wikipedia.org/wiki/Password_strength
            const int Size = 32;

            using var provider = new RNGCryptoServiceProvider();

            var token = new byte[Size];
            provider.GetBytes(token);

            return Base64UrlTextEncoder.Encode(token);
        }

        private static bool IsValidToken(HttpContext context, string cookie)
        {
            if (string.IsNullOrEmpty(cookie) || context.Request?.Headers == null)
            {
                return false;
            }

            return string.Equals(context.Request.Headers[HeaderName], cookie, StringComparison.Ordinal);
        }
    }
}