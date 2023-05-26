namespace SchoolWebRegister.Services.Courses
{
    public class CourseQuiz
    {
        public string Id { get; set; }
        public CourseQuestion[] Questions { get; set; }
    }
    public class CourseQuestionInfo
    {
        public string Question { get; set; }
        public string Type { get; set; }
        public string Variant { get; set; }

    }
    public class CourseQuestion
    {
        public string Id { get; set; }
        public bool Checked { get; set; }
        public bool Opened { get; set; }
        public CourseQuestionInfo Info { get; set; }
        public QuizQuestionAnswer[] Answers { get; set; }
    }
}
