using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public static class SeedAdmin
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

         
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            
            var adminEmail = "adminMahsaa@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new User { UserName = "admin", Email = adminEmail };
                await userManager.CreateAsync(user, "167Sh%ty");
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

    }
}
