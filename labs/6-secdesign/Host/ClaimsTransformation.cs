using System;
using System.Collections.Generic;
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

                // The idnetity of the user can either be from sub or (depending on IdP) from client_id.
                // We transform to a local id claim which will be associeated with a permission set later on 
                AddIdentityClaim(identity);
                
                // It is important to honor any scope that affect our domain, but even if we have scope claim(s) in our pricipal
                // it is good to transform them to applicaion specific claims in order to avoid any dependencies to scope values later on.
                AddScopeClaims(identity);

                // Often authorization is based on username (sub claim) and needs to be looked up from permission configuration data (in a database).
                // For more complex scenarios this is be done in a dedicated service, but it is also common to do it here and use the ClaimsPrincipal 
                // as a persissions cache (using claims to represent the permissions).
                // This example uses a Access control service, instead of making the look up call here. 
                
                return new ClaimsPrincipal(identity);
            }

            return principal;
        }

        private void AddIdentityClaim(ClaimsIdentity identity)
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

        private void AddScopeClaims(ClaimsIdentity identity)
        {
            var scopeClaims = identity.Claims.Where(c => c.Value == "scope").ToList(); 
            
            // Note that depending on IdP we might get a single scope claim with a space-separated list instead of all scopes as individual claims
            // (the JWT-middleware creates individual claims if scope claim in the token was a comma-separated list)
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