using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Courses
{
    public interface ICourseService
    {
        Task<BaseResponse> CreateCourse(CourseInfo course);
        Task<BaseResponse> EnrollStudent(int courseId, string studentId);
        Task<BaseResponse> ExpelStudent(int courseId, string studentId);
        Task DeleteCourse(int courseId);
        Task<int> GetStudentsCount(int courseId);
        IQueryable<ApplicationUser> GetStudentsList(int courseId);
    }
}
