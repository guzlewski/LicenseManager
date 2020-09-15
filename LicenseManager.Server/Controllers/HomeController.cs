using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LicenseManager.Server.Areas.Identity.Data;
using LicenseManager.Server.Data;
using LicenseManager.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace LicenseManager.Server.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> UrlDatasource([FromBody] DataManagerRequest dm)
        {
            var loggedUser = await userManager.GetUserAsync(User);
            var userLicensesList = await dbContext.Licenses
                .AsNoTracking()
                .Include(l => l.User)
                .Where(u => u.User == loggedUser)
                .Include(l => l.Product)
                .ToListAsync();

            var gridObjects = userLicensesList
                .Select(user => new
                {
                    user.Id,
                    user.Key,
                    user.Hwid,
                    user.Active,
                    Product = user.Product.Name,
                    user.RedeemDate
                });

            return this.PerformGridOerations(gridObjects, dm);
        }

        public async Task<IActionResult> Insert([FromBody] CRUDModel<ApplicationLicense> crud)
        {
            var license = await dbContext.Licenses.FirstOrDefaultAsync(l => l.Key == crud.Value.Key);

            if (license == null || license.User != null)
            {
                return StatusCode(400, "Invalid license key.");
            }

            license.User = await userManager.GetUserAsync(User);
            license.Active = true;
            license.RedeemDate = DateTime.Now;

            await dbContext.SaveChangesAsync();

            return Json(crud.Value);
        }

        public async Task<IActionResult> Update([FromBody] CRUDModel<dynamic> crud)
        {
            string key = crud.Value.Key.ToString();
            var loggedUser = await userManager.GetUserAsync(User);
            var license = await dbContext.Licenses.FirstOrDefaultAsync(l => l.Key == key);

            if (license == null)
            {
                return StatusCode(400, "License with this id not exists.");
            }

            if (license.User != loggedUser)
            {
                return StatusCode(401, "License is owned by other user.");
            }

            license.Hwid = crud.Value.Hwid;

            await dbContext.SaveChangesAsync();

            return Json(crud.Value);
        }
    }
}
