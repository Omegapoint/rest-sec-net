using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;

namespace IdentityService
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly string[] allowedOrigins = { "http://localhost:3000" };

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            var result = allowedOrigins.Contains(origin);

            return Task.FromResult(result);
        }
    }
}