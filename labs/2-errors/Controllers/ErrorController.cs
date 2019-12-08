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
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Get()
        {
            throw new InvalidOperationException("This message might contain sensitive information.");
        }
    }
}
