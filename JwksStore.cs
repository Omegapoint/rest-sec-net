using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Demo
{
    public class JwksStore
    {
        private readonly string url = "https://opkoko-identity.azurewebsites.net/.well-known/jwks";

        public IEnumerable<SecurityKey> GetSigningKeys()
        {
            var client = new HttpClient();
            var response = client.GetAsync(url).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var jwks = JsonConvert.DeserializeObject<Jwks>(content);

            var securityKeys = new List<SecurityKey>();

            foreach (var jwk in jwks.Keys)
            {
                if (jwk.Kty == "RSA") 
                {
                    var parameters = new RSAParameters();
                    parameters.Exponent = jwk.E.FromBaseUrlSafeString();
                    parameters.Modulus = jwk.N.FromBaseUrlSafeString();

                    var securityKey = new RsaSecurityKey(parameters);

                    securityKeys.Add(securityKey);
                }
            }

            return securityKeys;
        }
    }

    public class Jwks
    {
        public IEnumerable<Jwk> Keys { get; set; }
    }

    public class Jwk
    {
        public string Kty { get; set; }

        public string E { get; set; }

        public string N { get; set; }

        public string Alg { get; set; }        
    }

    public static class Base64Extensions
    {
        public static byte[] FromBaseUrlSafeString(this string value) 
        {
            string text = value
                .Replace('_', '/')
                .Replace('-', '+');

            switch(value.Length % 4) {
                case 2: 
                    return Convert.FromBase64String(text + "=="); 
                case 3: 
                    return Convert.FromBase64String(text + "="); 
            }

            return Convert.FromBase64String(text);            
        }
    }
   
}