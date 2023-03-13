using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public sealed class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db) { }

        public Task<ApplicationUser?> GetUserByLoginAsync(string login)
        {
            throw new NotImplementedException();
        }
    }
}
