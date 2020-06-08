using System.Collections.Generic;
using System.Threading.Tasks;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Infrastructure
{
    public class PermissionsInMemoryRepository : IPermissionsRepository
    {
        private Dictionary<string, ServicePermission> _scopeMap = new Dictionary<string, ServicePermission>();
        
        public PermissionsInMemoryRepository(){
            _scopeMap.Add(ClaimSettings.ScopeProductsRead, ServicePermission.ProductRead);
            _scopeMap.Add(ClaimSettings.ScopeProductsWrite, ServicePermission.ProductWrite);
        }

        // This sample will just add hard-coded claims to any authenticated user or client, but a real example would of course 
        // instead use a local permissions database or file etc.
        // Note that identity and scopes are just two common exampels of pramaters for permission look up,
        // others are e g role, authentication method (from the acr claim), client wher the user has isgned in (azp) etc.  
        public async Task<Permissions> GetPermissions(AccessControlParameters accessControlParameters)
        {
            var permissions = new Permissions();
            AddAuthorizedScopePermissions(accessControlParameters.Scopes, permissions);
            
            //TODO: GetPermissionsFromRepo(identity)
            permissions.GrantedProducts = new List<ProductId> { new ProductId("abc"), new ProductId("def") };

            return permissions;
        }

        private void AddAuthorizedScopePermissions(List<string> scopes, Permissions permissions)
        {
            foreach (var scope in scopes)
            {
                if(!_scopeMap.ContainsKey(scope)){
                    //TODO: log warning "Unknown scope was used"
                    continue;
                }

                permissions.GrantedServicePermissions.Add(_scopeMap[scope]);
            }
        }
    }
}