using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SchoolWebRegister.Services.Courses;

namespace SchoolWebRegister.Domain.Entity
{
    //[Index(nameof(Id), IsUnique = true)]
    //[PrimaryKey(nameof(Id))]
    //public sealed class Course
    //{
    //    public int Id { get; set; }

    //    [MaxLength(100)]
    //    public string? CourseName { get; set; }

    //    [MaxLength(200)]
    //    public string? Description { get; set; }
    //    public string? ImageURL { get; set; }
    //    public string AuthorId { get; set; }
    //    public ApplicationUser Author { get; set; }
    //    public ICollection<CourseLection> Lections { get; set; }
    //    public ICollection<Student> Students { get; set; }
    //    public ICollection<CourseEnrollment> Enrollments { get; set; }
    //}

    //[Index(nameof(Id), IsUnique = true)]
    //[PrimaryKey(nameof(Id))]
    //public sealed class CourseLection
    //{
    //    public int Id { get; set; }
    //    public Course Course { get; set; }

    //    [MaxLength(100)]
    //    public string? LectionName { get; set; }

    //    [MaxLength(200)]
    //    public string? Description { get; set; }
    //    public string? ImageURL { get; set; }
    //    public ICollection<Quiz> Quizes { get; set; }
    //}
}
