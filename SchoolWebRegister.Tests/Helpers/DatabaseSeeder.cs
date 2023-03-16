using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SchoolWebRegister.DAL;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;
using SchoolWebRegister.Domain.Permissions;

namespace SchoolWebRegister.Tests.Helpers
{
    public static class DatabaseSeeder
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Set<ApplicationUser>().Any()) return;

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IUserService>();

            IEnumerable<UserRole> roles = new[] { UserRole.Guest, UserRole.Administrator };

            var user = UserHelper.GenerateUser();

            foreach (UserRole role in roles)
            {
                string str = role.ToString();
                if (!roleManager.RoleExistsAsync(str).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(str)).GetAwaiter().GetResult();
                }
            }

            var password = new PasswordHasher<ApplicationUser>();
            var hashed = password.HashPassword(user, "secret");
            user.PasswordHash = hashed;

            var userStore = new UserStore<ApplicationUser>(context);
            var result = userStore.CreateAsync(user);

            if (result.Result.Succeeded)
            {
                userService.AddToRoles(user, roles).GetAwaiter().GetResult();

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };
                userService.AddClaims(user, claims).GetAwaiter().GetResult();
                userService.GrantPermission(user, Permissions.Read).GetAwaiter().GetResult();
            }
            await context.SaveChangesAsync();
        }
    }
}
