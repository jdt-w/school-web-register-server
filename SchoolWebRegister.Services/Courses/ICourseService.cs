using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Courses
{
    public interface ICourseService
    {
        Task<BaseResponse> CreateCourse(CourseInfo course);
        Task<BaseResponse> EnrollStudent(string courseId, string studentId);
        Task<BaseResponse> ExpelStudent(string courseId, string studentId);
        Task DeleteCourse(int courseId);
        Task<int> GetStudentsCount(int courseId);
        Task<IEnumerable<CourseInfo>> GetAllCourses();
        Task<CourseInfo> GetCourseById(string id);
        IQueryable<ApplicationUser> GetStudentsList(int courseId);
    }
}
