using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Identity;
using Identity.Domain.Entities;

namespace Identity.DAL.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(this IServiceProvider services)
        {
            var context = services.GetRequiredService<AuthDbContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

            IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
            try
            {
                if (!context.Roles.Any())
                {
                    AppRole adminRole = new()
                    {
                        Name = "Admin"
                    };

                    await roleManager.CreateAsync(adminRole);

                    AppRole userRole = new()
                    {
                        Name = "User"
                    };
                    await roleManager.CreateAsync(userRole);

                    await context.SaveChangesAsync();
                }

                if (!context.Users.Any())
                {
                    AppUser admin = new()
                    {
                        UserName = "Admin",
                        Email = "admin@mail.ru",
                    };

                    await userManager.CreateAsync(admin, "12qwasZX");
                    await userManager.AddToRoleAsync(admin, "Admin");

                    AppUser user = new()
                    {
                        UserName = "User",
                        Email = "user@mail.ru",
                    };
                    await userManager.CreateAsync(user, "12qwasZX");
                    await userManager.AddToRoleAsync(user, "User");

                    context.SaveChanges();
                }
                transaction?.Commit();
            }
            catch (Exception)
            {
                transaction?.Rollback();
                throw;
            }
        }
    }
}