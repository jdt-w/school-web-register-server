using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Tests.Helpers
{
    internal static class UserHelper
    {
        public static ApplicationUser GenerateUser()
        {
            return new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = "xxxx@example.com",
                NormalizedEmail = "XXXX@EXAMPLE.COM",
                PhoneNumber = "+111111111111",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
        }
        public static IEnumerable<ApplicationUser> GenerateMany()
        {
            yield return GenerateUser();
        }
    }
}
