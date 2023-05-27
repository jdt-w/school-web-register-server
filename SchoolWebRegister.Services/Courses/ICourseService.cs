using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Courses
{
    public interface ICourseService
    {
        Task<BaseResponse> CreateCourse(CourseInfo course);
        Task<BaseResponse> EnrollStudent(string courseId, string studentId);
        Task<BaseResponse> ExpelStudent(string courseId, string studentId);
        Task<BaseResponse> DeleteCourse(string courseId);
        Task<IEnumerable<CourseInfo>> GetAllCourses();
        Task<CourseInfo> GetCourseById(string id);
        Task<IEnumerable<CourseInfo>> GetCoursesFromStudent(string studentId);
        Task<IEnumerable<ApplicationUser>> GetStudentsList(string courseId);
    }
}
