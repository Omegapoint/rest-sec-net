using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SecureByDesign.Host
{
    internal class ClaimsTransformation : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            await Task.CompletedTask;

            if (principal.Identity.IsAuthenticated)
            {
                var identity = new ClaimsIdentity(principal.Identity);

                // This sample will just add hard-coded claims to any authenticated
                // user, but a real example would of course instead use a local
                // account database to get information about what organization and
                // local permissions to add.

                // It is important to honor any scope that affect our domain
                AddClaimIfScope(identity, "products.read",  new Claim(ClaimSettings.UrnLocalProductRead,  "true"));
                AddClaimIfScope(identity, "products.write", new Claim(ClaimSettings.UrnLocalProductWrite, "true"));

                // Often authorization is based on username (sub claim) and needs to be looked up from 
                // permission configuration data (in a database)
                LookupUserPermissions(identity);
                
                return new ClaimsPrincipal(identity);
            }

            return principal;
        }

        private void AddClaimIfScope(ClaimsIdentity identity, string scope, Claim claim)
        {
            if (identity.Claims.Any(c => c.Type == "scope" && c.Value == scope))
            {
                identity.AddClaim(claim);
            }
        }

        private void LookupUserPermissions(ClaimsIdentity identity){
            //Get permission based on: identity.Claims.Single(c => c.Type = "sub")
            identity.AddClaim(new Claim(ClaimSettings.UrnLocalProductIds, "abc,def"));
        }
    }
}