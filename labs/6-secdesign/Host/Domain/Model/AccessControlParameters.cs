using System.Collections.Generic;
using System.Linq;

namespace SecureByDesign.Host.Domain.Model
{
    public class AccessControlParameters
    {
        public string Identity {get; private set;} 
        public List<string> Scopes {get; private set;} 

        public AccessControlParameters(string identity, List<string> scopes)
        {
            AssertValidIdentity(identity);
            AssertValidScopes(scopes);
            Identity = identity;
            Scopes = new List<string>(scopes);
        }
        
        public string ToCacheKey(){
            return $"{Identity}|{string.Join('|', Scopes)}";
        }

        public static bool IsValidIdentity(string identity)
        {
            return !string.IsNullOrEmpty(identity) && identity.Length < 50;
        }

        public static void AssertValidIdentity(string identity)
        {
            if (!IsValidIdentity(identity))
            {
                throw new InvalidProductNameArgumentException($"The identity is not valid.");
            }
        }

        public static bool IsValidScope(string scope)
        {
            return !string.IsNullOrEmpty(scope) && scope.Length < 50;
        }

        public static void AssertValidScopes(List<string> scopes)
        {
            if (!scopes.All(IsValidScope))
            {
                throw new InvalidProductNameArgumentException($"The scopes are not valid.");
            }
        }
    }
}