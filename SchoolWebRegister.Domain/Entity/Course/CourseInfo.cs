﻿using Microsoft.EntityFrameworkCore;

namespace SchoolWebRegister.Services.Courses
{
    [PrimaryKey(nameof(Id))]
    public class CourseSection
    {
        public string Id { get; set; }
        public bool Checked { get; set; }
        public string Title { get; set; }
        public CourseLesson[] Lessons { get; set; }
    }
    [PrimaryKey(nameof(Id))]
    public class CourseLesson
    {
        public string Id { get; set; }
        public bool Checked { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public CourseQuiz? Quiz { get; set; }
    }
    [PrimaryKey(nameof(Id))]
    public sealed class CourseInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string? AuthorID { get; set; }
        public DateTime? CreateTime { get; set; }
        public CourseSection[] Sections { get; set; }
    }
}
