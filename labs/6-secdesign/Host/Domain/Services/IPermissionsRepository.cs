using System.Collections.Generic;
using System.Threading.Tasks;
using SecureByDesign.Host.Domain.Model;

public interface IPermissionsRepository
{
    public Task<Permissions> GetPermissions(AccessControlParameters accessControlParameters);
}