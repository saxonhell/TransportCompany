using Microsoft.AspNetCore.Identity;
using TransportCompany.Models;

namespace TransportCompany.Data.Initializer
{
    public static class DbUserInitializer
    {
        public static async Task Initialize(HttpContext context)
        {
            using (var scope = context.RequestServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string adminEmail = "admin@gmail.com";
                string adminPassword = "Admin123_q";

                string userEmail = "user@gmail.com";
                string userPassword = "User123_q";

                // Создание роли "admin"
                if (await roleManager.FindByNameAsync("admin") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("admin"));
                }

                // Создание роли "user"
                if (await roleManager.FindByNameAsync("user") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("user"));
                }

                // Создание администратора
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    ApplicationUser admin = new ApplicationUser
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "admin");
                    }
                    else
                    {
                        throw new Exception("Не удалось создать администратора: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }

                // Создание обычного пользователя
                if (await userManager.FindByEmailAsync(userEmail) == null)
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        Email = userEmail,
                        UserName = userEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, userPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "user");
                    }
                    else
                    {
                        throw new Exception("Не удалось создать пользователя: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }
    }
}
