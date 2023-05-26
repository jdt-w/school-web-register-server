using Azure;
using Microsoft.Extensions.Configuration;
using SchoolWebRegister.DAL.Repositories;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using System.Data.SqlClient;
using static System.Collections.Specialized.BitVector32;

namespace SchoolWebRegister.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;
        private readonly IConfiguration _config;
        public CourseService(ICourseRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }
        public async Task CreateSections(string courseId, CourseSection[] sections)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            string insertQuery = "insert into CourseSection1(Id,Checked,Title,CourseId) values (@id,@checked,@title,@courseId)";

            foreach (var section in sections)
            {
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@id", section.Id);
                cmd.Parameters.AddWithValue("@checked", section.Checked);
                cmd.Parameters.AddWithValue("@title", section.Title);
                cmd.Parameters.AddWithValue("@courseId", courseId);
                await cmd.ExecuteNonQueryAsync();

                await CreateLessons(section.Id, section.Lessons);
            }
        }
        public async Task CreateLessons(string sectionId, CourseLesson[] lessons)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            string insertQuery = "insert into CourseLesson(Id,Checked,Title,Content,SectionId) values (@id,@checked,@title,@content,@sectionId)";

            foreach (var lesson in lessons)
            {
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@id", lesson.Id);
                cmd.Parameters.AddWithValue("@checked", lesson.Checked);
                cmd.Parameters.AddWithValue("@title", lesson.Title);
                cmd.Parameters.AddWithValue("@content", lesson.Content);
                cmd.Parameters.AddWithValue("@sectionId", sectionId);
                await cmd.ExecuteNonQueryAsync();

                if (lesson.Quiz != null)
                {
                    await CreateQuiz(lesson.Id, lesson.Quiz);
                }
            }
        }
        public async Task CreateQuiz(string lessonId, CourseQuiz quiz)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            string insertQuery = "insert into CourseQuiz(LessonId,QuizId) values (@lessonId,@quizId)";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@lessonId", lessonId);
            cmd.Parameters.AddWithValue("@quizId", quiz.Id);
            await cmd.ExecuteNonQueryAsync();

            await CreateQuestions(quiz.Id, quiz.Questions);
        }
        public async Task CreateQuestions(string quizId, CourseQuestion[] questions)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            string insertQuery = "insert into CourseQuizQuestion(Id,Checked,Opened,Question,Type,Variant,QuizId) values (@id,@checked,@opened,@question,@type,@variant,@quizId)";

            foreach (var question in questions)
            {
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@id", question.Id);
                cmd.Parameters.AddWithValue("@checked", question.Checked);
                cmd.Parameters.AddWithValue("@opened", question.Opened);
                cmd.Parameters.AddWithValue("@question", question.Info.Question);
                cmd.Parameters.AddWithValue("@type", question.Info.Type);
                cmd.Parameters.AddWithValue("@variant", question.Info.Variant);
                cmd.Parameters.AddWithValue("@quizId", quizId);
                await cmd.ExecuteNonQueryAsync();

                await CreateQuestionAnswers(question.Id, question.Answers);
            }
        }
        public async Task CreateQuestionAnswers(string questionId, QuizQuestionAnswer[] questionAnswers)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            string insertQuery = "insert into CourseQuestionAnswer(Id,Answer,IsCorrect,QuestionId) values (@id,@answer,@isCorrect,@questionId)";

            foreach (var answer in questionAnswers)
            {
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@id", answer.Id);
                cmd.Parameters.AddWithValue("@answer", answer.Info.Answer);
                cmd.Parameters.AddWithValue("@isCorrect", answer.Info.IsCorrect);
                cmd.Parameters.AddWithValue("@questionId", questionId);
                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task CreateCourseBase(CourseInfo course)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            string insertQuery = "insert into Course1(Id,Title,AuthorId,CreateTime) values (@id,@title,@authorId,@createTime)";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@id", course.Id);
            cmd.Parameters.AddWithValue("@title", course.Title);
            cmd.Parameters.AddWithValue("@authorId", course.AuthorID);
            cmd.Parameters.AddWithValue("@createTime", course.CreateTime);
            await cmd.ExecuteNonQueryAsync();
        }
        public async Task<BaseResponse> CreateCourse(CourseInfo course)
        {
            try
            {
                await CreateCourseBase(course);

                await CreateSections(course.Id, course.Sections);

                return new BaseResponse(
                    code: StatusCode.Success,
                    data: course.Id
                );
            }
            catch (Exception ex)
            {
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: new ErrorType
                    {
                        Message = ex.Message,
                        Type = new string[] { "INVALID_DATA" }
                    }
                );
            }
        }
        //public async Task<BaseResponse> CreateCourse(CourseInfo course)
        //{
        //    if (course == null)
        //    {
        //        return new BaseResponse(
        //            code: StatusCode.Error,
        //            error: "MISSING_DATA"
        //        );
        //    }

        //    try
        //    {
        //        var result = await _repository.AddAsync(course);
        //        return new BaseResponse(
        //            code: StatusCode.Success,
        //            data: result.Id
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BaseResponse(
        //            code: StatusCode.Error,
        //            error: new ErrorType
        //            {
        //                Message = ex.Message,
        //                Type = new string[] { "INVALID_DATA" }
        //            }
        //        );
        //    }
        //}
        public async Task DeleteCourse(int courseId)
        {
            //Course? course = await _repository.GetByIdAsync(courseId.ToString());
            //if (course != null)
            //    await _repository.DeleteAsync(course);
        }
        public async Task<BaseResponse> EnrollStudent(int courseId, string studentId)
        {
            throw new NotImplementedException();
        }
        public async Task<BaseResponse> ExpelStudent(int courseId, string studentId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetStudentsCount(int courseId)
        {
            return 0;
            //return _repository.Select().Where(x => x.Id == courseId).Count();
        }

        public IQueryable<ApplicationUser> GetStudentsList(int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
