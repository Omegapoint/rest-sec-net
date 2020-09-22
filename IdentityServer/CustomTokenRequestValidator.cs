using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace IdentityServer
{
    // If you need to add custom CLIENT claims in the client credentials
    // flow, you need to use a ICustomTokenRequestValidator.
    // 
    // This is only applicable to the client credentials flow.
    // 
    // Please note that client claims are differently configured from USER
    // claims.  If you want to add custom USER claims, you need to use
    // IProfileService.
    public class CustomTokenRequestValidator : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            context.Result.ValidatedRequest.ClientClaims.Add(new Claim("my:client", "is:special"));


            return Task.CompletedTask;
        }
    }
}