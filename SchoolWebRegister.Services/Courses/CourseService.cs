using Microsoft.Extensions.Configuration;
using SchoolWebRegister.DAL;
using SchoolWebRegister.DAL.Repositories;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using System.Collections.Generic;
using System.Data;
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
        private async Task CreateSections(string courseId, CourseSection[] sections)
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
            conn.Close();
        }
        private async Task CreateLessons(string sectionId, CourseLesson[] lessons)
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
            conn.Close();
        }
        private async Task CreateQuiz(string lessonId, CourseQuiz quiz)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();
            string insertQuery = "insert into CourseQuiz(LessonId,QuizId) values (@lessonId,@quizId)";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@lessonId", lessonId);
            cmd.Parameters.AddWithValue("@quizId", quiz.Id);
            await cmd.ExecuteNonQueryAsync();

            await CreateQuestions(quiz.Id, quiz.Questions);
            conn.Close();
        }
        private async Task CreateQuestions(string quizId, CourseQuestion[] questions)
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
            conn.Close();
        }
        private async Task CreateQuestionAnswers(string questionId, QuizQuestionAnswer[] questionAnswers)
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
            conn.Close();
        }
        private async Task CreateCourseBase(CourseInfo course)
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
            conn.Close();
        }
        private async Task<IEnumerable<CourseInfo>> GetAllCoursesBase()
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.Course1";
            cmd.Parameters.Add("id", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("title", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("authorId", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("createTime", SqlDbType.DateTime);
            cmd.Parameters["id"].Direction = ParameterDirection.Output;
            cmd.Parameters["title"].Direction = ParameterDirection.Output;
            cmd.Parameters["authorId"].Direction = ParameterDirection.Output;
            cmd.Parameters["createTime"].Direction = ParameterDirection.Output;
            conn.Open();

            var list = new List<CourseInfo>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new CourseInfo
                    {
                        Id = reader["id"].ToString(),
                        Title = reader["title"].ToString(),
                        AuthorID = reader["authorId"].ToString(),
                        CreateTime = (DateTime)reader["createTime"],
                    });
                }
            }
            conn.Close();
            return list;
        }
        private async Task<IEnumerable<CourseSection>> GetCourseSections(CourseInfo course)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.CourseSection1 WHERE CourseId = @courseId";
            cmd.Parameters.Add("id", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("checked", SqlDbType.Bit);
            cmd.Parameters.Add("title", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("@courseId", SqlDbType.NVarChar, 450);
            cmd.Parameters["@courseId"].Direction = ParameterDirection.Input;
            cmd.Parameters["@courseId"].Value = course.Id;
            cmd.Parameters["id"].Direction = ParameterDirection.Output;
            cmd.Parameters["checked"].Direction = ParameterDirection.Output;
            cmd.Parameters["title"].Direction = ParameterDirection.Output;
            conn.Open();

            var list = new List<CourseSection>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new CourseSection
                    {
                        Id = reader["id"].ToString(),
                        Checked = (bool)reader["checked"],
                        Title = reader["title"].ToString()
                    });
                }
            }
            conn.Close();
            return list;
        }
        private async Task<IEnumerable<CourseLesson>> GetSectionLessons(CourseSection section)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.CourseLesson WHERE SectionId = @sectionId";
            cmd.Parameters.Add("id", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("checked", SqlDbType.Bit);
            cmd.Parameters.Add("title", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("content", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("@sectionId", SqlDbType.NVarChar, 450);
            cmd.Parameters["@sectionId"].Direction = ParameterDirection.Input;
            cmd.Parameters["@sectionId"].Value = section.Id;
            cmd.Parameters["id"].Direction = ParameterDirection.Output;
            cmd.Parameters["checked"].Direction = ParameterDirection.Output;
            cmd.Parameters["title"].Direction = ParameterDirection.Output;
            cmd.Parameters["content"].Direction = ParameterDirection.Output;
            conn.Open();

            var list = new List<CourseLesson>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new CourseLesson
                    {
                        Id = reader["id"].ToString(),
                        Checked = (bool)reader["checked"],
                        Title = reader["title"].ToString(),
                        Content = reader["content"].ToString(),
                    });
                }
            }
            conn.Close();
            return list;
        }
        private async Task<CourseQuiz> GetLessonQuiz(CourseLesson lesson)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.CourseQuiz WHERE LessonId = @lessonId";
            cmd.Parameters.Add("@lessonId", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("quizId", SqlDbType.NVarChar, 450);
            cmd.Parameters["@lessonId"].Direction = ParameterDirection.Input;
            cmd.Parameters["@lessonId"].Value = lesson.Id;
            cmd.Parameters["quizId"].Direction = ParameterDirection.Output;
            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string id = reader["quizId"]?.ToString();
                    return new CourseQuiz
                    {
                        Id = id,
                    };
                }
            }
            conn.Close();

            return null;
        }
        private async Task<IEnumerable<CourseQuestion>> GetQuizQuestions(CourseQuiz quiz)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.CourseQuizQuestion WHERE QuizId = @quizId";
            cmd.Parameters.Add("id", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("checked", SqlDbType.Bit);
            cmd.Parameters.Add("opened", SqlDbType.Bit);
            cmd.Parameters.Add("question", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("type", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("variant", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("@quizId", SqlDbType.NVarChar, 450);
            cmd.Parameters["@quizId"].Direction = ParameterDirection.Input;
            cmd.Parameters["@quizId"].Value = quiz.Id;
            cmd.Parameters["id"].Direction = ParameterDirection.Output;
            cmd.Parameters["checked"].Direction = ParameterDirection.Output;
            cmd.Parameters["opened"].Direction = ParameterDirection.Output;
            cmd.Parameters["question"].Direction = ParameterDirection.Output;
            cmd.Parameters["type"].Direction = ParameterDirection.Output;
            cmd.Parameters["variant"].Direction = ParameterDirection.Output;
            conn.Open();

            var list = new List<CourseQuestion>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new CourseQuestion
                    {
                        Id = reader["id"].ToString(),
                        Checked = (bool)reader["checked"],
                        Opened = (bool)reader["opened"],
                        Info = new CourseQuestionInfo
                        {
                            Question = reader["question"].ToString(),
                            Type = reader["type"].ToString(),
                            Variant = reader["variant"].ToString(),
                        },
                    });
                }
            }
            conn.Close();
            return list;
        }
        private async Task<IEnumerable<QuizQuestionAnswer>> GetQuestionAnswers(CourseQuestion question)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.CourseQuestionAnswer WHERE QuestionId = @questionId";
            cmd.Parameters.Add("id", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("answer", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("isCorrect", SqlDbType.Bit);
            cmd.Parameters.Add("@questionId", SqlDbType.NVarChar, 450);
            cmd.Parameters["@questionId"].Direction = ParameterDirection.Input;
            cmd.Parameters["@questionId"].Value = question.Id;
            cmd.Parameters["id"].Direction = ParameterDirection.Output;
            cmd.Parameters["answer"].Direction = ParameterDirection.Output;
            cmd.Parameters["isCorrect"].Direction = ParameterDirection.Output;
            conn.Open();

            var list = new List<QuizQuestionAnswer>();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new QuizQuestionAnswer
                    {
                        Id = reader["id"].ToString(),
                        Info = new AnswerInfo
                        {
                            Answer = reader["answer"].ToString(),
                            IsCorrect = (bool)reader["isCorrect"],
                        }
                    });
                }
            }
            conn.Close();
            return list;
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
        public async Task<IEnumerable<CourseInfo>> GetAllCourses()
        {
            var courseBases = await GetAllCoursesBase();

            foreach (CourseInfo course in courseBases) 
            {
                var sections = await GetCourseSections(course);
                course.Sections = sections.ToArray();

                foreach (CourseSection section in sections)
                {
                    var lessons = await GetSectionLessons(section);
                    section.Lessons = lessons.ToArray();

                    foreach (CourseLesson lesson in lessons)
                    {
                        var quiz = await GetLessonQuiz(lesson);
                        lesson.Quiz = quiz;

                        var questions = await GetQuizQuestions(quiz);
                        lesson.Quiz.Questions = questions.ToArray();

                        foreach (var question in questions)
                        {
                            var answers = await GetQuestionAnswers(question);
                            question.Answers = answers.ToArray();
                        }
                    }
                }
            }

            return courseBases;
        }
        public async Task<CourseInfo> GetCourseById(string id)
        {
            var result = await GetAllCourses();
            return result.FirstOrDefault(x => x.Id.Equals(id));
        }
        public IQueryable<ApplicationUser> GetStudentsList(int courseId)
        {
            throw new NotImplementedException();
        }
    }
}
