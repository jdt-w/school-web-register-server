using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SchoolWebRegister.Domain.Entity
{
    [Index(nameof(Id), IsUnique = true)]
    public sealed class ApplicationUser : IdentityUser
    {
        public int ProfileId { get; set; }
        public ICollection<ApplicationUserClaim> Claims { get; set; }
        public ICollection<ApplicationUserLogin> Logins { get; set; }
        public ICollection<ApplicationUserToken> Tokens { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        public override string ToString()
        {
            return string.Concat("ID = ", Id, " Login = ", UserName);
        }
    }

    public sealed class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
        public ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }

    public sealed class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }

    public sealed class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public ApplicationUser User { get; set; }
    }

    public sealed class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public ApplicationUser User { get; set; }
    }

    public sealed class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public ApplicationRole Role { get; set; }
    }

    public sealed class ApplicationUserToken : IdentityUserToken<string>
    {
        public ApplicationUser User { get; set; }
    }
}
