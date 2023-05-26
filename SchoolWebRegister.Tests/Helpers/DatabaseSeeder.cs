using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SchoolWebRegister.DAL;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;
using SchoolWebRegister.Domain.Permissions;
using SchoolWebRegister.Domain.Helpers;

namespace SchoolWebRegister.Tests.Helpers
{
    public static class DatabaseSeeder
    {
        public static async void GenerateStudent(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IUserService>();

            IEnumerable<UserRole> roles = new[] { UserRole.Student };

            var user = UserHelper.GenerateUser();
            user.Email = "student@example.com";

            foreach (UserRole role in roles)
            {
                string str = role.ToString();
                if (!roleManager.RoleExistsAsync(str).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(str)).GetAwaiter().GetResult();
                }
            }

            user.PasswordHash = HashPasswordHelper.HashPassword("secret");

            var result = userService.CreateUser(user).GetAwaiter().GetResult();
            if (result.Status == Domain.StatusCode.Success.ToString())
            {
                userService.AddToRoles(user, roles).GetAwaiter().GetResult();

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Email, user.Email)
                };
                userService.AddClaims(user, claims).GetAwaiter().GetResult();
                userService.GrantPermission(user, Permissions.Admin).GetAwaiter().GetResult();
            }
            await context.SaveChangesAsync();
        }
        public static async void GenerateTeacher(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IUserService>();

            IEnumerable<UserRole> roles = new[] { UserRole.Teacher };

            var user = UserHelper.GenerateUser();
            user.Email = "teacher@example.com";

            foreach (UserRole role in roles)
            {
                string str = role.ToString();
                if (!roleManager.RoleExistsAsync(str).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(str)).GetAwaiter().GetResult();
                }
            }

            user.PasswordHash = HashPasswordHelper.HashPassword("secret");

            var result = userService.CreateUser(user).GetAwaiter().GetResult();
            if (result.Status == Domain.StatusCode.Success.ToString())
            {
                userService.AddToRoles(user, roles).GetAwaiter().GetResult();

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Email, user.Email)
                };
                userService.AddClaims(user, claims).GetAwaiter().GetResult();
                userService.GrantPermission(user, Permissions.Admin).GetAwaiter().GetResult();
            }
            await context.SaveChangesAsync();
        }

        public static async void Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Set<ApplicationUser>().Any()) return;

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IUserService>();

            IEnumerable<UserRole> roles = new[] { UserRole.Student };

            var user = UserHelper.GenerateUser();

            foreach (UserRole role in roles)
            {
                string str = role.ToString();
                if (!roleManager.RoleExistsAsync(str).Result)
                {
                    roleManager.CreateAsync(new IdentityRole(str)).GetAwaiter().GetResult();
                }
            }
                        
            user.PasswordHash = HashPasswordHelper.HashPassword("secret");

            var result = userService.CreateUser(user).GetAwaiter().GetResult();
            if (result.Status == Domain.StatusCode.Success.ToString())
            {
                userService.AddToRoles(user, roles).GetAwaiter().GetResult();

                List<Claim> claims = new()
                {
                    new Claim(ClaimTypes.Email, user.Email)
                };
                userService.AddClaims(user, claims).GetAwaiter().GetResult();
                userService.GrantPermission(user, Permissions.Admin).GetAwaiter().GetResult();
            }
            await context.SaveChangesAsync();
        }
    }
}
