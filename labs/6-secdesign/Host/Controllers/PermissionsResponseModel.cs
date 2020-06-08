using System.Collections.Generic;
using SecureByDesign.Host.Domain.Model;

namespace SecureByDesign.Host.Controllers
{
    public class PermissionsResponseModel
    {
        public List<ServicePermission> GrantedServicePermissions {get; set;} 
        public List<ProductId> GrantedProducts {get; set;} 
    }

    public static class PermissionsResponseModelExtension
    {
        public static PermissionsResponseModel MapToPermissionsResponseModel(this Permissions permissions){
            return new PermissionsResponseModel{ 
                GrantedServicePermissions = new List<ServicePermission>(permissions.GrantedServicePermissions), 
                GrantedProducts = new List<ProductId>(permissions.GrantedProducts) };
        }
    }
}