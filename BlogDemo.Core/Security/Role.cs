using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Core.Security
{
    public class Role : IRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public List<RoleClaim> RoleClaims { get; } = new List<RoleClaim>();
    }
}
