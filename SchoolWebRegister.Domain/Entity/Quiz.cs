using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SchoolWebRegister.Domain.Entity
{
    [Index(nameof(Id), IsUnique = true)]
    public sealed class Quiz
    {
        public string Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        [Range(minimum: 0.0, maximum: 1.0)]
        public float PassThreshold { get; set; }

        public DateTime PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }

        public ICollection<QuizQuestion> Questions { get; set; }
    }

    [Index(nameof(QuestionId), IsUnique = true)]
    [PrimaryKey(nameof(QuestionId))]
    public sealed class QuizQuestion
    {
        public string QuestionId { get; set; }
        public Quiz Quiz { get; set; }

        [MaxLength(300)]
        public string Text { get; set; }
        public bool MultiChoice { get; set; }
        public ICollection<QuizAnswer> Answers { get; set; }
    }

    [Index(nameof(AnswerId), IsUnique = true)]
    [PrimaryKey(nameof(AnswerId))]
    public sealed class QuizAnswer
    {
        public string AnswerId { get; set; }

        [MaxLength(100)]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public string QuestionId { get; set; }
        public QuizQuestion Question { get; set; }
    }
}
