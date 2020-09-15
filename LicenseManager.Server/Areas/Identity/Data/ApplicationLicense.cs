using System;

namespace LicenseManager.Server.Areas.Identity.Data
{
    public class ApplicationLicense
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Hwid { get; set; }
        public bool Active { get; set; } = false;
        public DateTime? RedeemDate { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;

        public ApplicationProduct Product { get; set; }
        public ApplicationUser User { get; set; }
    }
}
