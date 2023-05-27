using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SchoolWebRegister.Domain;
using SchoolWebRegister.Domain.Entity;
using SchoolWebRegister.Services.Users;

namespace SchoolWebRegister.Services.Courses
{
    public class CourseService : ICourseService
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        SqlConnection conn;
        public CourseService(IUserService userService, IConfiguration config)
        {
            _userService = userService;
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
            string insertQuery = "insert into CourseLesson(Id,Checked,Title,Content,SectionId) values (@id,@checked,@title,@content,@sectionId)";
            conn.Open();
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
            string insertQuery = "insert into CourseQuizQuestion(Id,Checked,Opened,Question,Type,Variant,QuizId) values (@id,@checked,@opened,@question,@type,@variant,@quizId)";
            conn.Open();
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
            string insertQuery = "insert into CourseQuestionAnswer(Id,Answer,IsCorrect,QuestionId) values (@id,@answer,@isCorrect,@questionId)";
            conn.Open();
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
            string insertQuery = "insert into Course(Id,Title,AuthorId,CreateTime) values (@id,@title,@authorId,@createTime)";
            conn.Open();
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
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.Course";
            cmd.Parameters.Add("id", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("title", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("authorId", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("createTime", SqlDbType.DateTime);
            cmd.Parameters["id"].Direction = ParameterDirection.Output;
            cmd.Parameters["title"].Direction = ParameterDirection.Output;
            cmd.Parameters["authorId"].Direction = ParameterDirection.Output;
            cmd.Parameters["createTime"].Direction = ParameterDirection.Output;

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
            return list;
        }
        private async Task<IEnumerable<CourseSection>> GetCourseSections(CourseInfo course)
        {
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
            return list;
        }
        private async Task<IEnumerable<CourseLesson>> GetSectionLessons(CourseSection section)
        {
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

            return list;
        }
        private async Task<CourseQuiz> GetLessonQuiz(CourseLesson lesson)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM dbo.CourseQuiz WHERE LessonId = @lessonId";
            cmd.Parameters.Add("@lessonId", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("quizId", SqlDbType.NVarChar, 450);
            cmd.Parameters["@lessonId"].Direction = ParameterDirection.Input;
            cmd.Parameters["@lessonId"].Value = lesson.Id;
            cmd.Parameters["quizId"].Direction = ParameterDirection.Output;

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

            return null;
        }
        private async Task<IEnumerable<CourseQuestion>> GetQuizQuestions(CourseQuiz quiz)
        {
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

            return list;
        }
        private async Task<IEnumerable<QuizQuestionAnswer>> GetQuestionAnswers(CourseQuestion question)
        {
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
        private async Task DeleteSection(string sectionId)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string insertQuery = "DELETE FROM dbo.CourseSection1 WHERE Id = @sectionId";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@sectionId", sectionId);
            await cmd.ExecuteNonQueryAsync();
        }
        private async Task DeleteLesson(string lessonId)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string insertQuery = "DELETE FROM dbo.CourseLesson WHERE Id = @lessonId";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@lessonId", lessonId);
            await cmd.ExecuteNonQueryAsync();
        }
        private async Task DeleteQuiz(string quizId)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string insertQuery = "DELETE FROM dbo.CourseQuiz WHERE QuizId = @quizId";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@quizId", quizId);
            await cmd.ExecuteNonQueryAsync();
        }
        private async Task DeleteQuestion(string questionId)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string insertQuery = "DELETE FROM CourseQuizQuestion WHERE Id = @questionId";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@questionId", questionId);
            await cmd.ExecuteNonQueryAsync();
        }
        private async Task DeleteQuestionAnswers(string answerId)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string insertQuery = "DELETE FROM CourseQuestionAnswer WHERE Id = @answerId";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@answerId", answerId);
            await cmd.ExecuteNonQueryAsync();
        }
        private async Task DeleteCourseBase(string courseId)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string insertQuery = "DELETE FROM Course WHERE Id = @courseId";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@courseId", courseId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<BaseResponse> DeleteCourse(string courseId)
        {
            var course = await GetCourseById(courseId);

            if (course == null)
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: "INVALID_DATA"
                );

            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            try
            {
                conn.Open();
                foreach (var section in course.Sections)
                {
                    foreach (var lesson in section.Lessons)
                    {
                        var quiz = lesson.Quiz;
                        foreach (var question in quiz.Questions)
                        {
                            foreach (var answer in question.Answers)
                            {
                                await DeleteQuestionAnswers(answer.Id);
                            }
                            await DeleteQuestion(question.Id);
                        }
                        await DeleteQuiz(quiz.Id);
                        await DeleteLesson(lesson.Id);
                    }
                    await DeleteSection(section.Id);
                }
                await DeleteCourseBase(courseId);

                return new BaseResponse(code: StatusCode.Success);
            }
            catch (Exception ex) 
            {
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: ex.Message
                );
            }
            finally
            { 
                conn.Close(); 
            }
        }
        public async Task<BaseResponse> EnrollStudent(string courseId, string studentId)
        {
            var student = await _userService.GetUserById(studentId);
            if (student == null)
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: new ErrorType
                    {
                        Message = "Неверный GUID студента.",
                        Type = new string[] { "INVALID_DATA" }
                    }
                );

            var course = await GetCourseById(courseId);
            if (course == null)
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: new ErrorType
                    {
                        Message = "Неверный GUID курса.",
                        Type = new string[] { "INVALID_DATA" }
                    }
                );

            try
            {
                SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                conn.Open();
                string insertQuery = "insert into CourseEnrollments(StudentId,CourseId,Progress) values (@studentId,@courseId,@progress)";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@courseId", courseId);
                cmd.Parameters.AddWithValue("@progress", 0);
                await cmd.ExecuteNonQueryAsync();
                conn.Close();

                return new BaseResponse(
                   code: StatusCode.Success,
                   data: courseId
               );
            }
            catch (Exception ex)
            {
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: ex.Message
                );
            }
        }
        public async Task<BaseResponse> ExpelStudent(string courseId, string studentId)
        {
            var student = await _userService.GetUserById(studentId);
            if (student == null)
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: new ErrorType
                    {
                        Message = "Неверный GUID студента.",
                        Type = new string[] { "INVALID_DATA" }
                    }
                );

            var course = await GetCourseById(courseId);
            if (course == null)
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: new ErrorType
                    {
                        Message = "Неверный GUID курса.",
                        Type = new string[] { "INVALID_DATA" }
                    }
                );

            try
            {
                SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                conn.Open();
                string insertQuery = "DELETE FROM CourseEnrollments WHERE CourseId = @courseId AND StudentId = @studentId";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@studentId", studentId);
                cmd.Parameters.AddWithValue("@courseId", courseId);
                await cmd.ExecuteNonQueryAsync();
                conn.Close();

                return new BaseResponse(
                   code: StatusCode.Success,
                   data: courseId
               );
            }
            catch (Exception ex)
            {
                return new BaseResponse(
                    code: StatusCode.Error,
                    error: ex.Message
                );
            }
        }
        public async Task<IEnumerable<CourseInfo>> GetCoursesFromStudent(string studentId)
        {
            var student = await _userService.GetUserById(studentId);
            if (student == null)
                return null;

            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            string insertQuery = "SELECT (CourseId) FROM CourseEnrollments WHERE StudentId = @studentId";
            SqlCommand cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@studentId", studentId);
            cmd.Parameters.AddWithValue("courseId", studentId);
            cmd.Parameters["courseId"].Direction = ParameterDirection.Output;

            var list = new List<CourseInfo>();
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var course = await GetCourseById(reader["courseId"].ToString());
                        list.Add(course);
                    }
                }
            }
            finally
            {
                conn.Close();
            }
            return list;
        }
        public async Task<IEnumerable<CourseInfo>> GetAllCourses()
        {
            IEnumerable<CourseInfo> courseBases;

            conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            conn.Open();

            try
            {
                courseBases = await GetAllCoursesBase();

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
            }
            finally
            {
                conn.Close();
            }
            
            return courseBases;
        }
        public async Task<CourseInfo> GetCourseById(string id)
        {
            var result = await GetAllCourses();
            return result.FirstOrDefault(x => x.Id.Equals(id));
        }
        public async Task<IEnumerable<ApplicationUser>> GetStudentsList(string courseId)
        {
            SqlConnection conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT (StudentId) FROM dbo.CourseEnrollments WHERE CourseId = @courseId";
            cmd.Parameters.Add("studentId", SqlDbType.NVarChar, 450);
            cmd.Parameters.Add("@courseId", SqlDbType.NVarChar, 450);
            cmd.Parameters["@courseId"].Direction = ParameterDirection.Input;
            cmd.Parameters["@courseId"].Value = courseId;
            cmd.Parameters["studentId"].Direction = ParameterDirection.Output;

            var list = new List<ApplicationUser>();
            try
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = await _userService.GetUserById(reader["studentId"].ToString());
                        if (user != null) list.Add(user);
                    }
                }
            }
            finally
            {
                conn.Close();
            }
            return list;
        }
    }
}
