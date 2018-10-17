using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Infrastructure.Resources.Roles
{
    public class RolesViewModel
    {
        public List<RoleEntry> RoleEntries { get; set; } = new List<RoleEntry>();
    }

    public class RoleEntry
    {
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}
