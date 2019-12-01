using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SecureByDesign.Host
{
    internal class ClaimsTransformation : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            await Task.Delay(0);

            if (principal.Identity.IsAuthenticated)
            {
                var identity = new ClaimsIdentity(principal.Identity);

                // This sample will just add hard-coded claims to any authenticated
                // user, but a real example would of course instead use a local
                // account database to get information about what organization and
                // local permissions to add.

                // Transform scope and identity to local claims, for example:
                identity.AddClaim(new Claim(ClaimSettings.UrnLocalOrganizationId, "42"));

                // Lookup local permissions
                identity.AddClaim(new Claim(ClaimSettings.UrnLocalPermissionA, "true"));
                identity.AddClaim(new Claim(ClaimSettings.UrnLocalPermissionB, "true"));
                identity.AddClaim(new Claim(ClaimSettings.UrnLocalProductId, "abc"));

                return new ClaimsPrincipal(identity);
            }

            return principal;
        }
    }
}