using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace TokenService
{
    public class LocalCertificateStore
    {
        public LocalCertificateStore()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var subjectDistinguishedName = "CN=My OAuth2 Identity";

            try 
            {
                var certificates = store
                    .Certificates
                    .Find(X509FindType.FindBySubjectDistinguishedName, subjectDistinguishedName, false);

                var securityKeys = new List<SecurityKey>();

                foreach (var certificate in certificates)
                {
                    var securityKey = new X509SecurityKey(certificate);
                    securityKeys.Add(securityKey);
                }

                SecurityKeys = securityKeys;
                SigningCredentials = new SigningCredentials(securityKeys[0], SecurityAlgorithms.RsaSha256Signature);
            } 
            finally 
            {
                store.Close();
            }
        }

        public SigningCredentials SigningCredentials { get; private set; }

        public IEnumerable<SecurityKey> SecurityKeys { get; private set; }
    }
}