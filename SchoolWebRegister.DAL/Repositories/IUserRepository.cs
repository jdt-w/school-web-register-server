using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public interface IUserRepository : IDbRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserByLoginAsync(string login);
        Task<IEnumerable<ApplicationUser>> GetUserListAsync();
    }
}
