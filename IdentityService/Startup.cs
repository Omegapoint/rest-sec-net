using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ICorsPolicyService, CorsPolicyService>();
            services.AddSingleton<IProfileService, ProfileService>();

            services.AddTransient<IClientStore, ClientStore>();
            services.AddTransient<IResourceStore, ResourceStore>();

            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddProfileService<ProfileService>()
                .AddClientStore<ClientStore>()
                .AddResourceStore<ResourceStore>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
