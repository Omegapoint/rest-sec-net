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

    // https://tools.ietf.org/html/rfc7517
    [DataContract]
    public class JwkDataContract
    {
        [DataMember(Name="kty")]
        public string KeyType { get; set; }

        [DataMember(Name="use")]
        public string PublicKeyUse { get; set; }

        [DataMember(Name="kid")]
        public string KeyId { get; set; }

        [DataMember(Name="x5t")]
        public string X509Thumbprint { get; set; }

        [DataMember(Name="e")]
        public string Exponent { get; set; }

        [DataMember(Name="n")]
        public string Modulus { get; set; }

        [DataMember(Name="Alg")]
        public string Algorithm { get; set; }        
    }

    // https://tools.ietf.org/html/rfc7517#section-5
    [DataContract]
    public class JwksDataContract
    {
        [DataMember(Name="keys")]
        public IEnumerable<JwkDataContract> Keys { get; set; }
    }

    public static class Base64Extensions
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