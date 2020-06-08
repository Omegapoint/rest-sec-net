using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SecureByDesign.Host.Domain.Services;
using SecureByDesign.Host.Infrastructure;

namespace SecureByDesign.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config => {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddMemoryCache();
            services.AddSingleton<IClaimsTransformation, ClaimsTransformation>();
            services.AddScoped<IAccessControlService, AccessControlService>();
            services.AddScoped<IPermissionsRepository, PermissionsInMemoryRepository>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<IProductsRepository, ProductsInMemoryRepository>();
            services.AddScoped<ILoggingService, CentralizedLoggingService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false; // Only for testing purposes
                    options.Authority = "http://localhost:5000/";
                    options.Audience = "products";
                });
            services.AddAuthorization(options =>
            {
                //Assert that we always require JWT autentication (except when opt-out with AllowAnonymous attribute)
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Note that order is important and that: UseAuthentication and UseAuthorization should keept together and between UseRouting and UseEndpoints 
            app.UseHsts();
            app.UseHeaders();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
