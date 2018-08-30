using System.Runtime.Serialization;

namespace TokenService
{
    /// <summary>
    /// Represents a JWK structure.
    /// https://tools.ietf.org/html/rfc7517
    /// </summary>
    [DataContract]
    public class JwkDataContract
    {
        /// <summary>
        /// Gets or sets a value representing the cryptographic algorithm
        /// family used with the key, such as "RSA" or "EC".
        /// </summary>
        [DataMember(Name="kty")]
        public string KeyType { get; set; }

        /// <summary>
        /// Gets or sets a value representing the intended use of
        /// the public key, such as "sig" (signature), or "enc" 
        /// (encryption).
        /// </summary>
        [DataMember(Name="use")]
        public string PublicKeyUse { get; set; }

        /// <summary>
        /// Gets or sets a value that is used to match a specific key,
        /// for instance, to choose among a set of keys within a JWK Set
        /// during key rollover.
        /// </summary>
        [DataMember(Name="kid")]
        public string KeyId { get; set; }

        /// <summary>
        /// Gets or sets a value representing a base64url-encoded
        /// SHA-1 thumbprint (a.k.a. digest) of the DER encoding 
        /// of an X.509 certificate [RFC5280].
        /// </summary>
        [DataMember(Name="x5t")]
        public string X509Thumbprint { get; set; }

        /// <summary>
        /// Gets or sets a value representing the exponent value for the RSA
        /// public key
        /// </summary>
        [DataMember(Name="e")]
        public string Exponent { get; set; }

        /// <summary>
        /// Gets or sets a value representing the modulus value for the RSA
        /// public key
        /// </summary>
        [DataMember(Name="n")]
        public string Modulus { get; set; }

        /// <summary>
        /// Gets or sets a value representing the algorithm intended for
        /// use with the key
        /// </summary>
        [DataMember(Name="Alg")]
        public string Algorithm { get; set; }        
    }
}