using BlogDemo.Core.Security.Permissions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using BlogDemo.Core.Security;

namespace BlogDemo.Infrastructure.Resources.Roles
{
    public class EditRoleViewModel
    {
        public string Name { get; set; }
        public IDictionary<string, IEnumerable<Permission>> RoleCategoryPermissions { get; set; }
        public IEnumerable<string> EffectivePermissions { get; set; }

        [BindNever]
        public Role Role { get; set; }
    }
}
