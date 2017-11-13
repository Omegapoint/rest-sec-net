using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Demo
{
    internal class MinimalAgeRequirement : AuthorizationHandler<IAuthorizationRequirement>, IAuthorizationRequirement
    {
        private int age;

        public MinimalAgeRequirement(int age)
        {
            this.age = age;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
        {
            var ageClaimValue = context
                .User
                .Claims
                .FirstOrDefault(claim => claim.Type == "urn:omegapoint:age")
                ?.Value;

            if (int.TryParse(ageClaimValue, out var age) && age >= this.age)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            
            return Task.CompletedTask;
        }
    }
}