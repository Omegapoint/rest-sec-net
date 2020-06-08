using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureByDesign.Host.Domain.Model;
using SecureByDesign.Host.Domain.Services;

namespace SecureByDesign.Host.Controllers
{
    [ApiController]
    [Route("/api/user-info")]
    [Authorize]
    public class UserInfoController : ControllerBase
    {
        private readonly IAccessControlService accessControlService;

        public UserInfoController(IAccessControlService accessControlService)
        {
            this.accessControlService = accessControlService;
        }

        [HttpGet("permissions")]
        public async Task<ActionResult<PermissionsResponseModel>> GetPermissions()
        {
            var permissions = await accessControlService.GetPermissions(User);
            return Ok(permissions.MapToPermissionsResponseModel());
        }
    }
}