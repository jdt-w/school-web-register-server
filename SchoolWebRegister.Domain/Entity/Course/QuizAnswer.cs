using Microsoft.EntityFrameworkCore;

namespace SchoolWebRegister.Services.Courses
{
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Id))]
    public class AnswerInfo
    {
        public string Id { get; set; }
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
    [PrimaryKey(nameof(Id))]
    public class QuizQuestionAnswer
    {
        public string Id { get; set; }
        public AnswerInfo Info { get; set; }
        public CourseQuestion? Question { get; set; }
        public string? QuestionId { get; set; }
    }
}
