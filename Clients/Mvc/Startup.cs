using Clients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MvcCode
{
    /// <summary>
    /// Based on sample from https://identitymodel.readthedocs.io/en/latest/, where we added some commonly used security and identity features:
    /// - CSRF protection, using anti forgery tokens (double submit cookie pattern from https://cheatsheetseries.owasp.org/cheatsheets/Cross-Site_Request_Forgery_Prevention_Cheat_Sheet.html)
    /// - Host prefix cookie name (see more on https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie)
    /// - HSTS configuration, note that other security headers should be added but this is often not part of the application code and not part of this repo (see more on https://securityheaders.com/) 
    /// </summary>

    public class Startup
    {
        private const string Authority = "https://localhost:5009";
        private const string ApiBaseUrl = "https://localhost:5001";

        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddControllersWithViews();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "__Host-client-mvc";
                    
                    options.Events.OnSigningOut = async e =>
                    {
                        await e.HttpContext.RevokeUserRefreshTokenAsync();
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = Authority;

                    options.ClientId = "mvc";
                    options.ClientSecret = "secret";

                    // code flow + PKCE (PKCE is turned on by default)
                    options.ResponseType = "code";
                    options.UsePkce = true;
                    
                    options.Scope.Clear();
                    //OIDC
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("offline_access");
                    //Custom
                    options.Scope.Add("products.read products.write");

                    // not mapped by default
                    options.ClaimActions.MapJsonKey("website", "website");

                    // keeps id_token smaller
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };

                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = ctx =>
                        {
                            //Often we whant to initialize the login UI from the request by using ctx.HttpContext.Request.
                            //Here we just hard code it to a known Identity Server test user (with password alice).
                            ctx.ProtocolMessage.LoginHint = "Alice";
                            //ctx.ProtocolMessage.UiLocales = "sv";

                            return Task.CompletedTask;
                        }
                    };
                });

            // adds user and client access token management
            services.AddAccessTokenManagement(options =>
                {
                    // client config is inferred from OpenID Connect settings
                    // if you want to specify scopes explicitly, do it here, otherwise the scope parameter will not be sent
                    //options.Client.Scope = "products.read";
                })
                .ConfigureBackchannelHttpClient()
                    .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
                    {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(3)
                    }));

            // registers HTTP client that uses the managed user access token
            services.AddUserAccessTokenClient("user_client", configureClient: client =>
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
            });

            // registers HTTP client that uses the managed client access token
            services.AddClientAccessTokenClient("client", configureClient: client =>
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
            });

            // registers a typed HTTP client with token management support
            services.AddHttpClient<TypedUserClient>(client =>
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
            })
                .AddUserAccessTokenHandler();

            services.AddHttpClient<TypedClientClient>(client =>
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
            })
                .AddClientAccessTokenHandler();

            services.AddHsts(options =>
            {
                options.Preload = false;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromSeconds(31536000); //One year in seconds
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseHsts();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<AntiForgeryMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });
        }
    }
}