using System;
using System.Collections.Generic;

namespace LicenseManager.Server.Areas.Identity.Data
{
    public class ApplicationProduct
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Suspended { get; set; } = false;
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public virtual ICollection<ApplicationLicense> Licenses { get; set; }
    }
}
