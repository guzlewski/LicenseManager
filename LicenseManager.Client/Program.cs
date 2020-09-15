using System;
using System.Threading.Tasks;
using LicenseManager.Library;

namespace LicenseManager.Client
{
    class Program
    {
        static async Task Main()
        {
            while (true)
            {
                Console.Write("ProductId: ");
                var id = int.Parse(Console.ReadLine());
                Manager.Init("https://localhost:44303", id);


                Console.Write("LicenseKey: ");
                var key = Console.ReadLine();
                var response = await Manager.Verify(key);

                Console.WriteLine($"Success: {response.Success}");
                Console.WriteLine($"Owner: {response.Owner}");
                Console.WriteLine($"Product: {response.Product}");
                Console.WriteLine($"ErrorLog: {response.ErrorLog}");
                Console.WriteLine();
            }
        }
    }
}
