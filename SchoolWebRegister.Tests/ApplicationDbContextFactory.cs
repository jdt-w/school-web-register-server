using Microsoft.EntityFrameworkCore;
using SchoolWebRegister.DAL;
using SchoolWebRegister.Tests.Helpers;

namespace SchoolWebRegister.Tests
{
    internal class ApplicationDbContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            
            context.AddRange(UserHelper.GenerateMany());

            context.SaveChanges();

            return context;
        }
        public static void Destroy(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
