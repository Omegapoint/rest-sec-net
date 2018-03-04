using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityService
{
    public class ProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            if (context.Subject.Identity.Name == "martin.altenstedt@stenaline.com")
            {
                context.IssuedClaims.Add(new Claim(JwtClaimTypes.Email, "martin.altenstedt@stenaline.com"));
            }

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;

            return Task.FromResult(0);
        }
    }
}