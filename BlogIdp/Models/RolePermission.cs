using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogIdp.Models
{
    /// <summary>
    /// 用户权限
    /// </summary>
    public class RolePermission 
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string RoleName
        { get; set; }
        /// <summary>
        /// 请求Url
        /// </summary>
        public string Url
        { get; set; }

        public int Id { get; set; }
    }
}
