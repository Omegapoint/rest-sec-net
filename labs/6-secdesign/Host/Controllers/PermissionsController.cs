using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureByDesign.Host.Domain.Services;

namespace SecureByDesign.Host.Controllers
{
    [ApiController]
    [Route("/api/permissions")]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IAccessControlService accessControlService;

        public PermissionsController(IAccessControlService accessControlService)
        {
            this.accessControlService = accessControlService;
        }

        [HttpGet("")]
        public async Task<ActionResult<Permissions>> GetPermissions()
        {
            var permissions = await accessControlService.GetPermissions(User);
            return Ok(permissions);
        }
    }
}