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
        public string Id { get; set; }

        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(80)]
        public string SecondName { get; set; }

        [MaxLength(100)]
        public string? FamilyName { get; set; }

        [Column(TypeName = "smallint")]
        public int Age { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        [Column(TypeName = "date")]
        public DateTime Birthday { get; set; }
    }
}
