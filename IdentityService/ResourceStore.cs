using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityService
{
    public class ResourceStore : IResourceStore
    {
        private readonly Dictionary<string, ApiResource> apiResources = new Dictionary<string, ApiResource>();
        private readonly Dictionary<string, IdentityResource> identityResources = new Dictionary<string, IdentityResource>();

        public ResourceStore()
        {
            apiResources["https://example.com/api"] = 
                new ApiResource("https://example.com/api")
                {
                    UserClaims = { JwtClaimTypes.Email },
                    Scopes = { 
                        new Scope { Name = "read:product", DisplayName = "Read access to product data" }
                    }
                };

            identityResources["openid"] = new IdentityResources.OpenId();
            identityResources["email"] = new IdentityResources.Email();
            identityResources["address"] = new IdentityResources.Address();
            identityResources["phone"] = new IdentityResources.Phone();
            identityResources["profile"] = new IdentityResources.Profile();
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return apiResources.ContainsKey(name)
                ? Task.FromResult(apiResources[name])
                : Task.FromResult(default(ApiResource));
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var result = apiResources.Values
                .Where(resource => resource.Scopes
                    .Select(scope => scope.Name)
                    .Intersect(scopeNames)
                    .Any());

            return Task.FromResult(result);
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var result = scopeNames
                .Where(name => identityResources.ContainsKey(name))
                .Select(name => identityResources[name]);

            return Task.FromResult(result);
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var result = new Resources(identityResources.Values, apiResources.Values); 

            return Task.FromResult(result);
        }
    }
}