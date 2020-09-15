namespace LicenseManager.Server.Models
{
    public class LicenseCheckResponseModel
    {
        public bool Success { get; set; }
        public string Product { get; set; }
        public string Owner { get; set; }
        public string ErrorLog { get; set; }
    }
}
