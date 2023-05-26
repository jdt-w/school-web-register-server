using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Courses;

namespace SchoolWebRegister.DAL.Repositories
{
    public interface ICourseRepository : IDbRepository<CourseInfo>
    {
        Task<Student> GetStudentsList(int courseId);
    }
}
