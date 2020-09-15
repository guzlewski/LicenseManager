using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LicenseManager.Server.Areas.Identity.Data;
using LicenseManager.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using Utf8Json.Formatters;

namespace LicenseManager.Server.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ProductController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UrlDatasource([FromBody] DataManagerRequest dm)
        {
            var productsList = await dbContext.Products
                .AsNoTracking()
                .Include(p => p.Licenses)
                .ToListAsync();

            var gridObjects = productsList
                .Select(product => new
                {
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Suspended,
                    Keys = product.Licenses.Count,
                    product.CreationDate
                });

            return this.PerformGridOerations(gridObjects, dm);
        }

        public async Task<IActionResult> Insert([FromBody] CRUDModel<ApplicationProduct> crud)
        {
            var product = new ApplicationProduct
            {
                Name = crud.Value.Name,
                Description = crud.Value.Description,
                Suspended = crud.Value.Suspended
            };

            if (await dbContext.Products.FirstOrDefaultAsync(p => p.Name == product.Name) != null)
            {
                return StatusCode(400, "Insert failed, product with this name already exists!");
            }

            await dbContext.AddAsync(product);
            await dbContext.SaveChangesAsync();

            return Json(crud.Value);
        }

        public async Task<IActionResult> Update([FromBody] CRUDModel<ApplicationProduct> crud)
        {
            var product = await dbContext.Products.FindAsync(crud.Value.Id);
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.Name == crud.Value.Name);

            if (existingProduct != null && existingProduct.Id != product.Id)
            {
                return StatusCode(400, "Update failed, product with this name already exists!");
            }

            product.Name = crud.Value.Name;
            product.Description = crud.Value.Description;
            product.Suspended = crud.Value.Suspended;

            await dbContext.SaveChangesAsync();

            return Json(crud.Value);
        }

        public async Task<IActionResult> Delete([FromBody] CRUDModel<ApplicationProduct> crud)
        {
            var product = await dbContext.Products.FindAsync(int.Parse(crud.Key.ToString()));

            dbContext.Remove(product);
            await dbContext.SaveChangesAsync();

            return Json(crud.Value);
        }
    }
}
