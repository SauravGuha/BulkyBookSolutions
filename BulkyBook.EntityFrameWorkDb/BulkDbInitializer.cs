

using BulkyBook.Common.Interfaces;
using BulkyBook.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.EntityFrameWorkDb
{
    public class BulkDbInitializer : IDbInitializer
    {
        private readonly BulkyBookDbContext _bulkyBookDbContext;
        private UserManager<Customer> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public BulkDbInitializer(BulkyBookDbContext bulkyBookDbContext,
            UserManager<Customer> userManger, RoleManager<IdentityRole> roleManager)
        {
            this._bulkyBookDbContext = bulkyBookDbContext;
            this._userManager = userManger;
            this._roleManager = roleManager;
        }

        public async Task Initialise()
        {
            if (this._bulkyBookDbContext.Database.GetPendingMigrations().Count() > 0)
            {
                await this._bulkyBookDbContext.Database.MigrateAsync();
            }
            var superAdminRoleExists = this._roleManager.Roles.Any(e => e.Name == Common.Constants.Roles.SuperAdmin.ToString());
            if (!superAdminRoleExists)
            {
                await SeedRoles();
                await SeedUsers();
            }
        }

        private async Task SeedRoles()
        {
            await this._roleManager.CreateAsync(new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Common.Constants.Roles.SuperAdmin.ToString(),
                NormalizedName = Common.Constants.Roles.SuperAdmin.ToString(),
            });
            await this._roleManager.CreateAsync(new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Common.Constants.Roles.Admin.ToString(),
                NormalizedName = Common.Constants.Roles.Admin.ToString(),
            });
            await this._roleManager.CreateAsync(new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Common.Constants.Roles.Customer.ToString(),
                NormalizedName = Common.Constants.Roles.Customer.ToString(),
            });
            await this._roleManager.CreateAsync(new IdentityRole()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Common.Constants.Roles.CompanyUser.ToString(),
                NormalizedName = Common.Constants.Roles.CompanyUser.ToString(),
            });
        }

        private async Task SeedUsers()
        {
            await this._userManager.CreateAsync(new Customer()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Super Admin",
                Age = 999,
                EmailConfirmed = true,
                UserName = "superadmin@bulkybook.com",
                NormalizedUserName = "superadmin@bulkybook.com",
                Email = "superadmin@bulkybook.com",
                NormalizedEmail = "superadmin@bulkybook.com",
                PhoneNumber = "9681172609"
            }, "abcd.1234");

            var superAdminUser = this._userManager.Users.FirstOrDefault(e => e.UserName == "superadmin@bulkybook.com");
            if (superAdminUser != null)
            {
                await this._userManager.AddToRoleAsync(superAdminUser, Common.Constants.Roles.SuperAdmin.ToString());
                await this._userManager.AddToRoleAsync(superAdminUser, Common.Constants.Roles.Admin.ToString());
                await this._userManager.AddToRoleAsync(superAdminUser, Common.Constants.Roles.CompanyUser.ToString());
                await this._userManager.AddToRoleAsync(superAdminUser, Common.Constants.Roles.Customer.ToString());
            }
        }

    }
}
