using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace BlogDemo.Core.Security
{
    public class RoleClaim
    {
        /// <summary>
        /// Gets or sets the claim type for this claim.
        /// </summary>
        public string ClaimType { get; set; }

        /// <summary>
        /// Gets or sets the claim value for this claim.
        /// </summary>
        public string ClaimValue { get; set; }

        public Claim ToClaim()
        {
            return new Claim(ClaimType, ClaimValue);
        }
    }
}
