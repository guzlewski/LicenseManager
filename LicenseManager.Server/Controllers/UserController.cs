using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicenseManager.Server.Areas.Identity.Data;
using LicenseManager.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace LicenseManager.Server.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;

        public UserController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
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
            var usersList = await dbContext.Users
                .AsNoTracking()
                .Include(u => u.Licenses)
                .ToListAsync();

            var gridObjects = usersList
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

        public async Task<IActionResult> Update([FromBody] CRUDModel<ApplicationUser> crud)
        {
            var user = await userManager.FindByIdAsync(crud.Value.Id);
            var existingUsername = await userManager.FindByNameAsync(crud.Value.UserName);
            var existingEmail = await userManager.FindByNameAsync(crud.Value.Email);

            string error = string.Empty;

            if (existingUsername != null && existingUsername.Id != user.Id)
            {
                error += "User with this Username already exists!";
            }

            if (existingEmail != null && existingEmail.Id != user.Id)
            {
                if (error != string.Empty)
                {
                    error += " ";
                }

                error += "User with this Email already exists!";
            }

            if (error != string.Empty)
            {
                return StatusCode(400, error);
            }

            user.UserName = crud.Value.UserName;
            user.Email = crud.Value.Email;
            user.LockoutEnd = crud.Value.LockoutEnd;

            await userManager.UpdateAsync(user);

            return Json(crud.Value);
        }

        public async Task<IActionResult> Delete([FromBody] CRUDModel<ApplicationUser> crud)
        {
            var user = await userManager.FindByIdAsync(crud.Key.ToString());

            if (await userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                return StatusCode(400, "SuperAdmin cannot be deleted.");
            }

            await userManager.DeleteAsync(user);

            return Json(crud.Value);
        }
    }
}
