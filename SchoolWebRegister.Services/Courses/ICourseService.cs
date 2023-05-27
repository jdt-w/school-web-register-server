using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Courses
{
    public interface ICourseService
    {
        Task<BaseResponse> CreateCourse(CourseInfo course);
        Task<BaseResponse> EnrollStudent(string courseId, string studentId);
        Task<BaseResponse> ExpelStudent(string courseId, string studentId);
        Task<BaseResponse> DeleteCourse(string courseId, bool deleteEnrollments = false);
        Task<IEnumerable<CourseInfo>> GetAllCourses();
        Task<CourseInfo> GetCourseById(string id);
        Task<BaseResponse<decimal>> GetStudentProgress(string courseId, string studentId);
        Task<IEnumerable<CourseInfo>> GetCoursesFromStudent(string studentId);
        Task<IEnumerable<ApplicationUser>> GetStudentsList(string courseId);
        Task<BaseResponse> UpdateProgress(string courseId, string studentId, decimal newProgress);
    }
}
