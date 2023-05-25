using SchoolWebRegister.DAL.Repositories;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly IUserService _userService;
        public CourseService(ICourseRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }
        public async Task<BaseResponse<bool>> CreateCourse(Course course)
        {
            throw new NotImplementedException();
            //await _repository.AddAsync(course);
        }
        public async Task DeleteCourse(int courseId)
        {
            Course? course = await _repository.GetByIdAsync(courseId.ToString());
            if (course != null)
                await _repository.DeleteAsync(course);
        }
        public async Task<BaseResponse<bool>> EnrollStudent(int courseId, string studentId)
        {
            throw new NotImplementedException();
        }
        public async Task<BaseResponse<bool>> ExpelStudent(int courseId, string studentId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetStudentsCount(int courseId)
        {
            return _repository.Select().Where(x => x.Id == courseId).Count();
        }

        public IQueryable<ApplicationUser> GetStudentsList(int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
