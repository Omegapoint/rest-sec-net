using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace ProductsService
{
    internal class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.HasClaim(c => c.Issuer == "LocalClaimsTransformer") && principal.Identity.IsAuthenticated)
            {
                var identity = new ClaimsIdentity(principal.Identity);

                var localCustomerId = GetInternalCustomerId(identity.Claims.Single(c => c.Type == "client_id").Value);
                identity.AddClaim(new Claim("urn:local:customerId", localCustomerId, ClaimValueTypes.String, "LocalClaimsTransformer"));
                
                var result = new ClaimsPrincipal(identity);

                return Task.FromResult(result);
            }

            return Task.FromResult(principal);
        }

        private string GetInternalCustomerId(string userName)
        {
            return "Customer1";
        }
    }
}