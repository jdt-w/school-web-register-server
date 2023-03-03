using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SchoolWebRegister.Domain.Entity
{
    [PrimaryKey(nameof(Id))]
    public sealed class Course
    {
        public string Id { get; set; }

        [MaxLength(100)]
        public string CourseName { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
        public string ImageURL { get; set; }
        public string AuthorId { get; set; }
        public ApplicationUser Author { get; set; }
        public ICollection<CourseLection> Lections { get; set; }
    }

    [PrimaryKey(nameof(Id))]
    public sealed class CourseStudying
    {
        public string Id { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<ApplicationUser> Students { get; set; }

        [Column(TypeName = "decimal(2,1)")]
        public float Progress { get; set; }
    }

    [Index(nameof(Id), IsUnique = true)]
    [PrimaryKey(nameof(Id))]
    public sealed class CourseLection
    {
        public string Id { get; set; }
        public Course Course { get; set; }
        public string ImageURL { get; set; }
        public ICollection<Quiz> Quizes { get; set; }
    }
}
