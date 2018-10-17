using BlogDemo.Core.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogDemo.Core.Security
{
    public class StandardPermissions
    {
        public static readonly Permission SiteOwner = new Permission("SiteOwner", "Site Owners Permission");
    }
}
