using System.Collections.Generic;

namespace SecureByDesign.Host.Domain.Model
{
    public class Permissions
    {
        public List<ServicePermission> GrantedServicePermissions {get; set;}  = new List<ServicePermission>();
        public List<ProductId> GrantedProducts {get; set;} = new List<ProductId>();
    }

    public enum ServicePermission
    {
       ProductRead,
       ProductWrite
    }
}