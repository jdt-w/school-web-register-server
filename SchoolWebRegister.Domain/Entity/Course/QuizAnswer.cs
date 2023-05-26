using Microsoft.EntityFrameworkCore;

namespace SchoolWebRegister.Services.Courses
{
    public class AnswerInfo
    {
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class QuizQuestionAnswer
    {
        public string Id { get; set; }
        public AnswerInfo Info { get; set; }
    }
}
