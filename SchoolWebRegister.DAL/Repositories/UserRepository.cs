using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public sealed class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db) { }

        public async Task<ApplicationUser?> GetUserByLoginAsync(string login)
        {
            var users = await SelectAsync(user => user.UserName.Equals(login));
            return users.First();
        }
        public async Task<IEnumerable<ApplicationUser>> GetUserListAsync() => await SelectAsync();
    }
}
