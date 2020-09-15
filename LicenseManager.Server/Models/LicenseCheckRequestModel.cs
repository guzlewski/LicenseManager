namespace LicenseManager.Server.Models
{
    public class LicenseCheckRequestModel
    {
        public string Key { get; set; }
        public string Hwid { get; set; }
        public int Product { get; set; }
    }
}
