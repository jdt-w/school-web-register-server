using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Services.Courses;
using SchoolWebRegister.Services.Logging;

namespace SchoolWebRegister.Web.Areas.Users.Controllers
{
    [Authorize(Policy = "AllUsers")]
    [ApiController]
    [Route("[controller]")]
    public sealed class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService, ILoggingService logger)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [Route("/course/getAll")]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await _courseService.GetAllCourses();
            return Ok(new BaseResponse(
                code: Domain.StatusCode.Success,
                data: result
            ));
        }

        [HttpGet]
        [Route("/course/id")]
        public async Task<IActionResult> GetCourseById(string id)
        {
            var result = await _courseService.GetCourseById(id);
            return Ok(new BaseResponse(
               code: Domain.StatusCode.Success,
               data: result
            ));
        }

        [HttpGet]
        [Route("/course/getStudents")]
        public async Task<IActionResult> GetStudents(string courseId)
        {
            var result = await _courseService.GetStudentsList(courseId);
            return Ok(new BaseResponse(
               code: Domain.StatusCode.Success,
               data: result
            ));
        }

        [HttpGet]
        [Route("/course/getCoursesFromStudent")]
        public async Task<IActionResult> GetCoursesFromStudent(string studentId)
        {
            var result = await _courseService.GetCoursesFromStudent(studentId);
            return Ok(new BaseResponse(
               code: Domain.StatusCode.Success,
               data: result
            ));
        }

        [HttpGet]
        [Route("/course/progress")]
        public async Task<IActionResult> GetStudentProgress(string courseId, string studentId)
        {
            var result = await _courseService.GetStudentProgress(courseId, studentId);
            return Ok(result);
        }

        [HttpPost]
        [Route("/course/updateProgress")]
        public async Task<IActionResult> UpdateStudentProgress(string courseId, string studentId, decimal newProgress)
        {
            var result = await _courseService.UpdateProgress(courseId, studentId, newProgress);
            if (result.Status == Domain.StatusCode.Success.ToString())
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost]
        [Route("/course/create")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseInfo course)
        {
            var result = await _courseService.CreateCourse(course);
            if (result.Status == Domain.StatusCode.Success.ToString())
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost]
        [Route("/course/delete")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            var result = await _courseService.DeleteCourse(courseId, deleteEnrollments: true);
            if (result.Status == Domain.StatusCode.Success.ToString())
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost]
        [Route("/course/update")]
        public async Task<IActionResult> UpdateCourse([FromBody] CourseInfo course)
        {
            var result = await _courseService.DeleteCourse(course.Id);
            if (result.Status != Domain.StatusCode.Success.ToString())
                return BadRequest(result);

            result = await _courseService.CreateCourse(course);
            if (result.Status == Domain.StatusCode.Success.ToString())
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost]
        [Route("/course/enroll")]
        public async Task<IActionResult> EnrollStudent(string courseId, string studentId)
        {
            if (string.IsNullOrWhiteSpace(courseId) || string.IsNullOrWhiteSpace(studentId))
                return BadRequest(new BaseResponse(
                    code: Domain.StatusCode.Error,
                    error: new ErrorType
                    {
                        Type = new string[] { "MISSING_DATA" }
                    }
                ));

            var result = await _courseService.EnrollStudent(courseId, studentId);            
            return Ok(result);
        }

        [HttpPost]
        [Route("/course/expel")]
        public async Task<IActionResult> ExpelStudent(string courseId, string studentId)
        {
            if (string.IsNullOrWhiteSpace(courseId) || string.IsNullOrWhiteSpace(studentId))
                return BadRequest(new BaseResponse(
                    code: Domain.StatusCode.Error,
                    error: new ErrorType
                    {
                        Type = new string[] { "MISSING_DATA" }
                    }
                ));

            var result = await _courseService.ExpelStudent(courseId, studentId);
            return Ok(new BaseResponse(
               code: Domain.StatusCode.Success,
               data: result
            ));
        }
    }
}
