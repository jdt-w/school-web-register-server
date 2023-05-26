using Microsoft.EntityFrameworkCore;

namespace SchoolWebRegister.Services.Courses
{
    [PrimaryKey(nameof(Id))]
    public class CourseQuiz
    {
        public string Id { get; set; }
        public CourseQuestion[] Questions { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public class CourseQuestionInfo
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string Type { get; set; }
        public string Variant { get; set; }

    }
    [PrimaryKey(nameof(Id))]
    public class CourseQuestion
    {
        public string Id { get; set; }
        public bool Checked { get; set; }
        public bool Opened { get; set; }
        public CourseQuestionInfo Info { get; set; }
        public QuizQuestionAnswer[] Answers { get; set; }

        public string? QuizId { get; set; }
        public CourseQuiz? Quiz { get; set; }
    }
}
