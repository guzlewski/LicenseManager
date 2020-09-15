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
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UrlDatasource([FromBody] DataManagerRequest dm)
        {
            var adminsList = (await dbContext.Users
                .AsNoTracking()
                .Include(u => u.Licenses)
                .ToListAsync())
                .Where(u => IsAdmin(u).Result);

            var gridObjects = adminsList
                .Select(user => new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.LockoutEnd,
                    Keys = user.Licenses.Count,
                    user.RegisterDate
                });

            return this.PerformGridOerations(gridObjects, dm);
        }

        private async Task<bool> IsAdmin(ApplicationUser user)
        {
            return await userManager.IsInRoleAsync(user, "Administrator");
        }

        public async Task<IActionResult> Delete([FromBody] CRUDModel<ApplicationUser> crud)
        {
            var user = await userManager.FindByIdAsync(crud.Key.ToString());

            if (await userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                return StatusCode(400, "SuperAdmin cannot be deleted.");
            }

            await userManager.RemoveFromRoleAsync(user, "Administrator");

            return Json(crud.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Users = (await dbContext.Users
                .AsNoTracking()
                .ToListAsync())
                .Where(u => !IsAdmin(u).Result)
                .Select(u => new { u.Id , u.UserName});

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddAdminModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = (await dbContext.Users
                    .AsNoTracking()
                    .ToListAsync())
                    .Where(u => !IsAdmin(u).Result)
                    .Select(u => new { u.Id, u.UserName });

                return View();
            }

            var user = dbContext.Users.FirstOrDefault(u => u.Id == model.Id);
            await userManager.AddToRoleAsync(user, "Administrator");

            return Redirect("~/Admin/");
        }
    }
}
