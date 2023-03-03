using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public sealed class CourseRepository : BaseRepository<Course>
    {
        public CourseRepository(ApplicationDbContext db) : base(db) { }

        public async Task<float> GetUserProgressAsync(ApplicationUser user, string courseId)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<ApplicationUser>> GetUserListAsync()
        {
            throw new NotImplementedException();
        }
    }
}
