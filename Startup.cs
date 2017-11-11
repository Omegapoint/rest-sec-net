using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Demo
{
    public class Startup
    {
        private readonly JwksStore store = new JwksStore();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    var signingKeys = store.GetSigningKeys();

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "urn:omegapoint:opkoko",
                        ValidAudience = "urn:omegapoint:presentation",
                        IssuerSigningKeys = signingKeys
                    };
                });

            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
