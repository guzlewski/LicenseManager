using System.Threading.Tasks;
using LicenseManager.Server.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace LicenseManager.Server
{
    public class DataInitializer
    {
        public void SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            SeedRoles(roleManager).Wait();
            SeedUsers(userManager).Wait();
        }

        public async Task SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Administrator"))
            {
                ApplicationRole role = new ApplicationRole
                {
                    Name = "Administrator"
                };

                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                ApplicationRole role = new ApplicationRole
                {
                    Name = "SuperAdmin"
                };

                await roleManager.CreateAsync(role);
            }
        }

        public async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByNameAsync("admin");

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@localhost",
                };

                var password = "12345aA!";
                await userManager.CreateAsync(user, password);
            }

            if (!await userManager.IsInRoleAsync(user, "Administrator"))
            {
                await userManager.AddToRoleAsync(user, "Administrator");
            }

            if (!await userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                await userManager.AddToRoleAsync(user, "SuperAdmin");
            }
        }
    }
}
