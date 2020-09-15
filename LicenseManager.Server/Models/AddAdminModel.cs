using System.ComponentModel.DataAnnotations;

namespace LicenseManager.Server.Models
{
    public class AddAdminModel
    {
        [Required, Key]
        public string Id { get; set; }
    }
}
