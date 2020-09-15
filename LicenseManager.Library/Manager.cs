using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using libc.hwid;

namespace LicenseManager.Library
{
    public static class Manager
    {
        private static string serverUrl;
        private static int productId;

        public static void Init(string serverUrl, int productId)
        {
            Manager.serverUrl = serverUrl + "/Api/CheckLicense";
            Manager.productId = productId;
        }

        public static async Task<VerifyResult> Verify(string key)
        {
            VerifyResult result = null;

            try
            {
                using var client = new HttpClient();
                var response = await client.SendAsync(BuildRequest(key)).ConfigureAwait(false);

                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                result = await JsonSerializer.DeserializeAsync<VerifyResult>(stream).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result = new VerifyResult() { Success = false, ErrorLog = ex.ToString() };
            }

            return result;
        }

        private static HttpRequestMessage BuildRequest(string key)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, serverUrl)
            {
                Content = new StringContent(PostContent(key), Encoding.UTF8, "application/json")
            };

            message.Headers.Add("User-Agent", $"LicenseManager-Library version {Assembly.GetExecutingAssembly().GetName().Version:3}");

            return message;
        }

        private static string PostContent(string key)
        {
            var payload = new
            {
                Key = key,
                Hwid = HwId.Generate(),
                Product = productId
            };

            return JsonSerializer.Serialize(payload);
        }
    }
}
