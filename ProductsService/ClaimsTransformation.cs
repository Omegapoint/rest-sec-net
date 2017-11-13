using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Demo
{
    internal class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = new ClaimsIdentity(principal.Identity);
            identity.AddClaim(new Claim("urn:local:feature", "reports, statistics"));

            var result = new ClaimsPrincipal(identity);

            return Task.FromResult(result);
        }
    }
}