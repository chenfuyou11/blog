using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogDemo.Core.Security.Services
{
    public interface IRoleProvider
    {
        Task<IEnumerable<string>> GetRoleNamesAsync();
    }
}
