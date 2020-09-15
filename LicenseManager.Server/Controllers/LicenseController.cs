using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicenseManager.Server.Areas.Identity.Data;
using LicenseManager.Server.Data;
using LicenseManager.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;

namespace LicenseManager.Server.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LicenseController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public LicenseController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UrlDatasource([FromBody] DataManagerRequest dm)
        {
            var licensesList = await dbContext.Licenses
                .AsNoTracking()
                .Include(l => l.Product)
                .Include(l => l.User)
                .ToListAsync();

            var gridObjects = licensesList
                .Select(license => new
                {
                    license.Id,
                    license.Key,
                    license.Hwid,
                    license.Active,
                    Product = license.Product.Name,
                    User = license.User?.UserName,
                    license.RedeemDate,
                    license.CreationDate
                });

            return this.PerformGridOerations(gridObjects, dm);
        }

        public async Task<IActionResult> Update([FromBody] CRUDModel<dynamic> crud)
        {
            string key = crud.Value.Key.ToString();

            var license = await dbContext.Licenses.FindAsync(int.Parse(crud.Key.ToString()));
            var existingLicense = await dbContext.Licenses.FirstOrDefaultAsync(l => l.Key == key);

            if (existingLicense != null && existingLicense.Id != license.Id)
            {
                return StatusCode(400, "Update failed, license with this Key already exists!");
            }

            license.Key = crud.Value.Key;
            license.Hwid = crud.Value.Hwid;
            license.Active = crud.Value.Active;

            await dbContext.SaveChangesAsync();

            return Json(crud.Value);
        }

        public async Task<IActionResult> Delete([FromBody] CRUDModel<ApplicationLicense> crud)
        {            
            var license = await dbContext.Licenses.FindAsync(int.Parse(crud.Key.ToString()));

            dbContext.Remove(license);
            await dbContext.SaveChangesAsync();

            return Json(crud.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            ViewBag.Products = await dbContext.Products
                .AsNoTracking()
                .ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddLicenseModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = await dbContext.Products
                    .AsNoTracking()
                    .ToListAsync();

                return View();
            }

            var product = dbContext.Products.Find(model.Id);

            if (product == null)
            {
                ModelState.AddModelError("", "Selected product not longer exists.");

                ViewBag.Products = await dbContext.Products
                    .AsNoTracking()
                    .ToListAsync();

                return View();
            }

            for (int i = 0; i < model.Count; i++)
            {
                var license = new ApplicationLicense
                {
                    Key = Guid.NewGuid().ToString(),
                    Product = product
                };

                await dbContext.AddAsync(license);
            }

            await dbContext.SaveChangesAsync();

            return Redirect("~/License/");
        }
    }
}
