using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TokenService
{
    /// <summary>
    /// Represents a JWK Set structure.
    /// https://tools.ietf.org/html/rfc7517#section-5
    /// </summary>
    [DataContract]
    public class JwksDataContract
    {
        /// <summary>
        /// Gets or sets a value representing the JWKs that this 
        /// set contains.
        /// </summary>
        [DataMember(Name="keys")]
        public IEnumerable<JwkDataContract> Keys { get; set; }
    }
}