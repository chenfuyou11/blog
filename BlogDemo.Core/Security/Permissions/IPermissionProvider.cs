using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Core.Security.Permissions
{
    public interface IPermissionProvider
    {
        IEnumerable<Permission> GetPermissions();
        IEnumerable<PermissionStereotype> GetDefaultStereotypes();
    }

    public class PermissionStereotype
    {
        public string Name { get; set; }
        public IEnumerable<Permission> Permissions { get; set; }
    }
}
