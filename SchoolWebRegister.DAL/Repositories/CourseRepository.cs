using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Courses;

namespace SchoolWebRegister.DAL.Repositories
{
    public sealed class CourseRepository : BaseRepository<CourseInfo>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext db) : base(db) 
        {

        }

        public Task<Student> GetStudentsList(int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
