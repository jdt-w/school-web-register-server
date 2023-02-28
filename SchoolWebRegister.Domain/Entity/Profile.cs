using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Index = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace SchoolWebRegister.Domain.Entity
{
    public enum Gender : byte
    {
        Male = 0x00,
        Female = 0x01,
        Other = 0x02
    }

    [Index(nameof(Id), IsUnique = true)]
    public sealed class Profile
    {
        [Required]
        public int Id { get; set; } = -1;

        [Required(ErrorMessage = "Введите имя")]
        [StringLength(maximumLength: 30, MinimumLength = 1)]
        [Column(TypeName = "varchar(30)")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [StringLength(maximumLength: 80, MinimumLength = 1)]
        [Column(TypeName = "varchar(80)")]
        public string SecondName { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 1)]
        [Column(TypeName = "varchar(100)")]
        public string? FamilyName { get; set; }

        [Column(TypeName = "smallint")]
        public int Age { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        [Required(ErrorMessage = "Введите дату рождения")]
        [Column(TypeName = "date")]
        public DateTime Birthday { get; set; }
    }
}
