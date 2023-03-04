using Microsoft.EntityFrameworkCore;

namespace SchoolWebRegister.Domain.Entity
{
    [Index(nameof(Id), IsUnique = true)]
    [PrimaryKey(nameof(Id))]
    public sealed class Student
    {
        public string Id { get; set; }
        public Profile Profile { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<CourseEnrollment> Enrollments { get; set; }
    }
}
