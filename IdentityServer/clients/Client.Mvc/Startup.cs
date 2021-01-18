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
    //Based on sample from https://identitymodel.readthedocs.io/en/latest/
    public class Startup
    {
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
                    options.Authority = "https://demo.identityserver.io";

                    options.ClientId = "interactive.confidential.short";
                    options.ClientSecret = "secret";

                    // code flow + PKCE (PKCE is turned on by default)
                    options.ResponseType = "code";
                    options.UsePkce = true;
                    //Since we use code+PKCE we can also use query and avpid cookie issues with SameSite-policies for OIDC/OAuth flows
                    options.ResponseMode = "query"; 

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("api"); //If needed, add any scope needed api-access

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
                            //Often we whant to init the login UI from the request by using ctx.HttpContext.Request
                            //ctx.ProtocolMessage.LoginHint = "tobias.ahnoff+1@omegapoint.se";
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
                    options.Client.Scope = "api";
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
                client.BaseAddress = new Uri("https://demo.identityserver.io/api/");
            });

            // registers HTTP client that uses the managed client access token
            services.AddClientAccessTokenClient("client", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://demo.identityserver.io/api/");
            });

            // registers a typed HTTP client with token management support
            services.AddHttpClient<TypedUserClient>(client =>
            {
                client.BaseAddress = new Uri("https://demo.identityserver.io/api/");
            })
                .AddUserAccessTokenHandler();

            services.AddHttpClient<TypedClientClient>(client =>
            {
                client.BaseAddress = new Uri("https://demo.identityserver.io/api/");
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });

            //TODO: Add csrf protection
        }
    }
}