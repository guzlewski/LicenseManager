using System;
using System.Linq;
using System.Threading.Tasks;
using LicenseManager.Server.Data;
using LicenseManager.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LicenseManager.Server.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class CheckLicenseController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public CheckLicenseController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost]
        public async Task<LicenseCheckResponseModel> Post(LicenseCheckRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return new LicenseCheckResponseModel
                {
                    Success = false,
                    ErrorLog = "Invalid request"
                };
            }

            var license = await dbContext.Licenses
                .Where(l => l.Key == request.Key)
                .Include(l => l.User)
                .Include(l => l.Product)
                .FirstOrDefaultAsync();

            if (license == null || license.Product.Id != request.Product)
            {
                return new LicenseCheckResponseModel
                {
                    Success = false,
                    ErrorLog = "Invalid license key"
                };
            }

            if (!license.Active && license.User != null)
            {
                return new LicenseCheckResponseModel
                {
                    Success = false,
                    ErrorLog = "This license has been deactivated."
                };
            }

            if (license.Product.Suspended)
            {
                return new LicenseCheckResponseModel
                {
                    Success = false,
                    ErrorLog = "This product is suspended."
                };
            }

            if (license.User == null)
            {
                return new LicenseCheckResponseModel
                {
                    Success = false,
                    ErrorLog = "License is not activated, add it to your account first."
                };
            }

            if (license.User.LockoutEnd > DateTimeOffset.Now)
            {
                return new LicenseCheckResponseModel
                {
                    Success = false,
                    ErrorLog = $"Account is suspended till {license.User.LockoutEnd}."
                };
            }

            if (string.IsNullOrWhiteSpace(license.Hwid))
            {
                license.Hwid = request.Hwid;
                await dbContext.SaveChangesAsync();
            }

            if (license.Hwid != request.Hwid)
            {
                return new LicenseCheckResponseModel
                {
                    Success = false,
                    ErrorLog = "HWID missmatch, license is binded to another computer."
                };
            }

            return new LicenseCheckResponseModel
            {
                Success = true,
                Owner = license.User.UserName,
                Product = license.Product.Name
            };
        }
    }
}
