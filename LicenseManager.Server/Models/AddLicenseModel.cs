using System.ComponentModel.DataAnnotations;

namespace LicenseManager.Server.Models
{
    public class AddLicenseModel
    {
        [Required, Range(1, 200)]
        public int Count { get; set; }

        [Required, Key]
        public int Id { get; set; }
    }
}
