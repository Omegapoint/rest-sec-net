using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.Serialization;
using System;

namespace TokenService
{
    [Route("jwks")]
    public class JwksController : Controller
    {
        private readonly LocalCertificateStore certificateStore;

        public JwksController(LocalCertificateStore certificateStore)
        {
            this.certificateStore = certificateStore;
        }

        [HttpGet]
        public IActionResult GetAllCertificates()
        {
            var result = new List<JwkDataContract>();

            foreach (var securityKey in certificateStore.SecurityKeys)
            {
                if (securityKey is X509SecurityKey key)
                {
                    var rsa = key.PublicKey as RSA;
                    var parameters = rsa.ExportParameters(false);

                    var algorithm = certificateStore.SigningCredentials.Algorithm;

                    var jwk = new JwkDataContract
                    {
                        KeyType = "RSA",
                        PublicKeyUse = "sig",
                        KeyId = key.KeyId,
                        X509Thumbprint = key.Certificate.GetCertHash().ToBase64UrlSafeString(),
                        Exponent = parameters.Exponent.ToBase64UrlSafeString(),
                        Modulus = parameters.Modulus.ToBase64UrlSafeString(),
                        Algorithm = algorithm
                    };

                    result.Add(jwk);
                }
            }

            return Ok(new JwksDataContract { Keys = result });
        }
    }

    internal static class Base64Extensions
    {
        public static string ToBase64UrlSafeString(this byte[] bytes) 
        {
            return Convert
                .ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');

        }
    }    
}