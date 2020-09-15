using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace LicenseManager.Server.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime RegisterDate { get; set; } = DateTime.Now;

        public virtual ICollection<ApplicationLicense> Licenses { get; set; }
    }
}
