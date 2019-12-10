using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Errors.Controllers
{
    [ApiController]
    [Route("/api/error")]
    public class ErrorController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Throw()
        {
            throw new InvalidOperationException("This message might contain sensitive information.");
        }
    }
}
