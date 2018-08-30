using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TokenService
{
    [Route("token")]
    public class TokenController : Controller
    {
        private readonly LocalCertificateStore certificateStore;

        public TokenController(LocalCertificateStore certificateStore)
        {
            this.certificateStore = certificateStore;
        }

        [HttpPost]
        public IActionResult Authorize()
        {
            var expiresIn = TimeSpan.FromMinutes(10);
            var notBefore = DateTime.UtcNow;
            var expires = DateTime.UtcNow.Add(expiresIn);

            var claims = new []
            {
                new Claim("client_id", "unknown"),
                new Claim("scope", "read:product"), 
                new Claim(ClaimTypes.Name, "Martin Altenstedt"),
                new Claim("urn:omegapoint:age", "49")
            };

            var token = new JwtSecurityToken(
                "urn:omegapoint:opkoko",
                "urn:omegapoint:presentation",
                claims,
                notBefore,
                expires,
                certificateStore.SigningCredentials
            );

            return Ok(new AuthorizeResponseDataContract
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresIn = (int)expiresIn.TotalSeconds,
                TokenType = "bearer"
            });
        }
    }

    [DataContract]
    public class AuthorizeResponseDataContract 
    {
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }
    }
}