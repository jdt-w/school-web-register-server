using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;

namespace SchoolWebRegister.Services.Courses
{
    public interface ICourseService
    {
        Task<BaseResponse<bool>> CreateCourse(Course course);
        Task<BaseResponse<bool>> EnrollStudent(int courseId, string studentId);
        Task<BaseResponse<bool>> ExpelStudent(int courseId, string studentId);
        Task DeleteCourse(int courseId);
        Task<int> GetStudentsCount(int courseId);
        Task<IEnumerable<ApplicationUser>> GetStudentsList(int courseId);
    }
}
