using ECommerce.Core.Entities;
using ECommerce.Data.Seeders.Fakers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAsync(roleManager);

            // 1. Seed Admin Manually
            if (await userManager.FindByEmailAsync("admin@myapp.com") == null)
            {
                var admin = new ApplicationUser { FirstName = "Admin", LastName = "User", UserName = "admin", Email = "admin@myapp.com", EmailConfirmed = true, Created = DateTime.UtcNow, Updated = DateTime.UtcNow };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 2. Seed 20 Random Customers using Bogus
            if (await userManager.Users.CountAsync() < 5)
            {
                // Generate 20 fake users in memory
                var fakeUsers = UserFaker.GetUser().Generate(20);

                foreach (var user in fakeUsers)
                {
                    user.Addresses.FirstOrDefault()!.IsDefault = true;
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                    await userManager.AddToRoleAsync(user, "Customer");
                }
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "Customer", "Seller" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}