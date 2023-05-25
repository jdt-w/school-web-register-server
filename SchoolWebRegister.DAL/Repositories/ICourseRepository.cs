using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.DAL.Repositories
{
    public interface ICourseRepository : IDbRepository<Course>
    {
        Task<Student> GetStudentsList(int courseId);
    }
}
