using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemo.Core.Security
{
    public interface IRoleRemovedEventHandler
    {
        Task RoleRemovedAsync(string roleName);
    }
}
