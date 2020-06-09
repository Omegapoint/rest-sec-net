using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SecureByDesign.Host
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            await Task.CompletedTask;

            if (principal.Identity.IsAuthenticated)
            {
                var identity = new ClaimsIdentity(principal.Identity);

                // The identity of the user can either be from sub or (depending on IdP) from client_id.
                // We transform to a local id claim which will be associeated with a permission set later on 
                TransformToIdentityClaim(identity);
                
                // Depending on IdP we might get a single scope claim with a space-separated list instead of all scopes as individual claims
                // (the ASP:NET Core JWT-middleware creates individual claims if the scope claim in the token was a comma-separated list).
                TransformToScopeClaims(identity);

                // Often authorization is based on username (sub claim) and needs to be looked up from permission configuration data (in a database).
                // For more complex scenarios this is done in a dedicated service, but it is a common pattern to do it here and use the 
                // ClaimsPrincipal as a persissions cache (using claims to represent the permissions).
                // This example uses a Access control service, instead of making the look up call here.
                // AddPermissions(identity); 
                
                return new ClaimsPrincipal(identity);
            }

            return principal;
        }

        // Note that we keep the claim values as identities, thus we assume that sub and client_id are unique.
        // If not we need to handel this differently e g add a suffix (making them unique) or look up the 
        // unique identity (either here or in the Access Control service later on).
        // For example, if sub is a "person number" then we might want to transform that to a customer id as 
        // early as possible (here, if not done by the IdP).
        private void TransformToIdentityClaim(ClaimsIdentity identity)
        {
            var id = string.Empty;
            
            if(identity.HasClaim(c => c.Type == "sub" && !string.IsNullOrWhiteSpace(c.Value)))
            {
                id = identity.Claims.Single(c => c.Type == "sub").Value;
                identity.AddClaim(new Claim(ClaimSettings.UrnLocalIdentity, id));
                return;
            }

            if(identity.HasClaim(c => c.Type == "client_id"))
            {
                id = identity.Claims.Single(c => c.Type == "client_id" && !string.IsNullOrWhiteSpace(c.Value)).Value;
                identity.AddClaim(new Claim(ClaimSettings.UrnLocalIdentity, id));
                return;
            }
        }

        // Note that we do not transform the scope claim values, thus we will have dependencies to this later on in the Access Control service.
        // If that is hard to adminstrate, it might be better to transform to local/internal scope values we do not have any depndencies to
        // IdP and protocol specific details after claims transformation.
        private void TransformToScopeClaims(ClaimsIdentity identity)
        {
            var scopeClaims = identity.Claims.Where(c => c.Value == "scope").ToList(); 
            
            if(scopeClaims.Count == 1){
                var scopeStringList = scopeClaims[0].Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (scopeStringList != null && scopeStringList.Count() > 1) {
                    scopeClaims.Clear();
                    foreach (var scopeValue in scopeStringList) {
                        scopeClaims.Add(new Claim("scope", scopeValue));
                    }
                }
            }
        }
    }
}