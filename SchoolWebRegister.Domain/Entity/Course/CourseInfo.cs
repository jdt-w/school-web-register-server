using SchoolWebRegister.Domain.Entity;
using System.Text.Json.Serialization;

namespace SchoolWebRegister.Services.Courses
{
    public class CourseSection
    {
        public string Id { get; set; }
        public bool Checked { get; set; }
        public string Title { get; set; }
        public CourseLesson[] Lessons { get; set; }
    }
    public class CourseLesson
    {
        public string Id { get; set; }
        public bool Checked { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public CourseQuiz? Quiz { get; set; }
    }
    public sealed class CourseInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? AuthorID { get; set; }
        public DateTime? CreateTime { get; set; }
        public CourseSection[] Sections { get; set; }
    }
}
